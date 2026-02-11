using Discord;

namespace BelegtesBrot;

public static class Common
{
    public static async Task ReplyToMessage(IMessage message, string replyText)
    {
        if (message is IUserMessage userMessage)
        {
            var channel = userMessage.Channel;
            var messageReference = new MessageReference(userMessage.Id);

            // Antwort senden
            await channel.SendMessageAsync(replyText, messageReference: messageReference);
        }
        else
        {
            throw new Exception($"Can't reply to message {message.GetType().Name}");
        }
    }

    public static async Task SendMessage(IMessageChannel channel, string text)
    {
        await channel.SendMessageAsync(text);
    }

    public static string FormatAsChannelMention(ulong channelId)
    {
        return $"<#{channelId}>";
    }
}