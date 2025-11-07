using Discord;

namespace BelegtesBrot;

class LinkedChannel
{
    /// <summary>
    /// ID of the discord channel
    /// </summary>
    public readonly ulong ChannelId;
    /// <summary>
    /// Name of the game mode class
    /// </summary>
    public readonly string Channel;

    public LinkedChannel(IGuildChannel channelId, IServerMessageChannel channel)
    {
        ChannelId = channelId.Id;
        Channel = channel.GetType().FullName;
    }
}