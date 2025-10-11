using Discord;

namespace DGruppensuizidBot.AlphabetThread;

internal interface IServerMessageChannel : IBaseCom, IChannel
{
    public IGuildChannel Channel { get; }
}