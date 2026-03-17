using System.Text;
using Timer = System.Timers.Timer;

namespace BelegtesBrot.MinecraftServer;

public class MinecraftServer : Server
{
    private readonly Timer _checkForOnlinePlayerTimer = new()
    {
        Enabled = false,
        AutoReset = false,
        Interval = 1000*60*10
    };

    private readonly FileInfo _configFile;
    public readonly MCReceivedMessage McReceived;
    public readonly PlayerManager PlayerManager;
    public HallOfFame HallOfFame;
    public ServerTimeMeasure? ServerTimeMeasure = new ServerTimeMeasure();
    public new DirectoryInfo ServerRootFolder => base.ServerRootFolder;
    private DirectoryInfo _nonMcMcDataFolder;
    public Session ServerSession;
    private MinecraftLogger _logger;

    public ServerState? MinecraftServerState { get; private set; }

    public MinecraftServer(DirectoryInfo startupFolder, Session session) :
        base(startupFolder,
            OperatingSystem.IsWindows() ? "powershell.exe" : "/bin/bash",
            Path.Combine(startupFolder.FullName, OperatingSystem.IsWindows() ? "start.ps1" : "start.sh")
        )
    {
        ServerSession = session;
        _logger = new MinecraftLogger(ServerSession.Logger);
        _nonMcMcDataFolder = new DirectoryInfo(ServerRootFolder.FullName + "/DiscordData");
        
        McReceived = new MCReceivedMessage(this, _logger);
        McReceived.PlayerConnected += OnPlayerConnected;
        McReceived.PlayerDisconnected += OnPlayerDisconnected;
        McReceived.Ready += OnServerReady;
        McReceived.ShutdownComplete += OnServerStopped;

        _configFile = new FileInfo($"{ServerRootFolder.FullName}/server.properties");

        PlayerManager = new PlayerManager(Convert.ToInt16(GetServerInformation("max-players")));
        HallOfFame = new HallOfFame(_nonMcMcDataFolder);
        
        _checkForOnlinePlayerTimer.Elapsed += StopServer;
        AppDomain.CurrentDomain.ProcessExit += StopServer;
    }

    private void StopServer(object? obj, EventArgs e)
    {
        WriteSomethingToServer("stop");
    }

    private void WriteSomethingToServer(string str)
    {
        _logger.LogMessage("To server: " + str);
        WriteToProcess(new StringBuilder(str));
    }

    public void StartServer()
    {
        _logger.LogMessage("Server Started");
        MinecraftServerState = ServerState.Booting;
        StartProcess();
    }

    private void OnPlayerConnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogin(playerEventArgs.Playername);
        _logger.LogMessage($"Player {playerEventArgs.Playername} connected, stopping shutdown timer.");
        _checkForOnlinePlayerTimer.Stop();
    }

    private void OnPlayerDisconnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogout(playerEventArgs.Playername);
        _logger.LogMessage($"Player {playerEventArgs.Playername} disconnected");
        if (!PlayerManager.CurrentOnlinePlayers.Any())
        {
            _logger.LogMessage($"Empty. Starting shutdown timer.");
            _checkForOnlinePlayerTimer.Start();
        }
    }

    private void OnServerReady(object? sender, EventArgs args)
    {
        MinecraftServerState = ServerState.Online;
        ServerTimeMeasure = new ServerTimeMeasure();
        
        _logger.LogMessage("Server Ready, falling into wait for shutdown.");
        _checkForOnlinePlayerTimer.Start();
    }

    private void OnServerStopped(object? sender, EventArgs args)
    {
        MinecraftServerState = ServerState.Offline;
        _logger.LogMessage("Server Stopped");
        HallOfFame.AddEntry(ServerTimeMeasure.GetTimeTillnow(),
            PlayerManager.AllPlayers.OrderBy(x => x).ToArray());
        
        StopProcess();
    }

    private string GetServerInformation(string optionName)
    {
        if (!_configFile.Exists) throw new FileNotFoundException($"Config file not found a: '{_configFile.FullName}'");
        using var sr = new StreamReader(_configFile.FullName);
        while (!sr.EndOfStream)
        {
            var s = sr.ReadLine();
            if (s.StartsWith(optionName))
            {
                s = s.Split("=")[1].Trim();
                return s;
            }
        }

        return optionName;
    }
}

public enum ServerState
{
    Online,
    Booting,
    Offline
}

public class ServerTimeMeasure
{
    public ServerTimeMeasure()
    {
        StartTime = DateTime.Now;
    }
    public DateTime StartTime { get; }

    public TimeSpan GetTimeTillnow()
    {
        return DateTime.Now - StartTime;
    }
}