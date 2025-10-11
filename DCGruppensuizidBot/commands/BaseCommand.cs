using System.Reflection;

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
        message.Command?.Execute(message.Arguments);
        if (message.Command == null) //If I got that right, than the upper part will not be processed when command does not exist
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
    string Usage => $"-{Name}" + (TakesParameter == true ? $" {Parameter}" : TakesParameter == null ? $" ({Parameter})" : "");
    /// <summary>
    /// Custom Parameter Name for the command.
    /// </summary>
    string Parameter => "parameter(s)";
    bool? TakesParameter { get; }
    bool Visibility => true;

    void Execute(string? command);
    //void Execute(Command command);
}

public abstract class BaseCommand : ICommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public string Usage => $"!{Name}";
    public bool Visibility => true;
    public bool? TakesParameter => null;//TODO: es kann sein dass wenn diese wert nicht false ist, dann macht es etwas falsches um Usage anzeige.

    //public abstract void Execute(string[] args);
    public List<ICommand> SubCommands { get; } = new List<ICommand>();

    public void AddSubCommand(ICommand command)
    {
        if (GetSubCommand(command.Name) != null) throw new("Subcommand with same name already exists");
        SubCommands.Add(command);
    }

    public ICommand? GetSubCommand(string name)
    {
        return SubCommands.FirstOrDefault(cmd => cmd.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public abstract void ExecuteBaseCommand();

    public void Execute(UserCommand.CArgument[] userCommand)
    {
        if (userCommand.Length == 0)
        {
            ExecuteBaseCommand();
            return;
        }

        foreach (UserCommand.CArgument VARIABLE in userCommand)
        {
            VARIABLE.Command.Execute(VARIABLE.Argument);
        }
        //,   Console.WriteLine($"Executing {Name} with args: {string.Join(", ", args)}");
    }

    public void Execute(string command)
    {
    }
}