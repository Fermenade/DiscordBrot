using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DGruppensuizidBot.commands;

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

        class Search : ICommand
        {
            public string Name => "s";
            public string Description => "Searches for commands matching arguments";
            public bool? TakesParameter => true;
            public string Parameter => "command(s)";

            public void Execute(string? args)
            {
                if (args == null)
                {
                    Console.WriteLine("No parameters given");
                    return;
                }

                var s = StringFormating.Explode(args);
                foreach (string command in s)
                {
                    BaseCommand? baseCommand = (BaseCommand?)CommandManager.GetBaseCommand(command);
                    if (baseCommand == null||baseCommand.Visibility == false)
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

        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
        class All : ICommand
        {
            public string Name => "a";
            public string Description => "Gets all commands matching arguments";
            public bool? TakesParameter => null;
            public string Parameter => "command(s)";

            public void Execute(string? args)
            {
                StringBuilder help = new StringBuilder();
                help.AppendLine("'!command' -argument [parameter]");
                help.AppendLine("Available commands:");
                if (args != null)
                {
                    var s = StringFormating.Explode(args); //This is because when "command" does contain "command "command command""
                    ShowHelp(help, s);
                }
                else
                {
                    //help = ShowAllHelp(help); 
                    ShowAllHelp(help);
                }

                Console.WriteLine(help.ToString());
            }
        static StringBuilder ShowHelp(StringBuilder help,string[] command)
        {
            foreach (var VARIABLE in command)
            {
                var i = (BaseCommand)CommandManager.GetBaseCommand(VARIABLE);
                if (i == null)
                {
                    help.AppendLine($"{VARIABLE} unknown command");
                    continue;
                }
                ShowAllHelp(help, i);
            }
            return help;
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
                if (!command.Visibility) return;
                help.AppendLine($"{command.Name} : {command.Description}, usage: {((ICommand)command).Usage}");
                foreach (ICommand VARIABLE in command.SubCommands)
                {
                    if (VARIABLE.Visibility)
                    {
                        help.AppendLine($"\t{VARIABLE.Name} : {VARIABLE.Description}, usage: {VARIABLE.Usage}");
                    }
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

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Guthib : ICommand
        {
            public string Name => "guthib";
            public string Description => "Link to GitHub repository";
            public bool? TakesParameter => false;

            public void Execute(string? args)
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

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Stats : ICommand
        {
            public string Name => "s";
            public string Description => "Stats of alphabet";
            public bool? TakesParameter => null;
            public string Parameter => "username(s)";

            public void Execute(string? args)
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

        public override void ExecuteBaseCommand()
        {
            throw new NotImplementedException();
        }

        public class Start : ICommand //TODO: irgenwie hab ich da müll gebaut, die args werden nicht aufgerufen.
        {
            public string Name => "p";
            public string Description => "Power server";
            public bool? TakesParameter =>  false;

            public void Execute(string? args)
            {
                //MinecraftServer server = new MinecraftServer();TODO:Uncomment this line
                //server.Start();
            }
        }

        public class Console : ICommand
        {
            public string Name => "c";

            public string Description =>
                "Writes to the console of the server, it returns all content of the console for the next 10 seconds";
            public bool? TakesParameter => true;
            public bool Visibility => false;

            public void Execute(string? args)
            {
                throw new NotImplementedException();
            }
        }

        public class Stats : ICommand
        {
            public string Name => "s";
            public string Description => "Minecraft Server spezifische Statistics";
            public bool? TakesParameter => false;

            public void Execute(string? args)
            {
                if (args != null)
                {
                }

                throw new NotImplementedException();
            }
        }

        public class HallofFame : ICommand
        {
            public string Name => "h";
            public string Description => "show the hall of fame";
            public bool? TakesParameter => false;
            public void Execute(string? args)
            {

            }
        }
    }
}