using DGruppensuizidBot.Discord;
using Discord;
using Discord.WebSocket;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace DGruppensuizidBot.AlphabetThread;

public class AlphabetThread : IServerMessageChannel
{
    public IMessageChannel Channel { get; private set; }
    public ulong Id => Channel.Id;
    private SocketUserMessage _LastUserMessage;

    private KeyValuePair<IUser, string> _lastUserMessageFallback;

    private MessageCache messageCache = new MessageCache();

    private SocketMessage Deletedmessage;

    private Logger logger = new LoggerConfiguration()
        .WriteTo.File("Alphabet.log",rollingInterval: RollingInterval.Day)
        .WriteTo.Console()
        .CreateLogger();


    public async Task MessageReceived(SocketMessage sMessage)
    {
        AlphabetMessage message = new(sMessage);
        

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
    public async Task ProcessMessageQueue(Cacheable<IMessageChannel, ulong> channel) 
    {
        while (_messageQueue.TryDequeue(out var cachedMessage))
        {
            if (channel.Id == Serverstuff._ThreadAlphabetBack)
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

    async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage sMessage, ISocketMessageChannel channel)
    {
        AlphabetMessage message = new(sMessage);
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
                            AddFishReactionAsync(message);
                        }
                    }
                }
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
        EmbedBuilder embed = new EmbedBuilder
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

    private async void RemoveFishReactionAsync(SocketMessage message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"), CoreDiscord.GetBotID(message)); // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
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
                foreach (IMessage? message in messages)
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
            else
            {
                await channel.SendMessageAsync(GetNextPrint() + " " + selectedLine.Trim());
            }

            int delay = _random.Next(3600 * 12, 3600 * 24);
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken); // Wait for the random delay
        }
    }
    //if (channel.Id == Serverstuff._ThreadAlphabetBack)
    //{
    //    if (Deletedmessage != null)
    //    {
    //        // Check if the message is cached
    //        // why was  thjis even nessesary
    //        if (cachedMessage.Id != Deletedmessage.Id)
    //        {
    //            GetBotUpToDate();
    //            _LastUserMessage = null;
    //        }
    //    }
    //    else
    //    {
    //        GetBotUpToDate();
    //        //Spongebob
    //        _LastUserMessage = null;
    //    }
    //}
    private string GetNextPrint()
    {
        return GetNextCombination(_LastUserMessage == null ? _lastUserMessageFallback.Value :
            CheckFormat(_LastUserMessage) ? GetCombination(_LastUserMessage) : _lastUserMessageFallback.Value);
    }
    


    private static bool CheckFormat(IMessage message)
    {
        if (message.Content.Length < 3) return false;
        return GetCombination(message) == GetCombination(message).ToUpper();
    }

    public Task MessageReceived(IMessage message)
    {
        throw new NotImplementedException();
    }

    Task IServerMessageChannel.MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage message, ISocketMessageChannel channel)
    {
        return MessageUpdated(before, message, channel);
    }

    public Task MessageDeleted()
    {
        throw new NotImplementedException();
    }
}