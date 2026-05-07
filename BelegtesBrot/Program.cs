using BelegtesBrot.Command;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

internal class Program
{
    private const string tokenPath = "/run/secrets/token";
    private const string devTokenPath = "./devtoken.txt";
    private const bool isDev = false;
    public static DiscordSocketClient _client;
    private static DiscordClient _discordClient;

    private static void Main(string[] args)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages //Braucht es, weil baum
        };
        _client = new DiscordSocketClient(config);

        _client.Log += message => Logger.LogMessage($"{message.Severity}: {message} - {message.Exception} {message.Source}");

        _discordClient = new DiscordClient(_client);
        
        Start();
        _discordClient.Ready();
        Task.Delay(-1).GetAwaiter().GetResult();
    }

    private static async void Start()
    {
        var token = await File.ReadAllTextAsync(isDev ? devTokenPath : tokenPath);
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.WriteLine("Token is empty");
        }
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        TaskCompletionSource<bool> _readyCompletionSource = new();
        _client.Ready += () =>
        {
            _readyCompletionSource.SetResult(true); // Signal that the bot is ready
            return Task.CompletedTask; // He is Ready!!!
        };

        await _readyCompletionSource.Task;
        await InitCommands.Init();
    }
}