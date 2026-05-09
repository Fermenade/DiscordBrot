using System.Diagnostics;
using System.Net.Mime;
using System.Text;

namespace BelegtesBrot.MinecraftServer;

public abstract class Server
{
    private readonly Process _process;

    private readonly ProcessStartInfo _processStartInfo = new()
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
        if (!File.Exists(environmentExecutablePath))
            throw new FileNotFoundException("Server startup file not found", environmentExecutablePath);
        
        _processStartInfo.FileName = environmentExecutablePath;
        ServerRootFolder = serverRoot;
        _processStartInfo.Arguments = executablePath;
        
        _process = new Process
        {
            StartInfo = _processStartInfo
        };
        _process.OutputDataReceived += OnProcessDataReceived;
    }

    protected DirectoryInfo ServerRootFolder { get; }
    public bool Running
    {
        get
        {
            try
            {
                return !_process.HasExited;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }

    public event EventHandler<DataReceivedEventArgs> ReceivedData; // Custom event

    protected void StartProcess()
    {
        _process.Start();
        _process.BeginOutputReadLine();
    }

    protected void StopProcess()
    {
        _process.CancelOutputRead();
        _process.Kill(true);
    }

    public async Task WriteToProcess(StringBuilder stringBuilder)
    {
        if (Running) await _process.StandardInput.WriteLineAsync(stringBuilder);
    }
    private void OnProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        ReceivedData.Invoke(this, e);
    }
}