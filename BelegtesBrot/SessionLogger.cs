using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

public class SessionLogger(Session session)
{
    public readonly Session Session = session;

    public Task LogMessage(string message) => Logger.LogMessage($"[{TimeOnly.FromDateTime(DateTime.Now).ToString()}][{Session.Id}]{message}");
    public Task LogMessage(IMessage discordMessage, string message) => LogMessage(discordMessage.Channel.Id,discordMessage.Id, message);
    public Task LogMessage(ulong channelId, ulong messageId, string message) => LogMessage($"[{channelId}][{messageId}]{message}]");
    
    public Task LogCommand(SocketSlashCommand command, string message)=> LogMessage(command.Channel.Id,command.Id,$"[Command] {command.CommandName}: {message}]");
}