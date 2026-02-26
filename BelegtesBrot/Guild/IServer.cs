using BelegtesBrot.Command;
using BelegtesBrot.Guild.Channels;
using Discord;

namespace BelegtesBrot.Guild;

internal interface IServer : IBaseCom
{
    public IGuild Guild { get; }
    GuildFileManager GuildFileManager { get; }
    public CommandSession CommandSession { get; }

    public LinkedChannelManager MessageChannelManager { get; }
}