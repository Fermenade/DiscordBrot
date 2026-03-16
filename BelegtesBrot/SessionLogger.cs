namespace BelegtesBrot;

public class SessionLogger(Session session)
{
    public readonly Session Session = session;

    public Task LogMessage(string message) => Logger.LogMessage($"[{TimeOnly.FromDateTime(DateTime.Now).ToString()}][{Session.Id}]{message}");
}