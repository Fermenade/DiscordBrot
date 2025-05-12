using System.Diagnostics.CodeAnalysis;

namespace LogicTesting;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class UserCommand
{
    public class CArgument
    {
        public ICommand Command { get; private set; }
        public string? Argument { get; private set; }

        public CArgument(ICommand command)
        {
            this.Command = command;
        }

        public CArgument(ICommand command, string argument)
        {
            this.Command = command;
            this.Argument = argument;
        }
    }

    public BaseCommand Command { get; private set; }

    public CArgument[]? Arguments { get; private set; }
    public object? TheThing { get; private set; }

    public UserCommand(string command) : this(command, null)
    {
    }
    public UserCommand(string command, object? theThing)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new Exception("Not a valid Command");
        if (command[0] != '!') throw new Exception("Not a valid Command");
        else command = command.TrimStart('!');
        if (command.Length == 0) throw new Exception("Not a valid Command");

        
        string[] parts = StringFormating.SmalBoom(command); //TODO: das explode sorgt auch dafür, dass der prefix "-" nicht mehr als ein einzelnes element angesehen wird
        
        this.Command = (BaseCommand)CommandManager.GetBaseCommand(parts[0])??throw new Exception("Not a valid Command");
        this.Arguments = ParseAllArguments(parts.Skip(1).ToArray());
        this.TheThing = theThing;
    }
    public UserCommand(BaseCommand command, string? arguments) : this(command, arguments, null)
    {
        //Not sure why I did this cuz I will prob, never need this. (:
    }
    public UserCommand(BaseCommand command, string? arguments, object? theThing)
    {
        this.Command = command;
        this.Arguments = ParseAllArguments(StringFormating.SmalBoom(arguments));
        this.TheThing = theThing;
    }

    CArgument[]? ParseAllArguments(string[] input)
    {
        List<CArgument> arguments = new List<CArgument>();
        for (int i = 0; i < input.Length; i++)
        {
            string arg = StringFormating.RemoveQuotes(input[i]); //TODO: dat zeuch mal verschieben un duie Argument init
            if (arg.StartsWith("-"))
            {
                
                arg = arg.Substring(1);
                //Sumcommand handeling

                ICommand subCommand = Command.GetSubCommand(arg);
                if (subCommand != null)
                {
                    if (i + 1 < input.Length && input[i + 1][0] != '-')
                    {
                        if (subCommand.TakesParameter == false)
                            throw new($"{subCommand.Name} does not take parameters");
                        i++; //So that the parameter gets ignored
                        CArgument cArgument = new(subCommand, StringFormating.RemoveQuotes(input[i]));
                        arguments.Add(cArgument);
                    }
                    else
                    {
                        if(subCommand.TakesParameter == true)
                            throw new($"{subCommand.Name} takes parameters, but 0 are given");
                        CArgument cArgument = new(subCommand);
                        arguments.Add(cArgument);
                    }
                }
                else
                {
                    throw new Exception($"{Command.Name} doesnt have an argument called '-{arg}'");
                }
            }
            else
            {
                throw new Exception($"{arg} << unknown something");
            }
        }
        return arguments.ToArray();
    }
    public static bool TryParse(string input, out UserCommand? command,out string? exeption)
    {
        try
        {
            // Attempt to parse the string into a UserCommand object.
            command = new UserCommand(input);
            exeption = null;
            return true;
        }
        catch (Exception e)
        {
            // If parsing fails, set the output parameter to its default value and return false.
            command = null;
            exeption = e.Message;
            return false;
        }
    }
}