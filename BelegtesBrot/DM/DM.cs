using Discord;

namespace BelegtesBrot.DM;

internal class DM : Session
{
    private IDMChannel _channel;

    public DM(ulong id, IDMChannel channel) : base(id)
    {
        _channel = channel;
    }
}