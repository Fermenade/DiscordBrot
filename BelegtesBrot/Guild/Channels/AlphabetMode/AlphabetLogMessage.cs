using Discord;

namespace BelegtesBrot.Guild.Channels.AlphabetMode;

public static class AlphabetLogMessage
{
    public static void LogMessage(IMessage msg, string content)
    {
        LogMessage(msg.Id, content);
    }

    public static async void LogMessage(ulong id, string content)
    {
        await Logger.LogMessage($"[{id}] {content}");
    }
}