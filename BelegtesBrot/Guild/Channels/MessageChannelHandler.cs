/*using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels;

public class MessageChannelHandler : IBaseCom, IEntity<ulong>
{
    public ulong Id { get; }

    IBaseCom _serverMessageMode;
    public MessageChannelHandler(ulong channelId,IBaseCom mode)
    {
        Id = channelId;
        _serverMessageMode = mode;
    }

    public Task MessageReceived(IMessage message)
    {
        return _serverMessageMode.MessageReceived(message);
    }

    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, IMessageChannel channel)
    {
        return _serverMessageMode.MessageUpdated(previousMessage, currentMessage, channel);
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        return _serverMessageMode.MessageDeleted(message, channel);
    }
}*/

