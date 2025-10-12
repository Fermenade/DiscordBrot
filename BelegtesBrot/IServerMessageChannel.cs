using Discord;

namespace BelegtesBrot;

internal interface IServerMessageChannel : IBaseCom
{
    public IGuildChannel Channel { get; }
}