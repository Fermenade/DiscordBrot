using BelegtesBrot.FileSystem;

namespace BelegtesBrot;

public class GuildFileManager
{
    public GuildFileManager(IFolder baseFolder)
    {
        BaseFolder = baseFolder;
        LinkedChannelsFile = new LinkedChannelsFile(baseFolder);
    }

    public IFolder BaseFolder { get; }
    public LinkedChannelsFile LinkedChannelsFile { get; }
}