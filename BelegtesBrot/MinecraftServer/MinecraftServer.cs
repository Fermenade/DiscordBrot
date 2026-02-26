using System.Text;
using Timer = System.Timers.Timer;

namespace BelegtesBrot.BMC_Server;

public class MinecraftServer : Server
{
    private readonly Timer _checkForOnlinePlayerTimer = new();

    private readonly FileInfo _configFile;
    public readonly MCReceivedMessage McReceived;
    public readonly PlayerManager PlayerManager;
    public HallOfFame HallOfFame;
    public ServerTimeMeasure? ServerTimeMeasure;

    public MinecraftServer(DirectoryInfo startupFolder) :
        base(
            OperatingSystem.IsWindows() ? "powershell.exe" : @"/bin/bash",
            Path.Combine(startupFolder.FullName, OperatingSystem.IsWindows() ? "start.ps1" : "start.sh")
        )
    {
        McReceived = new MCReceivedMessage(this);
        McReceived.PlayerConnected += OnPlayerConnected;
        McReceived.PlayerDisconnected += OnPlayerDisconnected;
        McReceived.Ready += OnServerReady;
        McReceived.ShutdownComplete += OnServerStopped;

        _configFile = new FileInfo($"{startupFolder.FullName}/server.properties");

        PlayerManager = new PlayerManager(Convert.ToInt16(GetServerInformation("max-players")));

        //Cuz I don't want to place it inside the mc folder for 'reasons'
        HallOfFame = new HallOfFame(Environment.Parent!);
        _checkForOnlinePlayerTimer.Elapsed += StopServer;
    }

    private DirectoryInfo ServerRootFolder => Environment;

    public ServerState ServerState { get; private set; }

    private void StopServer(object? obj, EventArgs e)
    {
        WriteSomethingToServer("stop");
    }

    private void WriteSomethingToServer(string str)
    {
        WriteToProcess(new StringBuilder(str));
    }

    public void StartServer()
    {
        ServerState = ServerState.Booting;
        StartProcess();
    }

    private void OnPlayerConnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogin(playerEventArgs.Playername);

        _checkForOnlinePlayerTimer.Stop();
    }

    private void OnPlayerDisconnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        PlayerManager.PlayerLogout(playerEventArgs.Playername);
        if (!PlayerManager.CurrentOnlinePlayers.Any()) _checkForOnlinePlayerTimer.Start();
    }

    private void OnServerReady(object? sender, EventArgs args)
    {
        ServerState = ServerState.Online;
        ServerTimeMeasure = new ServerTimeMeasure();

        _checkForOnlinePlayerTimer.Start();
    }

    private void OnServerStopped(object? sender, EventArgs args)
    {
        ServerState = ServerState.Offline;

        HallOfFame.AddEntry(ServerTimeMeasure.GetTimeTillnow(),
            PlayerManager.AllPlayers.OrderBy(x => x).ToArray());
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
    public DateTime StartTime
    {
        get;
        init => field = DateTime.Now;
    }

    public TimeSpan GetTimeTillnow()
    {
        return DateTime.Now - StartTime;
    }
}