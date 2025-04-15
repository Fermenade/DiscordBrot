using System.Reflection;
using System.Runtime.InteropServices;

namespace LogicTesting;

public static class CommandManager
{
    static CommandManager()
    {
        RegisterAllCommands();
    }
    private static void RegisterAllCommands() //TODO: move this stuff to the right place
    {
        // Get all types in the assembly that implement ICommand
        IEnumerable<Type> commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (Type commandType in commandTypes)
        {
            // Create an instance of the command and register it
            BaseCommand commandInstance = (BaseCommand)Activator.CreateInstance(commandType);
            CommandManager.RegisterCommand(commandInstance);
        }
    }

    public static readonly List<ICommand> _commands = new();

    private static void RegisterCommand(ICommand command)
    {
        _commands.Add(command);
    }

    public static void ExecuteCommand(UserCommand message)
    {
        message._Command?.Execute(message._arguments);
        if (message._Command == null)//If I got that right, than the upper part will not be processed when command does not exist
        {
            Console.WriteLine($"Command not found");
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
    void Execute(string? command);
    //void Execute(Command command);
}

public abstract class BaseCommand: ICommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract string Usage { get; }

    //public abstract void Execute(string[] args);
    public List<ICommand> SubCommands { get; } = new List<ICommand>();

    public void AddSubCommand(ICommand command)
    {
        if(GetSubCommand(command.Name) != null)throw new ("Subcommand with same name already exists");
        SubCommands.Add(command);
    }

    public ICommand? GetSubCommand(string name)
    {
        return SubCommands.FirstOrDefault(cmd => cmd.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public abstract void ExecuteBaseCommand();
    public void Execute(UserCommand.Argument[] userCommand)
    {
        if (userCommand.Length == 0)
        {
            ExecuteBaseCommand();
            return;
        }
        
        foreach (var VARIABLE in userCommand)
        {
            VARIABLE.command.Execute(VARIABLE.argument);
        }
        //,   Console.WriteLine($"Executing {Name} with args: {string.Join(", ", args)}");
    }
    public void Execute(string command){}
}