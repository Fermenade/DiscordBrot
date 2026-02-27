using Discord;

namespace BelegtesBrot.Guild.Channels.AlphabetMode;

public static class AlphabetLogMessage
{
    public static void LogMessage(IMessage msg, string content)
    {
        LogMessage(((IGuildChannel)msg.Channel).GuildId,msg.Channel.Id, msg.Id, content);
    }

    public static async void LogMessage(ulong guildId, ulong channelId, ulong messageId,string content)
    {
        await Logger.LogMessage($"[{guildId}][{channelId}][{messageId}][Alphabet] {content}");
    }
}