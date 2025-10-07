using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Timers;
using DGruppensuizidBot.Discord;
using Discord.WebSocket;
using Timer = System.Timers.Timer;

namespace DGruppensuizidBot.BMC_Server;

public class CoreMCServer 
{
    protected SocketUser? UserThatStartedServer;
    protected  Process? _process = null;
    protected bool ServerOnline = false;
    
    protected PlayerManager _playerManager = new();
    protected string ServerPath { get; set; }
    protected string ScriptPath {get; set;}
    private Timer checkForOnlinePlayerTimer = new()
    {
        Interval = 1000*Serverstuff.ShutdownTime,
        AutoReset = false
    };

    protected DateTime _startTime = DateTime.MinValue;
    protected  DateTime _endTime = DateTime.MaxValue;
        
    protected ProcessStartInfo processInfo = new ProcessStartInfo
    {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : @"/bin/bash",

        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"-ExecutionPolicy Bypass -File \"{Serverstuff.ScriptPath}\""
            : Serverstuff.ScriptPath,
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    /// <summary>
    /// Gets Server current state
    /// </summary>
    /// <returns>A bool of server running (true) or not (false)</returns>
    private bool GetServerOnline()
    {
        return _process != null;
    }

    protected ServerState GetServerState()
    {
        ServerState serverState;
        if (GetServerOnline())
        {
            if (ServerOnline)
            {
                serverState = ServerState.Online;
            }
            else
            {
                serverState = ServerState.Booting;
            }
        }
        else
        {
            serverState = ServerState.Offline;
        }

        return serverState;
    }
    string GetServerInformation(string optionName)
    {
        string optionFilePath = ServerPath + "option.txt";
        return GetServerInformation(optionName, optionFilePath);
    }
    /// <summary>
    /// Get a value out of the Minecraft server config file
    /// </summary>
    /// <returns>The value of an option</returns>
    string GetServerInformation(string optionName, string optionFilePath)
    {
        using (StreamReader sr = new StreamReader($"{optionFilePath}"))
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

    protected enum ServerState
    {
        Online,
        Booting,
        Offline
    }
}

public class MinecraftCoreMcServer:CoreMCServer
{
    private static List<Process>? _RunningProcesses;

    public MinecraftCoreMcServer(SocketUser socketUser):this()
    {
        UserThatStartedServer = socketUser;
    }
    public MinecraftCoreMcServer()
    {
        ServerPath = Serverstuff.ServerPath;
        ScriptPath = Serverstuff.ScriptPath;
    }
    public void Start()
    {
        if (!File.Exists(Serverstuff.ScriptPath))
        {
            throw new FileNotFoundException();
        }
        if (_RunningProcesses.Contains(_process))
        {
            throw new("Process already running");
        }

        
        _process = new();
        if (_process.StartInfo.CreateNoWindow != true)
        {
            // Create a new process
            _process.StartInfo = processInfo;
        }
        _process.Start();
        _RunningProcesses.Add(_process);

        //der InputStreamWriter muss nach dem process.Start() folgen, da sonst ein error gethrowt wird, hat garnicht lange gerbraucht das herauszufinden.
        ReadAllContentFromProcess(_process);

        //await Task.Run(() => _process.WaitForExit());
    }

    private void ReadAllContentFromProcess(Process process)
    {
        // Event handler for output data received
        process.OutputDataReceived += HandleRecivedServerData;
        process.BeginOutputReadLine();
    }

    void WriteToProcess(string prompt)
    {
        if (_process != null)
           _process.StandardInput.WriteLine(prompt);
    }

    void ListPlayerOnServer()
    {
        WriteToProcess("list");
    }

    private Timer checkForOnlinePlayerTimer;

    public void StartServerstopCountDownTimer()
    {
        checkForOnlinePlayerTimer.Start();
        checkForOnlinePlayerTimer.Elapsed += ServerStop;
    }

    public void StopServerstopCountDownTimer()
    {
        checkForOnlinePlayerTimer.Stop();
        checkForOnlinePlayerTimer.Elapsed -= ServerStop;
        checkForOnlinePlayerTimer.Dispose();
    }


    private static bool _susysusus;

    void HandleRecivedServerData(object sender, DataReceivedEventArgs e)
    {
        if (_process == null) throw new ("Missing process");
        if (!string.IsNullOrWhiteSpace(e.Data))
        {
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
                Message.DisplayStuffInDC();
                ReplyToMessage(UserThatStartedServer, "Online!");
                StartServerstopCountDownTimer(); //Der Server soll gleich in den shutdown timer laufen lassen, damit falls keine person joinen sollte der server nicht online bleibt
                _susysusus = true;
                _startTime = DateTime.Now;
                
                ServerOnline = true;
                //leuten ne sperre verpassen, die den server hochfahren ohne das dieser benutzt wird.
            }
            else if (Regex.IsMatch(data, serverShutdown)) //Server Shutdown
            {
                //Here is the server online bool set to false, so that the 
                ServerOnline = false;
                //See if Run comes into hall of fame
                HallOfFame s = new HallOfFame(CalculateDifference(),
                    _playerManager.GetAllPlayers().OrderBy(x => x).ToArray());
                FreeAllResources();
            }
            else if (Regex.IsMatch(data, playerDiconnectPattern)) //check if player disconnected
            {
                match = Regex.Match(data, playerDiconnectPattern);
                
                _playerManager.PlayerLogout(match.Groups["playername"].Value);
                
                if (!_playerManager.GetCurrentOnlinePlayers().Any())
                {
                    StartServerstopCountDownTimer();
                }
            }
            else if (Regex.IsMatch(data, playerConnectPattern)) //check if player connected
            {
                match = Regex.Match(data, playerConnectPattern);
                
                _playerManager.PlayerLogin(match.Groups["playername"].Value);
                _susysusus = false;
                
                if (checkForOnlinePlayerTimer.Enabled) //If Shutdown was Started, it will stop it
                {
                    StopServerstopCountDownTimer();
                }
            }
            // else if (Regex.IsMatch(data, serverIsEmpty)) //check if server is empty
            // {
            //     StartServerstopCountDownTimer();
            // }

            async void FreeAllResources()
            {
                UserThatStartedServer = null;
                checkForOnlinePlayerTimer.Elapsed -= ServerStop;
                checkForOnlinePlayerTimer.Dispose();
                _process.OutputDataReceived -= HandleRecivedServerData;
                await Task.Delay(1000);
                _process.Dispose();
                _process = null;
                _startTime = DateTime.MinValue;
                _endTime = DateTime.MinValue;
            }
        }
    }

    void ServerStop(object? sender, ElapsedEventArgs e) //If Timer ran out method gets triggered
    {
        // Also, jetzt kann die Frage sein: Ey, warum wird hier nicht abgeprüft ob der Server leer ist oder nicht?
        // nun, die Antwort ist: es ist nicht nötig, wenn ich alles richtig gemacht hab (Zukunftsich bitte bestätige mich)
        // dann sollte diese methode immer ausgetragen werden wenn ein spieler connected.
        if (_susysusus) //wenn jemand rein trollt und den server startet obwohl er garnicht die intention hat daraufzugehen
        {
            //TODO: Write Logic for banning People from starting Server
            //WriteScoreboard("stop");
        }

        WriteToProcess("stop"); //write the stop command to process
    }
}