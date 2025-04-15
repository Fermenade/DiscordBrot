using Discord;
using Discord.WebSocket;
using System.Collections.Concurrent;

using System.Text;
using System.Text.Json;
using DGruppensuizidBot;
using DGruppensuizidBot.commands;

class Program
{
    private DiscordSocketClient _client;

    static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();
    private SocketUserMessage _LastUserMessage;
    

    private KeyValuePair<IUser, string> _LastUserMessageFallback;

    //private readonly ulong _ThreadAlphabetBack = 1215011525195075636;
    //private readonly ulong _TBoardChernobil = 1186307797453918259;
    private TaskCompletionSource<bool> _readyCompletionSource = new();
    private CancellationTokenSource _cancellationTokenSource = new();
    private CancellationTokenSource _cancellationTokenDayTime = new();
    private readonly Random _random = new();
    SocketGuildUser botID;
    SocketMessage Deletedmessage;
    byte TeaThinkCounter = 0;
    ushort messageCount;

    public async Task RunBotAsync() //Async
    {
        DiscordSocketConfig config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages, //Braucht es, weil baum
        };


        _client = new DiscordSocketClient(config);
        _client.Log += Log;

        if (!Directory.Exists(_prefixPath))
        {
            Directory.CreateDirectory(_prefixPath);
        }

        string token = File.ReadAllText(_pathToken); // Sike, ihr kriegt keinen Token
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        _client.Ready +=
            OnReady; //I'm ready!!! I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready I'm ready

        await _readyCompletionSource.Task;

        await GetBotUpToDate(); //Get Bot up to date - thats what the method says
        _client.MessageUpdated += MessageUpdated; // Update von Message
        _client.MessageReceived += MessageReceived; // Wenn neue Message
        _client.MessageDeleted += MessageDeleted; //Detelte


        //_ = SendRandomMessagesAsync(_cancellationTokenSource.Token); //Send Messages - cancelation is irgendwie cool.
        _ = UpdateStatusAsync(_cancellationTokenDayTime.Token);
        // Block the program until it is closed (:
        await Task.Delay(-1);
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

    private ConcurrentQueue<Cacheable<IMessage, ulong>> _messageQueue =
        new ConcurrentQueue<Cacheable<IMessage, ulong>>(); //Interfaces :)

    private bool _isProcessingQueue = false; // damit wenn noch in arbeit weiter gearbeitet wird

    private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage,
        Cacheable<IMessageChannel, ulong> channel)
    {
        //if (channel.Id == _ThreadAlphabetBack)
        {
            // Enqueue the message
            _messageQueue.Enqueue(cachedMessage); //enqhene

            // Start processing if not already processing
            if (!_isProcessingQueue) //wenn nicht dann doch
            {
                _isProcessingQueue = true;
                await ProcessMessageQueue(channel); //asnyc
                _isProcessingQueue = false;
            }
        }
    }

    private async Task ProcessMessageQueue(Cacheable<IMessageChannel, ulong> channel)
    {
        while (_messageQueue.TryDequeue(out var cachedMessage))
        {
            if (channel.Id == _ThreadAlphabetBack)
            {
                if (Deletedmessage != null)
                {
                    // Check if the message is cached
                    // why was  thjis even nessesary
                    if (cachedMessage.Id != Deletedmessage.Id)
                    {
                        GetBotUpToDate();
                        _LastUserMessage = null;
                    }
                }
                else
                {
                    GetBotUpToDate();
                    //Spongebob
                    _LastUserMessage = null;
                }
            }

            await Task.Delay(0); //await zerrrrrooooooo!
        }
    }

    public async Task ReplyToMessage(SocketMessage message, string replyText)
    {
        if (message is SocketUserMessage userMessage)
        {
            var channel = userMessage.Channel;
            var messageReference = new MessageReference(userMessage.Id);

            // Antwort senden
            await channel.SendMessageAsync(replyText, messageReference: messageReference);
        }
    }

    private async Task MessageReceived(SocketMessage message)
    {
        if (message.Channel is SocketThreadChannel threadChannel && threadChannel.Id == _ThreadAlphabetBack)
        {
            if (CheckFormat(message))
            {
                if (_LastUserMessage != null)
                {
                    if (message.Author != _LastUserMessage.Author)
                    {
                        if (GetCombination(message) != GetNextCombination(GetCombination(_LastUserMessage)))
                        {
                            AddReactionAsync(message);
                            _LastUserMessageFallback = new KeyValuePair<IUser, string>(message.Author,
                                GetNextCombination(GetCombination(_LastUserMessage)));
                            _LastUserMessage = null;
                        }
                        else
                        {
                            // Update the last message to the current one
                            _LastUserMessage = (SocketUserMessage)message;
                        }
                    }
                    else
                    {
                        Deletedmessage = message;
                        await message.DeleteAsync(); //wenn schon dann nicht mehr
                    }
                }
                else
                {
                    /* TODO: fix this (unknown cause)
                     * Unhandled exception. Discord.Net.HttpException: The server responded with error 10008: Unknown Message
   at Discord.Net.Queue.RequestBucket.SendAsync(RestRequest request)
   at Discord.Net.Queue.RequestQueue.SendAsync(RestRequest request)
   at Discord.API.DiscordRestApiClient.SendInternalAsync(String method, String endpoint, RestRequest request)
   at Program.RemoveFishReactionAsync(SocketMessage message) in /mnt/0_WorkWindows/Users/dumblecore/source/repos/GruppensuizidDC/DCGruppensuizidBot/Program.cs:line 666
   at System.Threading.Tasks.Task.<>c.<ThrowAsync>b__128_1(Object state)
   at System.Threading.ThreadPoolWorkQueue.Dispatch()
   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()
                     */
                    if (_LastUserMessageFallback.Key.Id != message.Author.Id)
                    {
                        if (GetCombination(message) != GetNextCombination(_LastUserMessageFallback.Value))
                        {
                            AddReactionAsync(message);
                            Console.WriteLine(GetCombination(message));
                            Console.WriteLine(GetNextCombination(_LastUserMessageFallback.Value));
                            _LastUserMessageFallback = new KeyValuePair<IUser, string>(message.Author,
                                GetNextCombination(_LastUserMessageFallback.Value));
                            _LastUserMessage = null;
                        }
                        else
                        {
                            _LastUserMessage = (SocketUserMessage)message;
                        }
                    }
                    else
                    {
                        Deletedmessage = message;
                        await message.DeleteAsync();
                    }
                }
            }
            else
            {
                Deletedmessage = message;
                await message.DeleteAsync();
            }
        }
        else if (message.Channel.Id == Serverstuff._TBoardCommands)
        {
            if (message.Author != GetBotID(message))
            {
                Command DasGroßeDing = new Command(message.Content,message);
                CommandManager.ExecuteCommand(DasGroßeDing);
            }
        }
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
MinecraftServer _server = null;
    void SearchDiscordCommand(SocketMessage command)
    {
        string commandString = command.Content.Remove(0, 1);
    }

    private void AddReactionAsync(SocketMessage message)
    {
        // name überschneidet sich mit methode die mit punkt aufgerufen wird (ja hab den begriff vergessen)
        message.AddReactionAsync(new Emoji("🐟"));
        AddUserPoint(message.Author);
    }

    private void AddUserPoint(SocketUser user)
    {
        Dictionary<ulong, object[]>?
            map = ReadScoreboard(); //Mir gefällt nicht, wie wir das Scoreboard lesen müssen, damit wir in das Scoreboard Schreiben können
        WriteScoreboard(map, user);
    }

    

    private void CreateScoreboardFile()
    {
        if (!File.Exists(scorefilepath)) File.Create(scorefilepath);
    }

    private Dictionary<ulong, object[]>? ReadScoreboard()
    {
        CreateScoreboardFile();
        Dictionary<ulong, object[]> map = new();
        FileStream fs = new(scorefilepath, FileMode.Open, FileAccess.Read);
        try
        {
            map = JsonSerializer.Deserialize<Dictionary<ulong, object[]>>(fs) ?? new();
            fs.Close();
            return map;
        }
        catch
        {
            fs.Close();
            return new();
        }
    }

    private void PrintAlphabetStats(Dictionary<ulong, object[]> map, ISocketMessageChannel channel)
    {
        var embed = new EmbedBuilder
        {
            Title = "Leute die das Alphabet ned können",
            Color = Color.Blue
        };

        foreach (var kvp in map.OrderBy(k => Convert.ToUInt16(((JsonElement)k.Value[1]).GetUInt16())))
        {
            embed.AddField(kvp.Value[0].ToString(), kvp.Value[1]);
        }

        //IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
        //IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == _ThreadAlphabetBack) as IThreadChannel;
        DisplayStuffInDC(embed, channel);
    }

    private async void DisplayStuffInDC(EmbedBuilder embed, ISocketMessageChannel channel)
    {
        await channel.SendMessageAsync(embed: embed.Build());
    }

    private async void DisplayStuffInDC(string text, ITextChannel channel)
    {
        await channel.SendMessageAsync(text);
    }

    //    private void WriteScoreboard<T>(T type)
    void WriteScoreboard(Dictionary<ulong, object[]> map, SocketUser user)
    {
        map ??= []; //wenn file korrupt oder (noch) leer
        if (!map.ContainsKey(user.Id)) map.Add(user.Id, [user.Username, 0]);
        map[user.Id] = [user.Username, Convert.ToInt64(map[user.Id][1].ToString()) + 1];

        FileStream fs = new(scorefilepath, FileMode.Open, FileAccess.Write);
        JsonSerializer.SerializeAsync(fs, map); //Sync oder Async
        fs.Close();
    }

    private void CheckIfChannelExists(ITextChannel channel)
    {
        if (channel == null)
        {
            throw new Exception("Channel not found.");
        }
    }

    private void CheckIfChannelExists(IVoiceChannel channel)
    {
        if (channel == null)
        {
            throw new Exception("Channel not found.");
        }
    }

    private async void RemoveFishReactionAsync(SocketMessage message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"),
            GetBotID(message)); // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }

    private async Task SendRandomMessagesAsync(CancellationToken cancellationToken)
    {
        ITextChannel? channel = _client.GetChannel(_ThreadAlphabetBack) as ITextChannel;
        CheckIfChannelExists(channel);
        while (!cancellationToken.IsCancellationRequested)
        {
            string[] lines = await File.ReadAllLinesAsync(_pathCommands);

            // Select a random line from the filtered lines
            string selectedLine = lines[_random.Next(lines.Length)];

            if (_random.Next(4) != 0)
            {
                selectedLine = ""; // "".Split()
            }

            if (selectedLine == "ShowTotalMessageCount")
            {
                IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
                IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == _ThreadAlphabetBack) as IThreadChannel;
                var userMessageCount = new Dictionary<ulong, int>();
                var messages = await thread.GetMessagesAsync(limit: 10000).FlattenAsync();
                DisplayStuffInDC(
                    GetNextPrint() + " " +
                    $"In diesem Thread wurden bis jetzt **{messages.Count()}** Nachrichten geschrieben", channel);
            }
            else if (selectedLine == "ShowEachTotalMessageCount")
            {
                IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
                IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == _ThreadAlphabetBack) as IThreadChannel;
                var userMessageCount = new Dictionary<ulong, int>();
                var messages = await thread.GetMessagesAsync(limit: 10000).FlattenAsync();
                foreach (var message in messages)
                {
                    if (userMessageCount.ContainsKey(message.Author.Id))
                    {
                        userMessageCount[message.Author.Id]++;
                    }
                    else
                    {
                        userMessageCount[message.Author.Id] = 1;
                    }
                }

                StringBuilder result = new StringBuilder();

                foreach (var kvp in userMessageCount.OrderByDescending(x => x.Value))
                {
                    var user = await _client.GetUserAsync(kvp.Key);
                    result.AppendLine($"{user.Username}: {kvp.Value} messages");
                }

                // Convert StringBuilder to string
                string finalResult = result.ToString();
                DisplayStuffInDC(GetNextPrint() + "\n" + finalResult, channel);
            }
            else if (selectedLine == "SendOrgans")
            {
                await channel.SendFileAsync("Program.cs", GetNextPrint() + " Hier ist mein Sourcecode:");
            }
            else
            {
                await channel.SendMessageAsync(GetNextPrint() + " " + selectedLine.Trim());
            }

            int delay = _random.Next(3600 * 12, 3600 * 24);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken); // Wait for the random delay
        }
    }

    private string GetNextPrint()
    {
        return GetNextCombination(_LastUserMessage == null ? _LastUserMessageFallback.Value :
            CheckFormat(_LastUserMessage) ? GetCombination(_LastUserMessage) : _LastUserMessageFallback.Value);
    }

    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage message,
        ISocketMessageChannel channel)
    {
        if (message.Channel is SocketThreadChannel threadChannel && threadChannel.Id == _ThreadAlphabetBack)
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
                            AddReactionAsync(message);
                        }
                    }
                }
            }
        }
    }

    private static SocketGuildUser GetBotID(SocketMessage message) // is diese methode so schlau?
    {
        // Get the guild (server) where the message was sent
        if (message.Channel is SocketGuildChannel guildChannel)
        {
            // Get the guild user from the message author
            return guildChannel.Guild.CurrentUser as SocketGuildUser;
        }

        return null; // Return null if the message is not in a guild
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

    private async Task GetBotUpToDate()
    {
        byte MessageLimit = 30;

        SocketThreadChannel thread = (SocketThreadChannel)_client.GetChannel(_ThreadAlphabetBack);
        IEnumerable<IMessage> messages = await thread.GetMessagesAsync(limit: MessageLimit).FlattenAsync();

        Streak CurrentStreak = new();
        Streak TopStreak = new();

        foreach (IMessage msg in messages.Reverse())
        {
            try
            {
                if (!CheckFormat(msg))
                {
                    AddReactionAsync((SocketMessage)msg);
                    continue;
                }
            }
            catch (InvalidCastException)
            {
                continue;
            }

            if (Streak.currentIndex == 0)
            {
                CurrentStreak.currentCombination = GetCombination(msg);
            }
            else if (CurrentStreak.currentCombination != GetLastCombination(msg))
            {
                CurrentStreak.streak = 0;
                CurrentStreak.currentCombination = GetCombination(msg);
            }
            else if (CurrentStreak.currentCombination == GetLastCombination(msg))
            {
                CurrentStreak.currentCombination = GetCombination(msg);
            }

            CurrentStreak.streak++;
            Streak.currentIndex++;
            if (CurrentStreak.streak > TopStreak.streak)
                TopStreak = new(CurrentStreak.streak, CurrentStreak.currentCombination);
        }

        int getIndexLastTopStreak = messages.ToList()
            .FindIndex(x => CheckFormat(x) && GetCombination(x) == TopStreak.currentCombination);
        string s = TopStreak.currentCombination;
        for (int i = 0; getIndexLastTopStreak != i; i++)
        {
            s = GetNextCombination(s);
        }

        _LastUserMessageFallback = new(messages.First().Author, s);
        Console.WriteLine(getIndexLastTopStreak);
        Console.WriteLine(s);
    }

    private async void GetExactCurrentCombination()
    {
        ITextChannel? channel = _client.GetChannel(_TBoardGeneral) as ITextChannel;
        if (channel == null)
        {
            throw new Exception("Channel not found.");
        }

        IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
        IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == _ThreadAlphabetBack) as IThreadChannel;
        if (thread == null)
        {
            throw new Exception("Thread not found.");
        }

        var messages = await thread.GetMessagesAsync(limit: 10000).FlattenAsync();
        ushort messageCount = (ushort)messages.Count();
        ushort counter = 0;
        Console.WriteLine(messageCount);
        for (char first = 'Z'; first >= 'A'; first--)
        for (char second = 'Z'; second >= 'A'; second--)
        for (char third = 'Z'; third >= 'A'; third--)
        {
            counter++;
            if (counter == messageCount) Console.WriteLine($"{first}{second}{third}");
        }

        ;
    }

    private static string GetLastCombination(IMessage message)
    {
        char[] chars = GetCombination(message).ToCharArray();

        for (int i = 2; i >= 0; i--)
        {
            if (chars[i] != 'Z')
            {
                chars[i] = (char)(chars[i] + 1);
                return new string(chars);
            }
            else chars[i] = 'A';
        }

        return new string(chars);
    }

    private static string GetNextCombination(string message)
    {
        char[] chars = message.ToCharArray();

        for (int i = 2; i >= 0; i--)
        {
            if (chars[i] != 'A')
            {
                chars[i] = (char)(chars[i] - 1);
                return new string(chars);
            }
            else chars[i] = 'Z';
        }

        return new string(chars);
    }

    private static bool CheckFormat(IMessage message)
    {
        if (message.Content.Length < 3) return false;
        return GetCombination(message) == GetCombination(message).ToUpper();
    }

    private static string GetCombination(IMessage message) => message.Content.Substring(0, 3);

    private async Task UpdateStatusAsync(CancellationToken cancellationToken)
    {
        byte i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var currentHour = DateTime.Now.Hour;

            // Set status based on the time of day
            if (_random.Next(2) == 1)
            {
                if (currentHour >= 8 && currentHour <= 22)
                {
                    await _client.SetStatusAsync(UserStatus.Online);
                }
                else
                {
                    await _client.SetStatusAsync(UserStatus.AFK);
                }
            }

            if (currentHour <= 4)
            {
                if (i == 0) continue;
                i = 0;
            }
            else if (currentHour <= 10)
            {
                if (i == 1) continue;
                i = 1;
            }
            else if (currentHour <= 14)
            {
                if (i == 2) continue;
                i = 2;
            }
            else if (currentHour <= 18)
            {
                if (i == 3) continue;
                i = 3;
            }
            else if (currentHour <= 23)
            {
                if (i == 4) continue;
                i = 4;
            }
            else
            {
                if (i == 5) continue;
                i = 5;
            }

            // Read messages from the text file
            if (!File.Exists(_pathActivities))
                File.Create(_pathActivities);
            string[] lines = await File.ReadAllLinesAsync(_pathActivities);
            var filteredLines = lines.Where(x => x.Split("@")[0] == $"{i}").ToArray();

            if (filteredLines.Length > 0)
            {
                // Select a random line from the filtered lines
                string selectedLine = filteredLines[_random.Next(filteredLines.Length)];
                lines = selectedLine.Split("@");

                // Optionally replace lines with an empty array based on a random condition
                if (_random.Next(3) != 0)
                {
                    lines = []; // or "".Split() if you prefer
                }
            }
            else
            {
                lines = [];
            }

            if (lines[1] == " Denkt über Tee nach" && TeaThinkCounter != 6)
            {
                lines[1] = " Denkt über Tee nach";
            }
            else if (TeaThinkCounter == 6) TeaThinkCounter = 0;

            if (lines.Length == 3)
            {
                await _client.SetGameAsync(lines[2].Trim());
            }
            else
            {
                await _client.SetCustomStatusAsync(lines[1].Trim());
            }

            // Und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte
            await Task.Delay(TimeSpan.FromMinutes(60), cancellationToken);
        }
    }

    internal class Streak // COMBOOOO!!!
    {
        public static byte currentIndex = 0;
        public byte streak = 0;
        public string currentCombination = "";

        public Streak()
        {
        }

        public Streak(byte Streak, string CurrentCombination)
        {
            streak = Streak;
            currentCombination = CurrentCombination;
        }
    }
}