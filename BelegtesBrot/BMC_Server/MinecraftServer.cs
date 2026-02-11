using System.Text;
using Timer = System.Timers.Timer;

namespace BelegtesBrot.BMC_Server;

internal class MinecraftServer : Server
{
    private readonly Timer _checkForOnlinePlayerTimer = new();

    private readonly FileInfo _configFile;
    public readonly PlayerManager _playerManager;
    public readonly MCReceivedMessage McReceived;
    public ServerTimeMeasure? _serverTimeMeasure;
    public HallOfFame hallOfFame;

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

        ServerState = ServerState.Booting;
        _playerManager = new PlayerManager(Convert.ToInt16(GetServerInformation("max-players")));
        hallOfFame =
            new HallOfFame(Environment.Parent!); //Cuz I don't want to place it inside the mc folder for 'reasons'

        _configFile = new FileInfo($"{startupFolder.FullName}/config.json");
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
        StartProcess();
    }

    private void OnPlayerConnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        _playerManager.PlayerLogin(playerEventArgs.Playername);

        _checkForOnlinePlayerTimer.Stop();
    }

    private void OnPlayerDisconnected(object? sender, PlayerEventArgs playerEventArgs)
    {
        _playerManager.PlayerLogout(playerEventArgs.Playername);
        if (!_playerManager.CurrentOnlinePlayers.Any()) _checkForOnlinePlayerTimer.Start();
    }

    private void OnServerReady(object? sender, EventArgs args)
    {
        ServerState = ServerState.Online;
        _serverTimeMeasure = new ServerTimeMeasure();

        _checkForOnlinePlayerTimer.Start();
    }

    private void OnServerStopped(object? sender, EventArgs args)
    {
        ServerState = ServerState.Offline;

        hallOfFame.AddEntry(_serverTimeMeasure.GetTimeTillnow(),
            _playerManager.AllPlayers.OrderBy(x => x).ToArray());
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