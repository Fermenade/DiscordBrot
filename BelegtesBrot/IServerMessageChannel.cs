using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.AlphabetThread;

internal interface IServerMessageChannel : IBaseCom, IChannel
{
    public IGuildChannel Channel { get; }
}