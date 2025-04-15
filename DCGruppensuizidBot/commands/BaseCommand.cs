using System.Reflection;
using Discord.WebSocket;

namespace DGruppensuizidBot.commands;

public class Command
{
    public class Argument
    {
        ICommand _command;
        string _argument;

        Argument(ICommand command, string argument)
        {
            _command = command;
            _argument = argument;
        }
    }
    public ICommand? _Command { get; private set; }
    
    public Argument[] _arguments { get; private set; }
    public object? dasDing { get; private set; }
    
    public Command(string command):this(command,null)
    { }
    public Command(string command, object? dasDing)
    {
        string input = command;
        if (string.IsNullOrWhiteSpace(input)) return;//TODO: schaun ob das probleme macht
        if(input[0] != Char.Parse("!")) return;
        else
        {
            input = input.Substring(1);
        }

        string[] parts = StringFormating.Explode(input);
        string commandName = parts[0];
        
        var args = parts.Skip(1).ToArray();
        foreach (var arg in parts.Skip(1))
        {
            if (arg.StartsWith("-"))
            {
                //Sumcommand handeling
                var subCommand =  GetSubCommand(args[0].Remove(0, 1));
                if (subCommand != null)
                {
                    //Argument handeling
                    subCommand.Execute(args.Skip(1).ToArray());
                }
                else
                {
                    Console.WriteLine($"{Name} doesnt have an argument called '{args[0]}'");
                }

                args = args.Skip(2).ToArray();
                Argument argument = new(subCommand, args);
            }
            else
            {
                Console.WriteLine($"{Name} doesnt have an subcomand called '{args[0]}'");
            }
        }
        
        _Command = CommandManager.GetBaseCommand(commandName);
        
        this.dasDing = dasDing;
    }
    
}
public class CommandManager
{
    public CommandManager()
    {
        RegisterAllCommands(new());
    }
    static void RegisterAllCommands(CommandManager commandManager) //TODO: move this stuff to the right place
    {
        // Get all types in the assembly that implement ICommand
        IEnumerable<Type> commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (Type commandType in commandTypes)
        {
            // Create an instance of the command and register it
            BaseCommand commandInstance = (BaseCommand)Activator.CreateInstance(commandType);
            commandManager.RegisterCommand(commandInstance);
        }
    }

    public static readonly List<ICommand> _commands = new();

    public void RegisterCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public static void ExecuteCommand(Command message)
    {
        message._Command?.Execute(message);
        if (message._Command == null)//If I got that right, than the upper part will not be processed when command does not exist
        {
            Console.WriteLine($"{message._args[0]} << Command not found");
        }
    }

    public static ICommand? GetBaseCommand(string commandName)
    {
        return _commands.FirstOrDefault(cmd =>
            cmd.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
    }
}

public interface ICommand
{
    string Name { get; }
    string Description { get; }
    string Usage { get; }
    void Execute(Command command);
}

public abstract class BaseCommand : ICommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract string Usage { get; }

    //public abstract void Execute(string[] args);
    public List<ICommand> SubCommands { get; } = new List<ICommand>();

    public void AddSubCommand(ICommand command)
    {
        SubCommands.Add(command);
    }
    

    public abstract void ExecuteBaseCommand();
    private bool _isFirstRun = true;

    public void Execute(Command command)
    {
        string[] args = command._args;
        if (args.Length > 0)
        {
            _isFirstRun = false;
            // if (args.First().StartsWith("-"))
            // {
            //     //Sumcommand handeling
            //     var subCommand = GetSubCommand(args[0].Remove(0, 1));
            //     if (subCommand != null)
            //     {
            //         //Argument handeling
            //         subCommand.Execute(args.Skip(1).ToArray());
            //     }
            //     else
            //     {
            //         Console.WriteLine($"{Name} doesnt have an argument called '{args[0]}'");
            //     }
            //
            //     args = args.Skip(2).ToArray();
            // }
            // else
            // {
            //     Console.WriteLine($"{Name} doesnt have an subcomand called '{args[0]}'");
            // }

            Execute(args.Skip(1).ToArray());
        }
        else if (_isFirstRun)
        {
            ExecuteBaseCommand();
        }

        _isFirstRun = true;
        //,   Console.WriteLine($"Executing {Name} with args: {string.Join(", ", args)}");
    }
}