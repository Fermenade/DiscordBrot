using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using BelegtesBrot.FileSystem;

namespace BelegtesBrot;

public class Session
{
    /// <summary>
    ///     Folder that stores all the info of the Session.
    /// </summary>
    public BaseFolder BaseFolder;

    public SessionLogger Logger;
    
    [JsonConstructor]
    public Session(ulong id)
    {
        Id = id;
        BaseFolder = new BaseFolder(id);
        Logger = new SessionLogger(this);
        //CommandSession = new CommandSession(this,Console.Out,Console.Out);
    }

    public ulong Id { get; }
}