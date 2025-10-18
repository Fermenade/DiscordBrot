using System.Diagnostics.CodeAnalysis;
using System.Text;
using BelegtesBrot.BMC_Server;
using Discord;

namespace BelegtesBrot;

partial class Commands
{
    public class Help : BaseCommand
    {
        public override string Name => "help";
        public override string Description => "Shows help";

        public Help()
        {
            AddSubCommand(new Search());
            AddSubCommand(new All());
        }

        class Search : IOptionArgument
        {
            public string Name => "s";
            public string Description => "Searches for commands matching arguments";
            public TakesParameter TakesParameter => TakesParameter.Required;
            public string Parameter => "optionArgument(s)";

            public object? Execute(string? args)
            {
                ArgumentNullException.ThrowIfNull(args);

                StringBuilder sb = new StringBuilder();
                string[] s = StringFormating.Explode(args);
                foreach (string command in s)
                {
                    BaseCommand? baseCommand = (BaseCommand?)CommandManager.GetBaseCommand(command);
                    if (baseCommand == null)
                    {
                        sb.Append($"{command} unknown optionArgument");
                        continue;
                    }

                    return ShowHelp(baseCommand);
                }

                return sb.ToString();
            }

            /// <summary>
            /// Get entire subcommand list of optionArgument
            /// </summary>
            /// <param name="command"></param>
            /// <returns>Returns entire subcommand list of optionArgument</returns>
            string ShowHelp(BaseCommand command)
            {
                StringBuilder help = new StringBuilder();
                help.AppendLine("Available arguments:");
                foreach (IOptionArgument subCommand in command.SubCommands)
                {
                    help.AppendLine($"-{subCommand.Name}");
                }

                return help.ToString();
            }
        }

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
        class All : IOptionArgument
        {
            public string Name => "a";
            public string Description => "Gets all commands matching arguments";
            public TakesParameter TakesParameter => TakesParameter.Optional;
            public string Parameter => "optionArgument(s)";

            public object? Execute(string? args)
            {
                StringBuilder help = new StringBuilder();
                help.AppendLine("'!optionArgument' -optionArgument [parameter]");
                help.AppendLine("Available commands:");
                if (args != null)
                {
                    var s = StringFormating.Explode(args); //This is because when "optionArgument" does contain "optionArgument "optionArgument optionArgument""
                    ShowHelp(help, s);
                }
                else
                {
                    //help = ShowAllHelp(help); 
                    ShowAllHelp(help);
                }

                return help.ToString();
            }
            static StringBuilder ShowHelp(StringBuilder help, string[] command)
            {
                foreach (var VARIABLE in command)
                {
                    BaseCommand? i = CommandManager.GetBaseCommand(VARIABLE);
                    if (i == null)
                    {
                        help.AppendLine($"{VARIABLE} unknown optionArgument");
                        continue;
                    }
                    ShowAllHelp(help, i);
                }
                return help;
            }

            /// <summary>
            /// Shows entire optionArgument list
            /// </summary>
            /// <returns>Returns list of all available commands</returns>
            static void ShowAllHelp(StringBuilder help)
            {
                foreach (BaseCommand command in CommandManager._commands)
                {
                    ShowAllHelp(help, command);
                }
            }

            static void ShowAllHelp(StringBuilder help, BaseCommand command)
            {
                if (!command.Visibility) return;
                help.AppendLine($"{command.Name} : {command.Description}, usage: !{command.Name}");
                foreach (IOptionArgument VARIABLE in command.SubCommands)
                {
                    if (VARIABLE.Visibility)
                    {
                        help.AppendLine($"\t{VARIABLE.Name} : {VARIABLE.Description}, usage: {VARIABLE.Usage}");
                    }
                }
            }
        }

        public override object? Execute()
        {
            return ShowHelp();
        }

        static string ShowHelp()
        {
            StringBuilder help = new StringBuilder();
            help.AppendLine("'!optionArgument' -arguments");
            help.AppendLine("Available commands:");
            foreach (BaseCommand command in CommandManager._commands)
            {
                if (command.Visibility)
                {
                    help.AppendLine($"{command.Name}");
                }
            }

            return help.ToString();
        }
    }

    public class General : BaseCommand
    {
        public override string Name => "bot";
        public override string Description => "Bot related commands";

        public General()
        {
            AddSubCommand(new Guthib());
        }

        public override object? Execute()
        {
            throw new NotImplementedException();
        }

        public class Guthib : IOptionArgument
        {
            public string Name => "guthib";
            public string Description => "Link to GitHub repository";
            public TakesParameter TakesParameter => TakesParameter.None;

            public object? Execute(string? args)
            {
                return new HiddenLink("Belegtes Brot", "https://github.com/Fermenade/GruppensuizidDC");
            }
        }
    }

    public class Alphabet : BaseCommand
    {
        public override string Name => "alphabet";
        public override string Description => "Alphabet";

        public Alphabet()
        {
            AddSubCommand(new Stats());
        }

        public override object? Execute()
        {
            throw new NotImplementedException();
        }

        public class Stats : IOptionArgument
        {
            public string Name => "s";
            public string Description => "Stats of alphabet";
            public TakesParameter TakesParameter => TakesParameter.Optional;
            public string Parameter => "username(s)";

            public object? Execute(string? args)
            {
                if (args == null)
                {
                }

                throw new NotImplementedException();
            }
        }
    }

    public class Server : BaseCommand
    {
        public override string Name => "server";
        public override string Description => "Minecraft Server spezifische Commands";

        public Server()
        {
            // Register sub-commands
            AddSubCommand(new Start());
            AddSubCommand(new Stats());
            AddSubCommand(new Console());
            AddSubCommand(new HallofFame());
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public class Start : IOptionArgument //TODO: irgenwie hab ich da müll gebaut, die args werden nicht aufgerufen.
        {
            public string Name => "p";
            public string Description => "Power server";
            public TakesParameter TakesParameter => TakesParameter.None;

            public object? Execute(string? args)
            {
                //MinecraftServer server = new MinecraftServer();TODO:Uncomment this line
                //server.Start();


            }
        }

        public class Console : IOptionArgument
        {
            public string Name => "c";

            public string Description =>
                "Writes to the console of the server, it returns all content of the console for the next 10 seconds";
            public TakesParameter TakesParameter => TakesParameter.Required;

            public bool Visibility => false;

            public object? Execute(string? args)
            {
                return null;
            }
        }

        public class Stats : IOptionArgument
        {
            public string Name => "s";
            public string Description => "Minecraft Server spezifische Statistics";
            public TakesParameter TakesParameter => TakesParameter.None;

            public object? Execute(string? args)
            {
                if (args != null)
                {}

                throw new NotImplementedException();
            }
            static EmbedBuilder BuildServerstats()
            {
                var embed = new EmbedBuilder
                {
                    Title = "Server Status",
                    Color = Color.Green,
                };
                embed.AddField("Status:", ServerInformation.GetServerState().ToString());
                if (ServerInformation.GetServerOnline())
                {
                    embed.AddField("Hobbylose:", $"{PlayerManager.GetCurrentOnlinePlayers()}");
                    embed.AddField("MC wird seit", $"{FormatDifference(CalculateDifference())} gesuchtet");
                }

                return embed;
            }

            static TimeSpan shutdownTime = TimeSpan.FromMinutes(10);

            static TimeSpan CalculateDifference()
            {
                _endTime = DateTime.Now;
                if (!ServerInformation.GetServerOnline())
                {
                    _endTime = _endTime - shutdownTime;
                }

                // Calculate the difference
                TimeSpan timeSpan = TimeSpan.FromSeconds(0);
                if (_startTime != DateTime.MinValue)
                {
                    timeSpan = _endTime - _startTime;
                }

                // Output the result
                return timeSpan;
            }
        }

        public class HallofFame : IOptionArgument
        {
            public string Name => "h";
            public string Description => "show the hall of fame";
            public TakesParameter TakesParameter => TakesParameter.None;
            public object? Execute(string? args)
            {
                return BuildServerHalloffame();
            }
            static EmbedBuilder BuildServerHalloffame()
            {
                EmbedBuilder embedBuilder = new EmbedBuilder()
                {
                    Title = "Hall Of Fame",
                    Color = Color.Gold,
                };
                var s = HallOfFame.GetEntries(); ;
                foreach (ScoreEntry VARIABLE in s)
                {
                    string a = "";
                    foreach (var VARIABLE1 in VARIABLE.Description)
                    {
                        a += VARIABLE1 + " ";
                    }

                    if (string.IsNullOrWhiteSpace(a)) break;
                    embedBuilder.AddField(FormatDifference(VARIABLE.Time), a);
                }

                return embedBuilder;
            }
            static string FormatDifference(TimeSpan timeSpan)
            {
                int years = 0;
                int months = 0;
                int days = timeSpan.Days;
                int hours = timeSpan.Hours;
                int minutes = timeSpan.Minutes; // Calculate minutes
                int seconds = timeSpan.Seconds;

                // Build the output string
                string result = $"{FormatPlural(years, "Jahr")} {FormatPlural(months, "Monat")} {FormatPlural(days, "Tag")} " +
                                $"{FormatPlural(hours, "Stunde")} {FormatPlural(minutes, "Minute")} {FormatPlural(seconds, "Sekunde")}";
                return result;
            }

            static string FormatPlural(int count, string singular)
            {
                if (count == 0 && singular != "Sekunde") return "";
                Dictionary<string, string> pluralForms = new Dictionary<string, string>
         {
             { "Jahr", "Jahre" },
             { "Monat", "Monate" },
             { "Tag", "Tage" },
             { "Stunde", "Stunden" },
             { "Minute", "Minuten" },
             { "Sekunde", "Sekunden" }
         };
                return count == 1 ? $"{count} {singular}" : $"{count} {pluralForms[singular]}";
            }
        }
    }
}