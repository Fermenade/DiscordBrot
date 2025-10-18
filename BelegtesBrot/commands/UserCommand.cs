using System.Diagnostics.CodeAnalysis;
using BelegtesBrot;

namespace BelegtesBrot;


[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class UserCommand
{
    public class CArgument
    {
        public IOptionArgument OptionArgument { get; private set; }
        public string? Argument { get; private set; }

        public CArgument(IOptionArgument optionArgument)
        {
            this.OptionArgument = optionArgument;
        }

        public CArgument(IOptionArgument optionArgument, string argument)
        {
            this.OptionArgument = optionArgument;
            this.Argument = argument;
        }
    }

    public BaseCommand Command { get; private set; }

    public CArgument[]? Arguments { get; private set; }


    public UserCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new Exception("Not a valid OptionArgument");
        if (command[0] != '!') throw new Exception("Not a valid OptionArgument");
        else command = command.TrimStart('!');
        if (command.Length == 0) throw new Exception("Not a valid OptionArgument");


        string[] parts = StringFormating.SmallBoom(command);

        this.Command = (BaseCommand)CommandManager.GetBaseCommand(parts[0]);
        this.Arguments = ParseAllArguments(parts.Skip(1).ToArray());
    }
    private CArgument[] ParseAllArguments(string[] input)
    {
        List<CArgument> arguments = new List<CArgument>();

        for (int i = 0; i < input.Length; i++)
        {
            string arg = StringFormating.RemoveQuotes(input[i]);

            if (arg.StartsWith('-'))
            {
                arg = arg[1..]; // Remove the leading hyphen

                // Check for subcommand
                IOptionArgument? subCommand = Command.GetSubCommand(arg);

                if (subCommand != null)
                {
                    // Check if a parameter is provided
                    if (i + 1 < input.Length)
                    {
                        string parameter = StringFormating.RemoveQuotes(input[i + 1]);

                        // Validate if parameter is present when it's required
                        if (subCommand.TakesParameter == TakesParameter.Required)
                        {
                            //Create an optionArgument with the subCommand and parameter
                            arguments.Add(new CArgument(subCommand, parameter));
                            i++; // Skip the parameter in the next iteration
                        }
                        else if (subCommand.TakesParameter == TakesParameter.Optional)
                        {
                            //Subcommand accepts parameter, but doesn't require it.
                            arguments.Add(new CArgument(subCommand));
                            i++;
                        }
                        else if (subCommand.TakesParameter == TakesParameter.None)
                        {
                            //TakesParameter not required and no parameter given
                            arguments.Add(new CArgument(subCommand));
                            i++;
                        }
                    }
                    else
                    {
                        //No parameter given, but optionArgument requires it
                        if (subCommand.TakesParameter == TakesParameter.Required)
                        {
                            throw new ArgumentException($"Subcommand '{subCommand.Name}' requires a parameter but none was provided.");
                        }
                        else
                        {
                            //Subcommand does not require a parameter, but still no parameter given.
                            arguments.Add(new CArgument(subCommand));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"Unknown subcommand: '{arg}'");
                }
            }
            else
            {
                throw new ArgumentException($"Not a optionArgument: '{arg}'");
            }
        }

        return arguments.ToArray();
    }
    //CArgument[]? ParseAllArguments(string[] input)
    //{
    //    List<CArgument> arguments = new List<CArgument>();
    //    for (int i = 0; i < input.Length; i++)
    //    {
    //        string arg = StringFormating.RemoveQuotes(input[i]);
    //        if (arg.StartsWith('-'))
    //        {

    //            arg = arg.Substring(1);
    //            //Sumcommand handeling

    //            IOptionArgument? subCommand = OptionArgument.GetSubCommand(arg);
    //            if (subCommand != null)
    //            {
    //                if (i + 1 < input.Length && input[i + 1][0] != '-')
    //                {
    //                    if (subCommand.TakesParameter == TakesParameter.None)
    //                        throw new($"{subCommand.Name} does not take parameters");
    //                    i++; //So that the parameter gets ignored
    //                    CArgument cArgument = new(subCommand, StringFormating.RemoveQuotes(input[i]));
    //                    arguments.Add(cArgument);
    //                }
    //                else
    //                {
    //                    if (subCommand.TakesParameter != TakesParameter.None)
    //                        throw new($"{subCommand.Name} takes parameters, but 0 are given");
    //                    CArgument cArgument = new(subCommand);
    //                    arguments.Add(cArgument);
    //                }
    //            }
    //            else
    //            {
    //                throw new Exception($"{OptionArgument.Name} doesnt have an optionArgument called '-{arg}'");
    //            }
    //        }
    //        else
    //        {
    //            throw new Exception($"{arg} << unknown something");
    //        }
    //    }
    //    return arguments.ToArray();
    //}
    public static bool TryParse(string input, out UserCommand? command, out string? exeption)
    {
        try
        {
            // Attempt to parse the string into a UserCommand object.
            command = Parse(input);
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
    public static UserCommand Parse(string input)
    {
        return new UserCommand(input);
    }
}