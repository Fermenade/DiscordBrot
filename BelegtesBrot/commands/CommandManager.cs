using System.Reflection;

namespace BelegtesBrot;

public static class CommandManager
{
    static CommandManager()
    {
        RegisterAllCommands();
    }

    private static void RegisterAllCommands() //TODO: move this stuff to the right place
    {
        // Get all types in the assembly that implement IOptionArgument
        IEnumerable<Type> commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(BaseCommand).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (Type commandType in commandTypes)
        {
            // Create an instance of the optionArgument and register it
            BaseCommand commandInstance = (BaseCommand)Activator.CreateInstance(commandType);
            CommandManager.RegisterCommand(commandInstance);
        }
    }

    public static readonly List<BaseCommand> _commands = new();

    private static void RegisterCommand(BaseCommand command)
    {
        _commands.Add(command);
    }

    public static object? ExecuteCommand(UserCommand message)
    {
        return message.Command?.Execute(message.Arguments);
    }

    public static BaseCommand? GetBaseCommand(string commandName)
    {
        return _commands.FirstOrDefault(cmd =>
            cmd.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase));
    }
}