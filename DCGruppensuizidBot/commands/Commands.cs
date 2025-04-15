using System.Text;
namespace DGruppensuizidBot.commands;
partial class Commands
{
    public class Help : BaseCommand
    {
        public override string Name => "help";
        public override string Description => "Shows help";
        public override string Usage => "!help";

        public Help()
        {
            AddSubCommand(new Search());
            AddSubCommand(new All());
        }

        class Search : ICommand
        {
            public string Name => "s";
            public string Description => "Searches for commands matching arguments";
            public string Usage => "-s <command>";

            public void Execute(string[] args)
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("No parameters given");
                    return;
                }

                var s = StringFormating.Explode(args[0]);
                foreach (string command in s)
                {
                    BaseCommand? baseCommand = (BaseCommand?)CommandManager.GetBaseCommand(command);
                    if (baseCommand == null)
                    {
                        Console.WriteLine($"{command} unknown command");
                        continue;
                    }

                    Console.WriteLine(ShowHelp(baseCommand));
                }
            }

            /// <summary>
            /// Get entire subcommand list of command
            /// </summary>
            /// <param name="command"></param>
            /// <returns>Returns entire subcommand list of command</returns>
            string ShowHelp(BaseCommand command)
            {
                StringBuilder help = new StringBuilder();
                help.AppendLine("Available arguments:");
                foreach (ICommand subCommand in command.SubCommands)
                {
                    help.AppendLine($"-{subCommand.Name}");
                }

                return help.ToString();
            }
        }

        class All : ICommand
        {
            public string Name => "a";
            public string Description => "Gets all commands matching arguments";
            public string Usage => "-a";

            public void Execute(string[] args)
            {
                StringBuilder help = new StringBuilder();
                help.AppendLine("'!command' -argument [parameters]");
                help.AppendLine("Available commands:");
                if (args.Length != 0)
                {
                    var s = StringFormating.Explode(args[0]);
                    foreach (var VARIABLE in s)
                    {
                        var i = (BaseCommand)CommandManager.GetBaseCommand(VARIABLE);
                        if (i == null)
                        {
                            Console.WriteLine($"{VARIABLE} unknown command");
                            continue;
                        }

                        ShowAllHelp(help, (BaseCommand)CommandManager.GetBaseCommand(VARIABLE));
                    }
                }
                else
                {
                    //help = ShowAllHelp(help); 
                    ShowAllHelp(help);
                }

                Console.WriteLine(help.ToString());
            }

            /// <summary>
            /// Shows entire command list
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
                help.AppendLine($"{command.Name} : {command.Description}, usage: {command.Usage}");
                foreach (ICommand VARIABLE in command.SubCommands)
                {
                    help.AppendLine($"\t{VARIABLE.Name} : {VARIABLE.Description}, usage: {VARIABLE.Usage}");
                }
            }
        }

        public override void ExecuteBaseCommand()
        {
            Console.WriteLine(ShowHelp());
        }

        static string ShowHelp()
        {
            StringBuilder help = new StringBuilder();
            help.AppendLine("'!command' -arguments");
            help.AppendLine("Available commands:");
            foreach (var command in CommandManager._commands)
            {
                help.AppendLine($"{command.Name}");
            }

            return help.ToString();
        }
    }

    public class General : BaseCommand
    {
        public override string Name => "bot";
        public override string Description => "Bot related commands";
        public override string Usage => "!bot";

        public General()
        {
            AddSubCommand(new Guthib());
        }

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Guthib : ICommand
        {
            public string Name => "guthib";
            public string Description => "Link to GitHub repository";
            public string Usage => "-guthib";

            public void Execute(string[] args)
            {
                try
                {
                    Console.WriteLine("Opening GitHub repository...");
                    // Logic to open the GitHub repo
                }
                catch (Exception ex)
                {
                    throw new NotImplementedException();
                }
            }

            public void ShowHelp()
            {
            }
        }
    }

    public class Alphabet : BaseCommand
    {
        public override string Name => "alphabet";
        public override string Description => "Alphabet";
        public override string Usage => "!alphabet";

        public Alphabet()
        {
            AddSubCommand(new Stats());
        }

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Stats : ICommand
        {
            public string Name => "s";
            public string Description => "Stats of alphabet";
            public string Usage => "-s <Username>";

            public void Execute(string[] args)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class Server : BaseCommand
    {
        public override string Name => "server";
        public override string Description => "Minecraft Server spezifische Commands";
        public override string Usage => "!server";

        public Server()
        {
            // Register sub-commands
            AddSubCommand(new Start());
            AddSubCommand(new Stats());
            AddSubCommand(new HallofFame());
        }

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Start : ICommand //TODO: irgenwie hab ich da müll gebaut, die args werden nicht aufgerufen.
        {
            public string Name => "start";
            public string Description => "Start server";
            public string Usage => "-start";

            public void Execute(string[] args)
            {
                MinecraftServer server = new MinecraftServer();
                server.Start();
            }
        }

        public class Console : ICommand
        {
            public string Name => "c";

            public string Description =>
                "Writes to the console of the server, it returns all content of the console for the next 10 seconds";

            public string Usage => "-c 'parameter'";
            public bool Visibility => false;

            public void Execute(string[] args)
            {
                throw new NotImplementedException();
            }
        }

        public class Stats : ICommand
        {
            public string Name => "stats";
            public string Description => "Minecraft Server spezifische Statistics";
            public string Usage => "-stats";

            public void Execute(string[] args)
            {
                BuildServerstats();
                throw new NotImplementedException();
            }
        }

        public class HallofFame : ICommand
        {
            public string Name => "halloffame";
            public string Description => "show the hall of fame";
            public string Usage => "-halloffame";

            public void Execute(string[] args)
            {
                throw new NotImplementedException();
            }
        }
    }
}