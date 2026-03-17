namespace BelegtesBrot.FileSystem;

public class BaseFolder : IFolder
{
    public BaseFolder(ulong id)
    {
        var stringifiedId = id.ToString();
        DirectoryInfo = new DirectoryInfo(Path.Combine(DiscordClient._directoryInfo.FullName, stringifiedId));
        if (!DirectoryInfo.Exists) DirectoryInfo.Create();
    }

    public DirectoryInfo DirectoryInfo { get; }
}