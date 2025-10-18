using System.Diagnostics;

namespace BelegtesBrot;

public abstract class BaseCommand
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public bool Visibility => true;
    public List<IOptionArgument> SubCommands { get; } = new List<IOptionArgument>();
    
    protected void AddSubCommand(IOptionArgument optionArgument)
    {
        if (GetSubCommand(optionArgument.Name) != null) throw new("Subcommand with same name already exists");
        SubCommands.Add(optionArgument);
    }

    public IOptionArgument? GetSubCommand(string name)
    {
        return SubCommands.FirstOrDefault(cmd => cmd.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public abstract object? Execute();

    public object? Execute(UserCommand.CArgument[] userCommand)
    {
        if (userCommand.Length == 0)
        {
            return Execute();
        }

        foreach (UserCommand.CArgument VARIABLE in userCommand)
        {
            return VARIABLE.OptionArgument.Execute(VARIABLE.Argument);
        }

        return null;
    }
}