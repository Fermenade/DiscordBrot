using Discord;

namespace BelegtesBrot.Guild.Channels.AlphabetMode;

public class AlphabetLogMessage
{
    private SessionLogger _sessionLogger;
    private IChannel _channel;
    public AlphabetLogMessage(SessionLogger sessionLogger,IChannel channel)
    {
        _sessionLogger = sessionLogger;
        _channel = channel;
    }
    public void LogMessage(IMessage msg, string content)
    {
        LogMessage(content, msg.Id);
    }

    public Task LogMessage(string message)
    {
        return LogMessage(message,null);
    }
    
    public Task LogMessage(string content, ulong? messageId = null)
    {
        return _sessionLogger.LogMessage($"[{_channel.Id}]{(messageId == null ? "":$"[{messageId}]")}[Alphabet] {content}");
    }
    public Task LogMessage( ulong messageId, string content)
    {
        return LogMessage(content, messageId);
    }
}