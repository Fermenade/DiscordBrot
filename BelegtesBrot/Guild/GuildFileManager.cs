using BelegtesBrot.FileSystem;

namespace BelegtesBrot.Guild;

public class GuildFileManager
{
    public GuildFileManager(Session session)
    {
        BaseFolder = session.BaseFolder;
        LinkedChannelsFile = new LinkedChannelsFile(session);
    }

    public IFolder BaseFolder { get; }
    public LinkedChannelsFile LinkedChannelsFile { get; }
}