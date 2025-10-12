using Discord;

namespace BelegtesBrot;

class LinkedChannels
{
    public readonly ulong ChannelId;
    public readonly string Channel;

    public LinkedChannels(IGuildChannel channelId, IServerMessageChannel channel)
    {
        ChannelId = channelId.Id;
        Channel = channel.GetType().FullName;
    }
}