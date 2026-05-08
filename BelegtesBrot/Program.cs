using System.Runtime.InteropServices;
using BelegtesBrot.Command;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

internal class Program
{
    private const string tokenPath = "/run/secrets/token";
    private const string devTokenPath = "./devtoken.txt";
    private const bool isDev = true;
    public static DiscordSocketClient _client;
    private static DiscordClient _discordClient;
    
    public static CancellationTokenSource ShutdownComplete = new();
    public static CancellationTokenSource ShutdownCts = new();
    private static async Task<int> Main(string[] args)
    {
        Console.Title = "BelegtesBrot";
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            OnShutdown();
        };
        using var termSignalRegistration =
            PosixSignalRegistration.Create(
                PosixSignal.SIGTERM,
                (_) => OnShutdown());
        // Replicates the previous behavior on Windows
        using var sigHupSignalRegistration =
            PosixSignalRegistration.Create(
                PosixSignal.SIGHUP,
                (_) => OnShutdown());
        ShutdownCts.Token.Register(() =>
        {
            if (_client.LoginState == LoginState.LoggedIn)
            {
                _client.LogoutAsync().Wait();
            }

            _client.StopAsync();
            _client.Dispose();
        });
        
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

        try
        {
            await Task.Delay(-1,ShutdownComplete.Token);
        }
        catch (OperationCanceledException)
        {}
        return 0;
    }

    private static void OnShutdown()
    {
        Console.WriteLine("Shutting down...");
        ShutdownComplete.CancelAfter(TimeSpan.FromSeconds(60));
        ShutdownCts.Cancel();               // signal cancellation
        ShutdownComplete.Cancel();
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