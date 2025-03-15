
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Timers;

namespace LogicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program().RunThisShit();
        }

        private void RunThisShit()
        {
            StartMcServer();
        }
            private static Process _process = new();
    List<string> OnlinePlayersDuringSession = new(10);
    List<string> ActivePlayers =  new(10);
    DateTime startTime;
    DateTime endTime;
        async void StartMcServer()
        {
            string scriptPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @".\BMC_Server\start.ps1"
                :
                @"./BMC_Server/start.sh";
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" :  @"/bin/bash",
                
                Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? $"-ExecutionPolicy Bypass -File \"{scriptPath}\"":scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            //process = new();
            if (_process.StartInfo.CreateNoWindow != true)
            {
                // Create a new process
                _process.StartInfo = processInfo;
            }
            _process.Start();
            //der InputStreamWriter muss nach dem process.Start() folgen, da sonst ein error gethrowt wird, hat garnicht lange gerbraucht das herauszufinden.
            ReadAllContentFromProcess();
            Task wait = Task.Delay(90000000);
            wait.Wait();
        }
        public void ReadAllContentFromProcess()
        {
            // Event handler for output data received
            _process.OutputDataReceived += HandleRecivedServerData;
            _process.BeginOutputReadLine();
        }
        void WriteToProcess(string prompt)
        {
            _process.StandardInput.WriteLine(prompt);
        }
        void ListPlayerOnServer()
        {
            WriteToProcess("list");
        }
        private  System.Timers.Timer  checkForOnlinePlayerTimer;
        void StartServerstopCountDownTimer()
        {
            checkForOnlinePlayerTimer = new System.Timers.Timer()
            {
                Interval = 1000 * 60 * 10,//Nach 10 min soll der 
                AutoReset = false
            };
            checkForOnlinePlayerTimer.Start();
            checkForOnlinePlayerTimer.Elapsed += ServerStop;
        }
        void StopServerstopCountDownTimer()
        {
            checkForOnlinePlayerTimer.Stop();
            checkForOnlinePlayerTimer.Elapsed -= ServerStop;
            checkForOnlinePlayerTimer.Dispose();
        }


        private bool _susysusus;
        void HandleRecivedServerData(object sender, DataReceivedEventArgs e)
        {

            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                Console.WriteLine("Output: " + e.Data);

                
                string data = e.Data;
                //[02:14:50] [Server thread/INFO]:
                string serverPrefix = @"\[(\d{2}:\d{2}:\d{2})\] \[Server thread/INFO\]:";
                string serverIsReady = @$"{serverPrefix} Done \((\d+(\.\d+)?)s\)! For help, type ""help""";

                //string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";
                
                string playerConnectPattern = @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) joined the game";
                string playerDiconnectPattern = @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) lost connection: Disconnected$";
                string serverIsEmpty = @$"{serverPrefix} There are 0 of a max of (\d+) players online:";
                // Done (0.838s)! For help, type \"help\"
                //string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";
                string serverShutdown = @$"{serverPrefix} Stopped IO worker!";
                Match match;
                if (Regex.IsMatch(data, serverIsReady))//Letzte nachricht, wenn der server hochgefahren wurde
                {
                    ListPlayerOnServer();//Der Server soll gleich in den shutdown timer laufen lassen, damit falls keine person joinen sollte der server nicht online bleibt
                    _susysusus = true;
                    startTime = DateTime.Now;
                    //leuten ne sperre verpassen, die den server hochfahren ohne das dieser benutzt wird.
                }
                else if (Regex.IsMatch(data,serverShutdown))//Server Shutdown
                {
                    FreeAllResources();
                }
                else if (Regex.IsMatch(data,playerDiconnectPattern))//check if player disconnected
                {
                    match = Regex.Match(data, playerDiconnectPattern);

                    if (match.Success)
                    {
                        string playerName = match.Groups["playername"].Value;
                        ActivePlayers.Remove(playerName);
                    }

                    ListPlayerOnServer();
                }
                else if (Regex.IsMatch(data,playerConnectPattern)) //check if player connected
                {
                    match = Regex.Match(data, playerConnectPattern);

                    if (match.Success)
                    {
                        string playerName = match.Groups["playername"].Value;
                        OnlinePlayersDuringSession.Add(playerName);
                        ActivePlayers.Add(playerName);
                    }

                    _susysusus = false;
                    if (checkForOnlinePlayerTimer.Enabled)//If Shutdown was Started, it will stop it
                    {
                        StopServerstopCountDownTimer();
                    }
                }
                else if (Regex.IsMatch(data, serverIsEmpty))//check if server is empty
                {
                    StartServerstopCountDownTimer();
                }
                
                void FreeAllResources()
                {
                    checkForOnlinePlayerTimer.Elapsed -= ServerStop;
                    checkForOnlinePlayerTimer.Dispose();
                    _process.OutputDataReceived -= HandleRecivedServerData;
                    _process.Dispose();
                    _process = null;
                }
            }
            
        }
        void ServerStop(object? sender, ElapsedEventArgs e)//If Timer ran out method gets triggered
        {
            // Also, jetzt kann die Frage sein: Ey, warum wird hier nicht abgeprüft ob der Server leer ist oder nicht?
            // nun, die Antwort ist: es ist nicht nötig, wenn ich alles richtig gemacht hab (Zukunftsich bitte bestätige mich)
            // dann sollte diese methode immer ausgetragen werden wenn ein spieler connected.
            if (_susysusus)//wenn jemand rein trollt und den server startet obwohl er garnicht die intention hat daraufzugehen
            {
            }
            
                Console.WriteLine("Server stopping");
                WriteToProcess("stop");//write the stop command to process
        }
    }
}