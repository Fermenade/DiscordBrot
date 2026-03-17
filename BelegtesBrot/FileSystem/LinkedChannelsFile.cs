using BelegtesBrot.Guild.Channels;

namespace BelegtesBrot.FileSystem;

public sealed class LinkedChannelsFile : JsonFile
{
    public Session Session { get; }
    public LinkedChannelsFile(Session session) : base(session.BaseFolder.DirectoryInfo)
    {
        Session = session;
    }

    public override string Name => "LinkedChannels.json";

    public void Save(IEnumerable<LinkedChannel> linkedChannel)
    {
        SaveAsync(linkedChannel).Wait();
    }

    public IEnumerable<LinkedChannel>? Load()
    {
        return LoadAsync<LinkedChannel[]>().Result;
    }
}