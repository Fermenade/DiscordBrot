using BelegtesBrot;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BelegtesBrot.BMC_Server;

public class MCServer
{
    private static MCServer? mcServer;
    private const string ServerPath = $"{Info.InfoFolder}/MinecraftServer";
    public event EventHandler<DataReceivedEventArgs> ReceivedData; // Custom event
    private Process _process;
    public bool Running => !_process.HasExited;

    private ProcessStartInfo processInfo = new ProcessStartInfo
    {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : @"/bin/bash",

        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"-ExecutionPolicy Bypass -File \"{ServerPath}/start.ps1\""
            : $"{ServerPath}/start.sh",
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };
    private MCServer()
    {
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
        // Potentially modify/filter/process the data here if needed.
        ReceivedData?.Invoke(this, e);
    }
}
public enum ServerState
{
    Online,
    Booting,
    Offline
}

public delegate void EmptyHandler<T>();
public class HigherMC
{
    private MCServer mcServer = MCServer.GetMcServer();
    private PlayerManager PlayerManager;
    private ServerTimeMeasure ServerTimeMeasure = new ServerTimeMeasure();

    public ServerState ServerState
    {
        get;
        private set;
    }
    public event EmptyHandler<HigherMC> ServerReady; 
    void StopServer(object? obj, EventArgs e)
    {
        WriteSomethingToServer("stop");
    }

    void WriteSomethingToServer(string str)
    {
        mcServer.WriteToProcess(new StringBuilder(str));
    }

    public HigherMC()
    {
        mcServer.ReceivedData += HandleReceivedServerData;
        mcServer.StartProcess();
        ServerState = ServerState.Booting;
        PlayerManager = new PlayerManager(Convert.ToInt16(GetServerInformation("max-players")));
    }
    void HandleReceivedServerData(object? sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) return;

        string data = e.Data;


        //[02:14:50] [Server thread/INFO]:
        // string serverPrefix = @"$$(\\d{2}:\\d{2}:\\d{2})$$ $$Server thread/INFO$$:";
        // string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";
        string serverPrefix = @"\[(\d{2}:\d{2}:\d{2})\] \[Server thread/INFO\]:";
        string serverIsReady = @$"{serverPrefix} Done \((\d+(\.\d+)?)s\)! For help, type ""help""";

        //string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";

        string playerConnectPattern = @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) joined the game";
        string playerDiconnectPattern =
            @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) lost connection: Disconnected$";
        string serverIsEmpty = @$"{serverPrefix} There are 0 of a max of (\d+) players online:.*";
        // Done (0.838s)! For help, type \"help\"
        string serverShutdown = @$"{serverPrefix} Stopped IO worker!";
        //serverShutdown = @$"{serverPrefix} ThreadedAnvilChunkStorage: All dimensions are saved";


        Match match;
        if (Regex.IsMatch(data, serverIsReady)) //Letzte nachricht, wenn der server hochgefahren wurde
        {
            //Der Server soll gleich in den shutdown timer laufen lassen, damit falls keine person joinen sollte der server nicht online bleibt

            ServerState = ServerState.Online;
            ServerTimeMeasure.StartTime = DateTime.Now;
            ServerReady.Invoke();
            //leuten ne sperre verpassen, die den server hochfahren ohne das dieser benutzt wird.
        }
        else if (Regex.IsMatch(data, serverShutdown)) //Server Shutdown
        {
            //Here is the server online bool set to false, so that the 
            ServerState = ServerState.Offline;
            //See if Run comes into hall of fame
            HallOfFame s = new HallOfFame(ServerTimeMeasure.GetTimeTillnow(),
                PlayerManager.GetAllPlayers().OrderBy(x => x).ToArray());

        }
        else if (Regex.IsMatch(data, playerDiconnectPattern)) //check if player disconnected
        {
            match = Regex.Match(data, playerDiconnectPattern);

            PlayerManager.PlayerLogout(match.Groups["playername"].Value);

            if (!PlayerManager.GetCurrentOnlinePlayers().Any())
            {
                StartServerstopCountDownTimer();
            }
        }
        else if (Regex.IsMatch(data, playerConnectPattern)) //check if player connected
        {
            match = Regex.Match(data, playerConnectPattern);

            PlayerManager.PlayerLogin(match.Groups["playername"].Value);
        }
        //else if (Regex.IsMatch(data, serverIsEmpty)) //check if server is empty
        //{
        //    StartServerstopCountDownTimer();
        //}
    }
    string GetServerInformation(string optionName)
    {
        using (StreamReader sr = new StreamReader())
        {
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (s.StartsWith(optionName))
                {
                    s = s.Split("=")[1].Trim();
                    return s;
                }
            }
        }

        return optionName;
    }
    private System.Timers.Timer checkForOnlinePlayerTimer = new();
    private void StartServerstopCountDownTimer()
    {
        checkForOnlinePlayerTimer.Start();
        checkForOnlinePlayerTimer.Elapsed += StopServer;
    }
}

public class HigestMC
{
    //This class interacts with discord stuff.
}

public class ServerTimeMeasure
{
    public DateTime StartTime;
    public TimeSpan GetTimeTillnow()
    {
        return DateTime.Now - StartTime;
    }
}