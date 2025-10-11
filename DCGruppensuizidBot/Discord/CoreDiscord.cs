using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.Discord;

class CoreDiscord
{
    private string _token { get; set; }
    public Random _random = new();
    public static DiscordSocketClient _client;

    public static DiscordShardedClient e; // TODO: Look at me!!!
    private TaskCompletionSource<bool> _readyCompletionSource = new();
    public CoreDiscord(string token)
    {
        e.JoinedGuild
        _client.JoinedGuild
        _Base._token = token;
    }
    DiscordSocketConfig config = new DiscordSocketConfig
    {
        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.Guilds |
                         GatewayIntents.GuildMessages, //Braucht es, weil baum
    };
    public CoreDiscord()
    {
        _client = new DiscordSocketClient(config);
        _client.Log += Log;//TODO: Log all stuff into file. - Not Console - except maybe fatal error

        string token = File.ReadAllText(_token); // Sike, ihr kriegt keinen Token
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        _client.Ready += () =>
        {
            _readyCompletionSource.SetResult(true); // Signal that the bot is ready
            return Task.CompletedTask; // He is Ready!!!
        };
        await _readyCompletionSource.Task;

        _client.MessageUpdated += MessageUpdated; // Update von Message
        _client.MessageDeleted +=; //Detelte
        _client.MessageReceived += MessageReceived; // Wenn neue Message

        //_ = SendRandomMessagesAsync(_cancellationTokenSource.Token); //Send Messages - cancelation is irgendwie cool.
        //_ = UpdateStatusAsync(_cancellationTokenDayTime.Token);
    }
    private Task OnReady()
    {
        _readyCompletionSource.SetResult(true); // Signal that the bot is ready
        return Task.CompletedTask; // He is Ready!!!
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask; //She is readyy!!
    }





    private async Task MessageReceived(SocketMessage message)
    {
        //AlphabetThread 

        //CommandChannel

        // if (message is SocketUserMessage userMessage && message.Author is SocketUser user) /*&&  !message.Author.IsBot*/
        // {
        //     if (message.Channel is SocketDMChannel || message.MentionedUsers.Any(u => u.Id == _client.CurrentUser.Id))
        //     {
        //         Console.WriteLine($"Message from {message.Author}: {message.Content}");
        //
        //         // Prompt for a response
        //         Console.Write("Enter your response: ");
        //         string response = Console.ReadLine();
        //         if (response == "") return;
        //         // Send the response back to the channel
        //         await message.Channel.SendMessageAsync(response);
        //     }
        // }
    }

    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage message, ISocketMessageChannel channel)
    {
        //AlphabetThread 
        if (message.Channel is SocketThreadChannel threadChannel && threadChannel.Id == Serverstuff._ThreadAlphabetBack)
        {
            await GetBotUpToDate();
            IEnumerable<IMessage> messages = await channel.GetMessagesAsync(30).FlattenAsync();
            IMessage? targetMessage = messages.FirstOrDefault(m => m.Id == message.Id);
            if (targetMessage != null)
            {
                // Get the index of the target message
                byte index = (byte)(messages.ToList().IndexOf(targetMessage) + 1);


                // Check if there is a previous message
                ushort counter = 0;
                string Comby = _LastUserMessage == null
                    ? _LastUserMessageFallback.Value
                    : GetCombination(_LastUserMessage);
                for (char first = Comby[0]; first <= 'Z'; first++)
                    for (char second = Comby[1]; second <= 'Z'; second++)
                        for (char third = Comby[2]; third <= 'Z'; third++)
                        {
                            counter++;
                            if (counter == index)
                            {
                                if ($"{first}{second}{third}" == GetCombination(targetMessage))
                                {
                                    RemoveFishReactionAsync(message);
                                }
                                else //There was a bug
                                {
                                    AddFishfReactionAsync(message);
                                }
                            }
                        }
            }
        }
    }
    public void CreateFileIfNotExists(string path)
    {
        if (!File.Exists(path)) File.Create(path);
    }

    public static SocketGuildUser GetBotID(SocketMessage message) // is diese methode so schlau?
    {
        // Get the guild (server) where the message was sent
        if (message.Channel is SocketGuildChannel guildChannel)
        {
            // Get the guild user from the message author
            return guildChannel.Guild.CurrentUser as SocketGuildUser;
        }

        return null; // Return null if the message is not in a guild
    }

}
