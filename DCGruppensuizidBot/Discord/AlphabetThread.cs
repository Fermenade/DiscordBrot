using System.Text;
using System.Text.Json;
using Discord;
using Discord.WebSocket;

namespace DGruppensuizidBot.Discord;

public class AlphabetThread :Message
{
    private SocketUserMessage _LastUserMessage;

    public AlphabetThread()
    {
        _client.MessageReceived += ClientOnMessageReceived;
        _client.MessageUpdated += ClientOnMessageUpdated;
    }

    private Task ClientOnMessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
        throw new NotImplementedException();
        //TODO: wenn eine nachricht korrigiert wird, und im selben moment eine neue nachricht gesendet wird, dann wird die überprüfung abgebrochen für die andere.
    }

    private KeyValuePair<IUser, string> _lastUserMessageFallback;
    private SocketMessage Deletedmessage;
    private Task ClientOnMessageReceived(SocketMessage message)
    {
        if (message.Channel is SocketThreadChannel threadChannel && threadChannel.Id == Serverstuff._ThreadAlphabetBack)
        {//TODO: Weiterleitungen werden nicht zugelassen und die Weiterleitung wird gelöscht.
            if (CheckFormat(message))
            {
                if (_LastUserMessage != null)
                {
                    if (message.Author != _LastUserMessage.Author)
                    {
                        if (GetCombination(message) != GetNextCombination(GetCombination(_LastUserMessage)))
                        {
                            AddFishReactionAsync(message);
                            _lastUserMessageFallback = new KeyValuePair<IUser, string>(message.Author,
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
                    if (_lastUserMessageFallback.Key.Id != message.Author.Id)
                    {
                        if (GetCombination(message) != GetNextCombination(_lastUserMessageFallback.Value))
                        {
                            AddFishReactionAsync(message);
                            Console.WriteLine(GetCombination(message));
                            Console.WriteLine(GetNextCombination(_lastUserMessageFallback.Value));
                            _lastUserMessageFallback = new KeyValuePair<IUser, string>(message.Author,
                                GetNextCombination(_lastUserMessageFallback.Value));
                            _LastUserMessage = null;
                        }
                        else
                        {
                            _LastUserMessage = (SocketUserMessage)message;
                        }
                    }
                    else
                    {
                        DeleteMessage(message);
                    }
                }
            }
            else
            {
                DeleteMessage(message);
            }
        }
    }

    private async void DeleteMessage(SocketMessage message)
    {
        Deletedmessage = message;
        await message.DeleteAsync();
    }
    private void AddFishReactionAsync(SocketMessage message)
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

    private Dictionary<ulong, object[]>? ReadScoreboard()
    {
        CreateScoreboardFile();
        Dictionary<ulong, object[]> map = new();
        FileStream fs = new(Serverstuff.Scorefilepath, FileMode.Open, FileAccess.Read);
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

    //    private void WriteScoreboard<T>(T type)
    void WriteScoreboard(Dictionary<ulong, object[]> map, SocketUser user)
    {
        map ??= []; //wenn file korrupt oder (noch) leer
        if (!map.ContainsKey(user.Id)) map.Add(user.Id, [user.Username, 0]);
        map[user.Id] = [user.Username, Convert.ToInt64(map[user.Id][1].ToString()) + 1];

        FileStream fs = new(Serverstuff.Scorefilepath, FileMode.Open, FileAccess.Write);
        JsonSerializer.SerializeAsync(fs, map); //Sync oder Async
        fs.Close();
    }

    private bool CheckIfChannelExists(IChannel? channel)
    {
        return channel != null;
    }

    private bool CheckIfVoiceChannelExists(IAudioChannel? channel)
    {
        return channel != null;
    }

    private async void RemoveFishReactionAsync(SocketMessage message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"),
            GetBotID(message)); // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }

    private async Task SendRandomMessagesAsync(CancellationToken cancellationToken)
    {
        ITextChannel? channel = _client.GetChannel(Serverstuff._ThreadAlphabetBack) as ITextChannel;
        
        if(!CheckIfChannelExists(channel))return;
        while (!cancellationToken.IsCancellationRequested)
        {
            string[] lines = await File.ReadAllLinesAsync(Serverstuff.PathCommands);

            // Select a random line from the filtered lines
            string selectedLine = lines[_random.Next(lines.Length)];

            if (_random.Next(4) != 0)
            {
                selectedLine = ""; // "".Split()
            }

            if (selectedLine == "ShowTotalMessageCount")
            {
                IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
                IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == Serverstuff._ThreadAlphabetBack) as IThreadChannel;
                var userMessageCount = new Dictionary<ulong, int>();
                var messages = await thread.GetMessagesAsync(limit: 10000).FlattenAsync();
                DisplayStuffInDC(
                    GetNextPrint() + " " +
                    $"In diesem Thread wurden bis jetzt **{messages.Count()}** Nachrichten geschrieben", channel);
            }
            else if (selectedLine == "ShowEachTotalMessageCount")
            {
                IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
                IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == Serverstuff._ThreadAlphabetBack) as IThreadChannel;
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
        return GetNextCombination(_LastUserMessage == null ? _lastUserMessageFallback.Value :
            CheckFormat(_LastUserMessage) ? GetCombination(_LastUserMessage) : _lastUserMessageFallback.Value);
    }
    private async Task GetBotUpToDate()
    {
        byte MessageLimit = 30;

        SocketThreadChannel thread = (SocketThreadChannel)_client.GetChannel(Serverstuff._ThreadAlphabetBack);
        IEnumerable<IMessage> messages = await thread.GetMessagesAsync(limit: MessageLimit).FlattenAsync();

        Streak CurrentStreak = new();
        Streak TopStreak = new();

        foreach (IMessage msg in messages.Reverse())
        {
            try
            {
                if (!CheckFormat(msg))
                {
                    AddFishReactionAsync((SocketMessage)msg);
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

        _lastUserMessageFallback = new(messages.First().Author, s);
        Console.WriteLine(getIndexLastTopStreak);
        Console.WriteLine(s);
    }

    private async void GetExactCurrentCombination()
    {
        ITextChannel? channel = _client.GetChannel(Serverstuff._TBoardGeneral) as ITextChannel;
        if (channel == null)
        {
            throw new Exception("Channel not found.");
        }

        IReadOnlyCollection<IThreadChannel> threads = await channel.GetActiveThreadsAsync();
        IThreadChannel? thread = threads.FirstOrDefault(t => t.Id == Serverstuff._ThreadAlphabetBack) as IThreadChannel;
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

    internal class UserMessage
    {
        private IUser user;
        private string UserCombination;
        
    }
}