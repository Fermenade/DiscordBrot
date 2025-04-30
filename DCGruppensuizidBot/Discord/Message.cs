using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.Discord;

public class Message:CoreDiscord, IDisplayStuffInDC
{
    //public DiscordSocketClient _client;
    protected static async Task DisplayStuffInDC(EmbedBuilder embed, IMessageChannel channel)
    {
        await channel.SendMessageAsync(embed: embed.Build());
        IMessageChannel chan = channel;
    }
    public static async Task DisplayStuffInDC(string text, IMessageChannel channel)
    {
        await channel.SendMessageAsync(text);
    }
    
    public static async Task ReplyToMessage(SocketMessage message, string replyText)
    {
        if (message is SocketUserMessage userMessage)
        {
            var channel = userMessage.Channel;
            var messageReference = new MessageReference(userMessage.Id);

            // Antwort senden
            await channel.SendMessageAsync(replyText, messageReference: messageReference);
        }
    }
    private void PrintMessage(SocketMessage message)
    {
        Console.WriteLine($"Message ID: {message.Id}");
        Console.WriteLine($"Author: {message.Author.Username}");
        Console.WriteLine($"Content: {message.Content}");
        Console.WriteLine($"Message Type: {message.Type}");
        Console.WriteLine($"Has Embeds: {message.Embeds.Count > 0}");
        Console.WriteLine($"Has Attachments: {message.Attachments.Count > 0}");
        Console.WriteLine("-----");
    }
}

public interface IDisplayStuffInDC
{
    static string ds = "123";
    public static async void DisplayStuffInDC(EmbedBuilder embed, IMessageChannel channel)
    {
        await channel.SendMessageAsync(embed: embed.Build());
        IMessageChannel chan = channel;
    }
    public static async void DisplayStuffInDC(string text, IMessageChannel channel)
    {
        await channel.SendMessageAsync(text);
    }
}