using Discord;

namespace BelegtesBrot;

internal interface IServerMessageChannel : IBaseCom, IChannel
{
    public IGuildChannel Channel { get; }
}