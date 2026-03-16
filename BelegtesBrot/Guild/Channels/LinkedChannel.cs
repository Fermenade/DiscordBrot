using System.Text.Json.Serialization;

namespace BelegtesBrot.Guild.Channels;

public sealed class LinkedChannel
{
    [JsonConstructor]
    private LinkedChannel(ulong channelId, string modeName, Session session)
    {
        ChannelId = channelId;
        ModeName = modeName;
        Session = session;
    }

    // Serialized properties
    public ulong ChannelId { get; init; }
    public string ModeName { get; init; }
    public Session Session { get; init; }

    // Runtime-only property (not serialized)
    [JsonIgnore] public IBaseCom Channel => field ??= CreateChannel();

    public static LinkedChannel Create(Session session, ulong channelId, string modeName)
    {
        ValidateMode(modeName);
        return new LinkedChannel(channelId, modeName, session);
    }

    private IBaseCom CreateChannel()
    {
        var modeType = ModeChannelMapper.GetMode(ModeName)
                       ?? throw new InvalidOperationException($"Mode '{ModeName}' does not exist");

        if (!typeof(IBaseCom).IsAssignableFrom(modeType))
            throw new InvalidOperationException($"{modeType.FullName} does not implement IBaseCom");

        return (IBaseCom)(Activator.CreateInstance(modeType, ChannelId, Session)
                          ?? throw new InvalidOperationException($"Could not create instance of {modeType.FullName}"));
    }

    private static void ValidateMode(string modeName)
    {
        if (string.IsNullOrWhiteSpace(modeName))
            throw new ArgumentException("Mode name cannot be null or empty", nameof(modeName));

        if (ModeChannelMapper.GetMode(modeName) is null)
            throw new ArgumentException($"Mode '{modeName}' does not exist", nameof(modeName));
    }
}