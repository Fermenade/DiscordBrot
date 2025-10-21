using System.CommandLine;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace LogicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string e = Console.ReadLine();
            new Program().RunThisShit("! manage -h");
        }

        private void RunThisShit(string args)
        {
            var e = CreateRootCommand();
            var x = e.Parse(Console.ReadLine());
            x.InvokeAsync();
        }

        private Command CreateRootCommand()
        {
            Command command = new Command("!", "Bot related commands")
            {
                Options = { new HelpOption()},
            };
            // 'manage' command (parent for start, stats, stop)
            var manage = new Command("server", "Manage the service");

            // 'start' subcommand
            var start = new Command("start", "Start the service");
            start.SetAction(service =>
            {
                // start logic...
            });

            // 'stats' subcommand
            var stats = new Command("stats", "Show service stats");
            stats.SetAction(service =>
            {
                // pretend to retrieve stats
                var uptime = TimeSpan.FromHours(5.5);
                var requests = 12345;

                Console.WriteLine($"Service: {service}");
                Console.WriteLine($"Uptime : {uptime}");
                Console.WriteLine($"Requests: {requests}");
            });
            Option<bool> forceOpt = new Option<bool>("force", "--force", "-F")
            {
                Description = "apply some force",
                Hidden = true
            };
            // 'stop' subcommand
            Command stop = new Command("stop", "Stop the service")
            {
                forceOpt
            };

            stop.SetAction(parseResult =>
            {
                Process process = new();
                bool forceStop = parseResult.GetValue(forceOpt);//Kills the process
                if (forceStop)
                {
                    process.Kill();
                }
                else
                {
                    //write 'stop' to process
                }
            });

            // Assemble command tree
            manage.Add(start);
            manage.Add(stats);
            manage.Add(stop);

            command.Add(manage);

            Command commandGamemodeRegister = new("register");//Gamemode Management command
            Command commandGamemodeRemove = new("remove");//Gamemode Management command
            Command commandGamemodeReload = new("reload");//Gamemode Management command

            Command commandGamemode = new("gamemode")
            {
                commandGamemodeRegister,
                commandGamemodeRemove,
                commandGamemodeReload
            };//Gamemode Management command



            var alphabet = new Command("alphabet", "alphabet command");
            alphabet.SetAction(service => {});
            Argument<long> argument = new Argument<long>("channel")
            {
                CustomParser = result =>
                {
                    string argumentValue = result.Tokens[0].Value;
                    Regex regex = new Regex(@"\<#(?<channelID>\\d*)>");
                    Match e = regex.Match(argumentValue);
                    if (!e.Success)
                    {
                        result.AddError($"{result.Argument.Name} was not a valid channel");
                        return 0;
                    }

                    return long.Parse(e.Groups["channelID"].Value);
                }
            };
            alphabet.Add(argument);
            Command fools = new("fools", "fools of the alphabet");
            //Argument<long> user = new Argument<long>("");
            Option<long> OptionUser = new Option<long>("user","-p","--profile"){
                Description = "Specify user",
                CustomParser = result =>
                {
                    string argumentValue = result.Tokens[0].Value;
                    Regex regex = new Regex(@"\<@(?<userID>\\d*)>");
                    Match e = regex.Match(argumentValue);
                    if (!e.Success)
                    {
                        result.AddError($"{result.Argument.Name} was not a valid user");
                        return 0;
                    }

                    return long.Parse(e.Groups["userID"].Value);
                }
            };
            fools.SetAction(result =>
            {
                long? nUser = result.GetValue(OptionUser);
                if (nUser.HasValue)
                {
                    long user = nUser.Value;
                    //show place of user;
                }
            });
            fools.Add(OptionUser);
            alphabet.Add(fools);

            Command AlphabetStats = new Command("stats", "get alphabet stats")
            {
            };


            command.Add(alphabet);
            return command;
        }
    }
}