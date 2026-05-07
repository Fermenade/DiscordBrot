using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ChildMurderer;

class Program
{
// args:
    // 0: parent process ID (int)
    // 1: child executable path
    // 2: child arguments (single string, may be empty)
    // 3: optional shutdown token to send to child stdin
    static async Task<int> Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Usage: wrapper <parentPid> <exePath> <exeArgs> [shutdownToken]");
            return 1;
        }

        if (!int.TryParse(args[0], out var parentPid))
        {
            Console.Error.WriteLine("Invalid parent PID.");
            return 1;
        }

        var exePath = args[1];
        var exeArgs = args[2];
        var shutdownToken = args.Length >= 4 ? args[3] : null;

        Process parentProcess = null;
        try
        {
            parentProcess = Process.GetProcessById(parentPid);
        }
        catch
        {
            // Parent already gone → treat as "parent exited"
        }

        using var child = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = exeArgs,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            EnableRaisingEvents = true
        };

        try
        {
            if (!child.Start())
            {
                Console.Error.WriteLine("Failed to start child process.");
                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to start child process: {ex.Message}");
            return 1;
        }

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        // Forward child stdout/stderr to our stdout/stderr
        var stdoutTask = ForwardStreamAsync(child.StandardOutput, Console.Out, token);
        var stderrTask = ForwardStreamAsync(child.StandardError, Console.Error, token);

        // Forward our stdin to child stdin
        var stdinTask = ForwardStreamAsync(Console.In, child.StandardInput, token, closeDestinationOnEnd: true);

        // Monitor parent exit (if we could attach)
        Task parentExitTask = Task.CompletedTask;
        if (parentProcess != null)
        {
            var tcs = new TaskCompletionSource<object?>();
            parentProcess.EnableRaisingEvents = true;
            parentProcess.Exited += (_, __) => tcs.TrySetResult(null);
            parentExitTask = tcs.Task;
        }

        // Also monitor child exit
        var childExitTcs = new TaskCompletionSource<object?>();
        child.Exited += (_, __) => childExitTcs.TrySetResult(null);
        var childExitTask = childExitTcs.Task;

        // If parent already gone, parentExitTask is completed
        if (parentProcess == null || parentProcess.HasExited)
        {
            parentExitTask = Task.CompletedTask;
        }

        // Wait for either parent exit or child exit
        var first = await Task.WhenAny(parentExitTask, childExitTask);

        if (first == parentExitTask)
        {
            // Parent exited → initiate shutdown of child
            await InitiateShutdownAsync(child, shutdownToken, token);
        }

        // Ensure child is not still running
        if (!child.HasExited)
        {
            TryKillProcessTree(child);
        }

        // Cancel forwarding tasks and wait them out
        cts.Cancel();
        try
        {
            await Task.WhenAll(stdoutTask, stderrTask, stdinTask);
        }
        catch (OperationCanceledException)
        {
            // Expected on cancellation
        }

        // Wait for child to fully exit
        try
        {
            child.WaitForExit();
        }
        catch { }

        return child.HasExited ? child.ExitCode : 1;
    }

    private static async Task ForwardStreamAsync(TextReader source, TextWriter destination, CancellationToken token, bool closeDestinationOnEnd = false)
    {
        char[] buffer = new char[4096];
        try
        {
            while (!token.IsCancellationRequested)
            {
                int read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), token);
                if (read <= 0)
                    break;

                await destination.WriteAsync(buffer.AsMemory(0, read), token);
                await destination.FlushAsync(token);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal on cancellation
        }
        finally
        {
            if (closeDestinationOnEnd)
            {
                try { destination.Flush(); } catch { }
                try { destination.Dispose(); } catch { }
            }
        }
    }

    private static async Task InitiateShutdownAsync(Process child, string? shutdownToken, CancellationToken externalToken)
    {
        if (child.HasExited)
            return;

        // Try graceful shutdown via stdin token
        if (!string.IsNullOrEmpty(shutdownToken) && child.StartInfo.RedirectStandardInput)
        {
            try
            {
                await child.StandardInput.WriteLineAsync(shutdownToken);
                await child.StandardInput.FlushAsync();
            }
            catch
            {
                // Ignore failures (child may have closed stdin)
            }
        }

        // Wait a bit for graceful exit
        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, externalToken);

        try
        {
            while (!child.HasExited && !linked.Token.IsCancellationRequested)
            {
                await Task.Delay(200, linked.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // Timeout or external cancellation
        }
    }

    private static void TryKillProcessTree(Process child)
    {
        try
        {
            // .NET Core / .NET 5+ supports killing the process tree cross-platform
            child.Kill(entireProcessTree: true);
        }
        catch
        {
            // Fallback: try simple kill
            try { child.Kill(); } catch { }

            // On Unix, as a last resort, send kill to process group
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    // Best-effort: kill process group using 'kill -TERM -<pgid>'
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "kill",
                        Arguments = $"-TERM -{child.Id}",
                        UseShellExecute = false
                    })?.Dispose();
                }
                catch { }
            }
        }
    }
}