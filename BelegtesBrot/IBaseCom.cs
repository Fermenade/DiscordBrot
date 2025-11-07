using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

public interface IBaseCom
{
    public Task MessageReceived(IMessage message);
    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel);
    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel);
}