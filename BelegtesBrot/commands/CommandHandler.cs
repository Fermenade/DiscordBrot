using Discord;

namespace BelegtesBrot.commands;

public class CommandHandler
{
    Commands commands =  new Commands();
    public void ReceivedMessage(IMessage message)
    {
        UserCommand userCommand = new UserCommand(message);
        commands.root.Parse(userCommand);
    }
}