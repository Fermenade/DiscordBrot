using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.AlphabetThread;

internal interface IServerMessageChannel : IBaseCom
{
    public IMessageChannel Channel { get; }
}