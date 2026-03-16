using BelegtesBrot.Command;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Guild;

internal class Server : Session, IServer
{
    public Server(IGuild guild) : base(guild.Id)
    {
        Logger.LogMessage($"Handling started");
        Guild = guild;
        GuildFileManager = new GuildFileManager(this);
        MessageChannelManager = new LinkedChannelManager(this);
        CommandSession = new CommandSession(this);
    }

    public IGuild Guild { get; }
    public GuildFileManager GuildFileManager { get; }
    public CommandSession CommandSession { get; }
    public LinkedChannelManager MessageChannelManager { get; }

    public Task MessageReceived(IMessage message)
    {
        try
        {
            Logger.LogMessage(message,"Received message");
            foreach (var messageChannel in MessageChannelManager.LinkedChannels)
                if (messageChannel.ChannelId == message.Channel.Id)
                {
                    return messageChannel.Channel.MessageReceived(message);
                }
        }
        catch (Exception e)
        {
            Logger.LogMessage($"{e.Message}\n" +
                              $"{e.StackTrace}");
        }

        return Task.CompletedTask;
    }

    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage,
        IMessageChannel channel)
    {
        try
        {
            Logger.LogMessage(currentMessage,"Updated message");
            foreach (var messageChannel in MessageChannelManager.LinkedChannels)
                if (messageChannel.ChannelId == channel.Id)
                {
                    return messageChannel.Channel.MessageUpdated(previousMessage, currentMessage, channel);
                }
        }
    
        catch (Exception e)
        {
            Logger.LogMessage($"{e.Message}\n" +
                              $"{e.StackTrace}");
        }
        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        try
        {
            Logger.LogMessage(channel.Id,message.Id,"Deleted message");
            foreach (var messageChannel in MessageChannelManager.LinkedChannels)
                if (messageChannel.ChannelId == channel.Value.Id)
                {
                    return messageChannel.Channel.MessageDeleted(message, channel);
                }
        }
        catch (Exception e)
        {
            Logger.LogMessage($"{e.Message}\n" +
                              $"{e.StackTrace}");
        }
        
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand command)
    {
        try
        {
            Logger.LogCommand(command, "Received command");
            return CommandSession.Command(command);
        }
        catch (Exception e)
        {
            Logger.LogMessage($"{e.Message}\n" +
                              $"{e.StackTrace}");
        }
        return Task.CompletedTask;
    }
}