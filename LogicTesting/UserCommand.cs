namespace LogicTesting;

public class UserCommand
{
    public class Argument
    {
        public ICommand command { get; private set; }
        public string? argument {get; private set;}

        public Argument(ICommand command)
        {
            this.command = command;
        }
        public Argument(ICommand command, string argument)
        {
            this.command = command;
            this.argument = argument;
        }
    }

    public BaseCommand _Command { get; private set; }

    public Argument[] _arguments { get; private set; }
    public object? dasDing { get; private set; }

    public UserCommand(string command) : this(command, null)
    {
    }

    public UserCommand(string command, object? dasDing)
    {
        string input = command;
        if (string.IsNullOrWhiteSpace(input)) throw new Exception("Not a valid Command"); //TODO: schaun ob das return probleme macht
        if(input.Length == 1) throw new Exception("Not a valid Command");
        if (input[0] != '!') throw new Exception("Not a valid Command");
        else input = input.TrimStart('!');
        

        string[] parts = StringFormating.SmalBoom(input); //TODO: das explode sorgt auch dafür, dass der prefix "-" nicht mehr als ein einzelnes element angesehen wird
        _Command = (BaseCommand)CommandManager.GetBaseCommand(parts[0]);
        var args = parts.Skip(1).ToArray();
        List<Argument> arguments = new List<Argument>();
        
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];//TODO: dat zeuch mal verschieben un duie Argument init
            if (arg.StartsWith("-"))
            {
                arg = arg.Substring(1);
                //Sumcommand handeling
                
                ICommand subCommand = _Command.GetSubCommand(arg);
                if (subCommand != null)
                {
                    i++;
                    if(args[i][0]!='-'){//Dies müsste den index increasen und gleivhzeitig den nächten index verwenden
                    Argument argument = new(subCommand, StringFormating.RemoveQuotes(args[i]));
                    arguments.Add(argument);
                    }
                    else
                    {
                        Argument argument = new(subCommand);
                        arguments.Add(argument);
                        continue;
                        //Assumning that this is a no parameter argument
                    }
                }
                else
                {
                    Console.WriteLine($"{_Command.Name} doesnt have an argument called '{arg}'");
                }
            }
            else
            {
                throw new Exception("unknown something");
            }
        }
        _arguments = arguments.ToArray();
        
        this.dasDing = dasDing;
    }
}