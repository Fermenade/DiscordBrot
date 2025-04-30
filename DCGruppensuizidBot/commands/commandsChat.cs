using DGruppensuizidBot.Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.commands;

public class commandsChat:Message
{
    public commandsChat()
    {

        _client.MessageReceived += ClientOnMessageReceived;
    }

    private Task ClientOnMessageReceived(SocketMessage message)
    {
        if (message.Channel.Id == Serverstuff._TBoardCommands)
        {
            if (message.Author != GetBotID(message))//da ansonsten
            {
                try
                {
                    UserCommand command = new(message.Content);
                    CommandManager.ExecuteCommand(command);
                }
                catch (Exception e)
                {
                    DisplayStuffInDC(e.Message,message.Channel);
                }
            }
        }
    }
}