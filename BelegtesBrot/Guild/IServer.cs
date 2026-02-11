using Discord;

namespace BelegtesBrot;

internal interface IServer : IBaseCom
{
    public IGuild Guild { get; }
    GuildFileManager GuildFileManager { get; }
    public CommandSession CommandSession { get; }

    public LinkedChannelManager MessageChannelManager { get; }
}