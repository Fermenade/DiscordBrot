using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

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
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == message.Channel.Id)
                return messageChannel.Channel.MessageReceived(message);

        return Task.CompletedTask;
    }

    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage,
        IMessageChannel channel)
    {
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == channel.Id)
                return messageChannel.Channel.MessageUpdated(previousMessage, currentMessage, channel);

        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        foreach (var messageChannel in MessageChannelManager.LinkedChannels)
            if (messageChannel.ChannelId == channel.Value.Id)
                return messageChannel.Channel.MessageDeleted(message, channel);

        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand command)
    {
        CommandSession.Command(command);
        return Task.CompletedTask;
    }
}