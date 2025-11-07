using Discord.WebSocket;

namespace BelegtesBrot;

public interface IServerMessageChannel : IBaseCom
{
    public SocketTextChannel Channel { get; }
}