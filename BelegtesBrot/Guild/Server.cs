using BelegtesBrot.Command;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Guild;

internal class Server : Session, IServer
{
    public Server(IGuild guild) : base(guild.Id)
    {
        Guild = guild;
        GuildFileManager = new GuildFileManager(BaseFolder);
        MessageChannelManager = new LinkedChannelManager(this);
        CommandSession = new CommandSession(this);
    }

    public IGuild Guild { get; }
    public GuildFileManager GuildFileManager { get; }
    public CommandSession CommandSession { get; }
    public LinkedChannelManager MessageChannelManager { get; }

    public Task MessageReceived(IMessage message)
    {
        Logger.LogMessage($"Received message {message.Id} in {message.Channel.Id}");
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == message.Channel.Id)
            {
                Logger.LogMessage($"Message {message.Id} handled with mode {messageChannel.ModeName}");
                return messageChannel.Channel.MessageReceived(message);
            }

        return Task.CompletedTask;
    }

    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage,
        IMessageChannel channel)
    {
        Logger.LogMessage($"Updated message {currentMessage.Id} in {channel.Id}");
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == channel.Id)
            {
                Logger.LogMessage($"Message {currentMessage.Id} handled with mode {messageChannel.ModeName}");
                return messageChannel.Channel.MessageUpdated(previousMessage, currentMessage, channel);
            }
        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        Logger.LogMessage($"Deleted message {message.Id} in {channel.Id}");
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == channel.Value.Id)
            {
                Logger.LogMessage($"Message {message.Id} handled with mode {messageChannel.ModeName}");
                return messageChannel.Channel.MessageDeleted(message, channel);
            }
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand command)
    {
        Logger.LogMessage($"Command [{command.Id}] by [{command.User.Id}] executed: {command.CommandName}");
        return CommandSession.Command(command);
    }
}