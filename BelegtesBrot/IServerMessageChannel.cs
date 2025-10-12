using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

internal interface IServerMessageChannel : IBaseCom
{
    public SocketTextChannel Channel { get; }
}