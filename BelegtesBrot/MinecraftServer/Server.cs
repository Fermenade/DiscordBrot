using System.Diagnostics;
using System.Text;

namespace BelegtesBrot.MinecraftServer;

public abstract class Server
{
    private readonly Process _process;

    private readonly ProcessStartInfo processInfo = new()
    {
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    /// <summary>
    ///     Create an instance of McServer
    /// </summary>
    /// <param name="environmentExecutablePath">Path to environment of server</param>
    /// <param name="executablePath">Path to the server executable</param>
    protected Server(DirectoryInfo serverRoot, string environmentExecutablePath, string executablePath)
    {
        if (!File.Exists(executablePath))
            throw new FileNotFoundException("Server startup file not found", executablePath);
        
        processInfo.FileName = environmentExecutablePath;
        processInfo.Arguments = (ServerRootFolder = serverRoot).FullName;

        _process = new Process
        {
            StartInfo = processInfo
        };
        _process.OutputDataReceived += OnProcessDataReceived;
    }

    protected DirectoryInfo ServerRootFolder { get; }
    public bool Running => !_process.HasExited;
    public event EventHandler<DataReceivedEventArgs> ReceivedData; // Custom event

    protected void StartProcess()
    {
        _process.Start();
    }

    public async void WriteToProcess(StringBuilder stringBuilder)
    {
        if (Running) await _process.StandardInput.WriteLineAsync(stringBuilder);
    }
    private void OnProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        Logger.LogMessage(e.Data);
        ReceivedData.Invoke(this, e);
    }
}