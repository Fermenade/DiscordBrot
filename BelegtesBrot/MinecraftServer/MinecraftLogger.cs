namespace BelegtesBrot.MinecraftServer;

public class MinecraftLogger
{
    SessionLogger Logger;

    public MinecraftLogger(SessionLogger logger)
    {
        Logger = logger;
    }

    public Task LogMessage(string message) => Logger.LogMessage($"[Mc] {message}");
    
}