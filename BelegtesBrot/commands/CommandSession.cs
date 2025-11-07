using System.CommandLine;
using System.CommandLine.Parsing;
using BelegtesBrot.commands;
using Discord;

namespace BelegtesBrot;

//Create a session class that has all base functionality of a session?
public class CommandSession
{
    private CommandsCollection _commandsCollection;
    private PermissionManager _permissionManager = new PermissionManager();

    public CommandSession(List<IServerMessageChannel> serverMessageChannels,TextWriter output, TextWriter error) : this(output, error)
    {
        _commandsCollection = new CommandsCollection(serverMessageChannels,_permissionManager);
    }
    private CommandSession(TextWriter output, TextWriter error)
    {
        invocationConfiguration.Output =  output;
        invocationConfiguration.Error = error;
    }
    
    private InvocationConfiguration invocationConfiguration = new InvocationConfiguration();
    
    void CommandReceived(IMessage message)
    {
        IUserCommand userCommand = new UserCommand(message,_permissionManager);
        var parseResult = _commandsCollection.root.Parse(userCommand);
        if (parseResult.Errors.Any())
        {
            //TODO: print errors
            return;
        }
        parseResult.Invoke(invocationConfiguration);
    }

    private void SavePermissions()
    {
        throw new NotImplementedException();
    }

    private void LoadPermissions()
    {
        throw new NotImplementedException();
    }
}