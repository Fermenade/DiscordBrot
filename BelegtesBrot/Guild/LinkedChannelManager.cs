using System.Collections.Immutable;

namespace BelegtesBrot;

internal class LinkedChannelManager
{
    private readonly List<LinkedChannel> _linkedChannels;
    private readonly IServer _server;

    private List<LinkedChannel>? _linkedChannelsCopy;

    public LinkedChannelManager(IServer server)
    {
        _linkedChannels = server.GuildFileManager.LinkedChannelsFile.Load()?.ToList() ?? [];
        _server = server;
    }

    public ImmutableArray<LinkedChannel> LinkedChannels
    {
        get
        {
            if (_linkedChannelsCopy?.GetHashCode() == _linkedChannels.GetHashCode()) return field;

            _linkedChannelsCopy = new List<LinkedChannel>(_linkedChannels);
            field = [.._linkedChannels];
            return field;
        }
    }

    public void Add(ulong channelId, string channelType)
    {
        Add(LinkedChannel.Create(channelId, channelType));
    }

    private void Add(LinkedChannel channel)
    {
        if (_linkedChannels.Any(x => x.ChannelId == channel.ChannelId))
            throw new Exception("A mode is already linked to that channel");
        _linkedChannels.Add(channel);
        _server.GuildFileManager.LinkedChannelsFile.Save(LinkedChannels);
    }

    public void Remove(ulong channelId)
    {
        _linkedChannels.Remove(_linkedChannels.First(x => x.ChannelId == channelId));
        _server.GuildFileManager.LinkedChannelsFile.Save(LinkedChannels);
    }
}