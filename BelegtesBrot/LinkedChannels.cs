using Discord;

namespace BelegtesBrot;

class LinkedChannels
{
    public readonly IGuildChannel ChannelId;
    public readonly string Channel;

    public LinkedChannels(IGuildChannel channelId, IServerMessageChannel channel)
    {
        ChannelId = channelId;
        Channel = channel.GetType().FullName;
    }
}