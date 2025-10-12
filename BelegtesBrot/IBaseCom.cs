using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

internal interface IBaseCom
{
    public void MessageReceived(IMessage message);
    public void MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel);
    public void MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> message1);
}