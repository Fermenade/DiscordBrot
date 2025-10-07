using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.Discord;

public class Message
{
    //public DiscordSocketClient _client;
    public static async void DisplayStuffInDC(string text, IMessageChannel channel)
    {
        await channel.SendMessageAsync(text);
    }
    protected static async Task DisplayStuffInDC(EmbedBuilder embed, IMessageChannel channel)
    {
        await channel.SendMessageAsync(embed: embed.Build());
    }

    public static async Task ReplyToMessage(SocketMessage message, string replyText)
    {
        if (message is SocketUserMessage userMessage)
        {
            ISocketMessageChannel channel = userMessage.Channel;
            MessageReference messageReference = new MessageReference(userMessage.Id);

            // Antwort senden
            await channel.SendMessageAsync(replyText, messageReference: messageReference);
        }
    }
    private void PrintMessage(SocketMessage message)
    {
        Console.WriteLine($"Author: {message.Author.Username}");
        Console.WriteLine($"Content: {message.Content}");
        Console.WriteLine($"Message Type: {message.Type}");
        Console.WriteLine($"Has Embeds: {message.Embeds.Count > 0}");
        message.
        Console.WriteLine($"Has Attachments: {message.Attachments.Count > 0}");
        Console.WriteLine("-----");
    }
}