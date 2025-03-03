using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

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
            StartMCServer();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            ShouldServerStop();
        }

        private static Process process = new();
        private static StreamReader? outputReader;
        private static StreamWriter? inputWriter;
        private static StringBuilder last250Chars = new StringBuilder();
        private static readonly object lockObject = new object();

        void WriteToProcess(string prompt)
        {

            process.StandardInput.WriteLine(prompt);

        }

        bool CheckForOnlinePlayer()
        {
            WriteToProcess("list");
            int i = 0;
            while (i < 100)
            {
                //string line = process.StandardOutput.ReadLine();
                Console.WriteLine(last250Chars.ToString());
                i++;
            }

            //if (line.Contains("[Server thread/INFO]: There are 0 of a max of 10 players online:")) return false;
            return true;
        }

        bool CheckForPlayerDisconnect()
        {
            string pattern = @"Server thread/INFO: (?<playerName>.+) lost connection: Disconnected";
            return Regex.IsMatch(last250Chars.ToString(), pattern);
        }

        void ShouldServerStop()
        {
            if (!CheckForOnlinePlayer()) Console.WriteLine();
            WriteToProcess("stop");
            int i = 0;
            while (i < 100)
            {
                string line = process.StandardOutput.ReadLine();
                Console.WriteLine(line);
                i++;
            }
            process.Close();
        }

        async Task MonitorPlayersAsync()
        {
            while (true)
            {
                WriteToProcess("list");
                Console.WriteLine("checking for shudown");
                await Task.Delay(1000); // Wait for a second before checking again
                ShouldServerStop();
            }
        }

        public async Task ReadAllContentFromProcess()// Die Methode ist völliger crap
        {
            ushort characterLimit = 500;


            outputReader = process.StandardOutput;

            // Event handler for output data received
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null) // Check if there's data
                {
                    // Append the new output to the StringBuilder
                    last250Chars.Append(e.Data + Environment.NewLine);

                    // If the length exceeds the character limit, trim it
                    if (last250Chars.Length > characterLimit)
                    {
                        // Keep only the last 'characterLimit' characters
                        last250Chars.Remove(0, last250Chars.Length - characterLimit);
                    }
                }
            };

            // Start reading the output stream asynchronously
            process.BeginOutputReadLine();

            // Await the process to exit
            await Task.Run(() => process.WaitForExit());
        }
        static bool IsFileInUse(string filePath)
        {
            try
            {
                // Attempt to open the file with exclusive access
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    // If we can open the file, it is not in use
                    return false;
                }
            }
            catch (IOException)
            {
                // An IOException indicates that the file is in use
                return true;
            }
        }
        void StartMCServer()
        {
            string scriptPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? @".\BMC_Server\start.ps1"
                : @".\BMC_Server\start.sh";
            Console.WriteLine(File.Exists(scriptPath));
            if (IsFileInUse(scriptPath))
            {
                Console.WriteLine("Der Server ist bereits genutzt, obwohl keine Process ge init wurde.\nforcekill? y/N?");//Replacen mit ner besseren Alternative
                string input;
                if ((input = Console.ReadLine()) == "N")
                {
                    return;
                }
                else if (input == "y")
                {
                    KillProcessByName("java.exe");
                }
            }
            ProcessStartInfo processInfo = new ProcessStartInfo
            {
                FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : "/bin/bash",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            if (process.StartInfo.CreateNoWindow != true)
            {
                // Create a new process
                process.StartInfo = processInfo;
            }

            //string arguments = "your_arguments_here"; // e.g., "google.com"
            //process.StartInfo.Arguments = arguments;
            // Start the process

            //Task.Run(() => ReadAllContentFromProcess());
            ReadAllContentFromProcess();

            process.Start();
            // Start monitoring players in a separate task
            //Task.Run(() =
            //> MonitorPlayersAsync());
            Console.WriteLine("Server Start");
        }
        static void KillProcessByName(string processName)
        {
            try
            {
                // Get all processes with the specified name
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length == 0)
                {
                    Console.WriteLine($"No process found with the name: {processName}");
                    return;
                }

                foreach (Process process in processes)
                {
                    process.Kill(); // Kill the process
                    process.WaitForExit(); // Optional: Wait for the process to exit
                    Console.WriteLine($"Killed process: {process.ProcessName} (ID: {process.Id})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}