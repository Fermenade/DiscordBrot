using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BelegtesBrot.BMC_Server;

public class MCServer
{
    private static MCServer? mcServer;
    public string ServerPath = $"{Info.InfoFolder}/MinecraftServer";
    public event EventHandler<DataReceivedEventArgs> ReceivedData; // Custom event
    private Process _process;
    public bool Running => !_process.HasExited;

    private ProcessStartInfo processInfo = new ProcessStartInfo
    {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : @"/bin/bash",
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };
    private MCServer()
    {
        processInfo.Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"-ExecutionPolicy Bypass -File \"{ServerPath}/start.ps1\""
            : $"{ServerPath}/start.sh";
        _process = new Process()
        {
            StartInfo = processInfo,
        };
        _process.OutputDataReceived += OnProcessDataReceived;
        mcServer = this;
    }
    public static MCServer GetMcServer()
    {
        if (mcServer != null) return mcServer;

        return mcServer = new MCServer();
    }
    
    public void StartProcess()
    {
        _process.Start();
    }

    public async void WriteToProcess(StringBuilder stringBuilder)
    {
        if (Running)
        {
            await _process.StandardInput.WriteLineAsync(stringBuilder);
        }
    }
    private void OnProcessDataReceived(object sender, DataReceivedEventArgs e)
    {
        ReceivedData?.Invoke(this, e);
    }
}
public enum ServerState
{
    Online,
    Booting,
    Offline
}

public delegate void EmptyHandler();
public class HigherMc
{
    private MCServer mcServer = MCServer.GetMcServer();
    private PlayerManager PlayerManager;
    private System.Timers.Timer checkForOnlinePlayerTimer = new();
    private ServerTimeMeasure ServerTimeMeasure = new ServerTimeMeasure();
    private MCReceivedMessage McReceived;
    public ServerState ServerState
    {
        get;
        private set;
    }
    public event EmptyHandler ServerReady;
    void StopServer(object? obj, EventArgs e)
    {
        WriteSomethingToServer("stop");
    }

    void WriteSomethingToServer(string str)
    {
        mcServer.WriteToProcess(new StringBuilder(str));
    }

    public HigherMc()
    {
        McReceived = new MCReceivedMessage(mcServer);
        McReceived.PlayerConnected += OnPlayerConnected;
        McReceived.PlayerDisconnected += OnPlayerDisconnected;
        McReceived.Ready += OnServerReady;
        McReceived.ShutdownComplete += OnServerStopped;

        ServerState = ServerState.Booting;
        PlayerManager = new PlayerManager(Convert.ToInt16(GetServerInformation("max-players")));

        _configFile = $"{mcServer.ServerPath}/config.json";
        checkForOnlinePlayerTimer.Elapsed += StopServer;
    }

    public void StartServer()
    {
        mcServer.StartProcess();
    }
    private void OnPlayerConnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogin(playerEventArgs.Playername);
        
        checkForOnlinePlayerTimer.Stop();
    }
    private void OnPlayerDisconnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogout(playerEventArgs.Playername);
        if (!PlayerManager.GetCurrentOnlinePlayers().Any())
        {
            checkForOnlinePlayerTimer.Start();
        }
    }
    private void OnServerReady(object? sender, EventArgs args)
    {
        ServerState = ServerState.Online;
        ServerTimeMeasure.StartTime = DateTime.Now;
        
        checkForOnlinePlayerTimer.Start();
    }
    private void OnServerStopped(object? sender, EventArgs args)
    {
        ServerState = ServerState.Offline;

        HallOfFame.AddEntry(ServerTimeMeasure.GetTimeTillnow(),
            PlayerManager.GetAllPlayers().OrderBy(x => x).ToArray());
    }

    private readonly string _configFile;
    string GetServerInformation(string optionName)
    {
        using StreamReader sr = new StreamReader(_configFile);
        while (!sr.EndOfStream)
        {
            string s = sr.ReadLine();
            if (s.StartsWith(optionName))
            {
                s = s.Split("=")[1].Trim();
                return s;
            }
        }

        return optionName;
    }
}

public class ServerTimeMeasure
{
    public DateTime StartTime;
    public TimeSpan GetTimeTillnow()
    {
        return DateTime.Now - StartTime;
    }
}