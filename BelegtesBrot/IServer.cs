using Discord;

namespace BelegtesBrot;

internal interface IServer : IBaseCom
{
    public IGuild Guild { get; }
    public List<IServerMessageChannel> MessageChannels { get; }
}