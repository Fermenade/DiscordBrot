using System.Text;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

public class SessionLogger(Session session)
{
    public readonly Session Session = session;

    public Task LogMessage(string message)
    {
        return Logger.LogMessage($"[{Session.Id}]{message}");
    }

    public Task LogMessage(IMessage discordMessage, string message)
    {
        return LogMessage(discordMessage.Channel.Id, discordMessage.Id, message);
    }

    public Task LogMessage(ulong channelId, ulong messageId, string message)
    {
        return LogMessage($"[{channelId}][{messageId}]{message}");
    }

    public Task LogCommand(SocketSlashCommand command, string message)
    {
        return LogMessage(command.Channel.Id, command.Id,
            $"[Command] {message}: {command.CommandName} {command.Data.Name} {string.Join(';',command.Data.Options.Select(x => BuildNodeString(x)))}");
    }

    private static string BuildNodeString(SocketSlashCommandDataOption node, int indent = 0)
    {
        if (node == null) return string.Empty;
        var sb = new StringBuilder();
        sb.AppendLine(new string(' ', indent * 2) + node.Name);
        sb.AppendLine(new string(' ', indent * 2) + node.Value);
        sb.AppendLine(new string(' ', indent * 2) + node.Type);
        
        foreach (var child in node.Options) sb.Append(BuildNodeString(child, indent + 1));
        return sb.ToString();
    }
}