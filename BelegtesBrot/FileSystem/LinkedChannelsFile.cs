using BelegtesBrot.Guild.Channels;

namespace BelegtesBrot.FileSystem;

public sealed class LinkedChannelsFile : JsonFile
{
    public LinkedChannelsFile(IFolder baseFolder) : base(baseFolder.DirectoryInfo)
    {
    }

    public override string Name => "LinkedChannels.json";

    public void Save(IEnumerable<LinkedChannel> linkedChannel)
    {
        SaveAsync(linkedChannel).Wait();
    }

    public IEnumerable<LinkedChannel>? Load()
    {
        return LoadAsync<IEnumerable<LinkedChannel>>().Result;
    }
}