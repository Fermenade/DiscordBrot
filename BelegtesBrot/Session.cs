using BelegtesBrot.FileSystem;

namespace BelegtesBrot;

internal class Session
{
    /// <summary>
    ///     Folder that stores all the info of the Session.
    /// </summary>
    public BaseFolder BaseFolder;

    public Session(ulong id)
    {
        Id = id;
        BaseFolder = new BaseFolder(id);

        //CommandSession = new CommandSession(this,Console.Out,Console.Out);
    }

    public ulong Id { get; }
}