using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.AlphabetThread;

public interface IServerMessageChannel
{
    public IMessageChannel Channel { get; }
    public ulong Id => Channel.Id;
    Task ProcessMessageQueue(Cacheable<IMessageChannel, ulong> channel); // Delete //TODO: take a look at me
    Task MessageReceived(IMessage message);

    Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage message, ISocketMessageChannel channel);
}