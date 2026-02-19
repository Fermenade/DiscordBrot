using System.Diagnostics;
using System.Text;

namespace BelegtesBrot.BMC_Server;

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
    protected Server(string environmentExecutablePath, string executablePath)
    {
        if (!File.Exists(executablePath))
            throw new FileNotFoundException("Server startup file not found", executablePath);
        Environment = new DirectoryInfo(executablePath);

        processInfo.FileName = environmentExecutablePath;
        processInfo.Arguments = (Environment = new (executablePath)).FullName;

        _process = new Process
        {
            StartInfo = processInfo
        };
        _process.OutputDataReceived += OnProcessDataReceived;
    }

    public DirectoryInfo Environment { get; }
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
        ReceivedData?.Invoke(this, e);
    }
}