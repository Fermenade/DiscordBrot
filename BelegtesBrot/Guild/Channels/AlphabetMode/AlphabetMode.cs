using BelegtesBrot.Guild.Channels.AlphabetMode.Alphabet;
using BelegtesBrot.Guild.Channels.AlphabetMode.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Guild.Channels.AlphabetMode;

[ModeName("alphabet", "Count from ZZZ to AAA.")]
internal class AlphabetMode : IBaseCom
{
    private IMessageChannel _channel;
    private OrderCachedMessages<Combination, char> _orderCachedMessages;
    private AlphabetLogMessage _alphabetLogMessage;

    public AlphabetMode(ulong channelId, Session session)
    {
        _channel = (IMessageChannel)Program._client.GetChannel(channelId);
        _alphabetLogMessage = new AlphabetLogMessage(session.Logger,_channel!);
        _alphabetLogMessage.LogMessage("Initializing...");
        InitFromChannel(channelId);
        _alphabetLogMessage.LogMessage("Initialized done.");
    }
    bool isFirst = true;
    public Task MessageReceived(IMessage msg)
    {
        _alphabetLogMessage.LogMessage(msg,"New message");
        AlphabetMessage<Combination, char> message = new(msg);
        _alphabetLogMessage.LogMessage(msg, $"{(message.Content.Length >= 10 ? message.Content.Substring(0, 10) : message.Content)} - {(message.Combination == null ? "???" : message.Combination.ToString())}");

        if (isFirst)
        {
            if (_orderCachedMessages.GetLastestEntry().message.Id == msg.Id)
            {
                _orderCachedMessages.Delete(msg.Id);
            }
        }
        
        switch (_orderCachedMessages.Add(message))
        {
            case FailureCase.DuplicateAuthor:
                _alphabetLogMessage.LogMessage(msg,$"{message.Author} was same as prev {_orderCachedMessages.Collection[^2].message.Author}");
                message.DeleteAsync();
                break;
            case FailureCase.NotCombination:
                _alphabetLogMessage.LogMessage(msg,"NaC");
                message.DeleteAsync();
                break;
            case FailureCase.WrongCombination:
                _alphabetLogMessage.LogMessage(msg, $"Wrong com -> {_orderCachedMessages.Collection.Last().actuallCombination.GetCombo(-1)}");
                AddFishReactionToMessage(message);
                break;
            case FailureCase.None:
                _alphabetLogMessage.LogMessage(msg, "No Failure");
                break;
        }

        isFirst = false;
        return Task.CompletedTask;
    }


    public Task MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, IMessageChannel channel)
    {
        _alphabetLogMessage.LogMessage(curMsg,"Updated message");
        AlphabetMessage<Combination, char> currentMessage = new(curMsg);
        _alphabetLogMessage.LogMessage(curMsg, $"{(currentMessage.Content.Length >= 10 ? currentMessage.Content.Substring(0, 10) : curMsg.Content)} - {(currentMessage.Combination == null ? "???" : currentMessage.Combination.ToString())}");
        
        var failure = _orderCachedMessages.Update(preMsg.Id, currentMessage);
        switch (failure)
        {
            case FailureCase.WrongCombination:
                _alphabetLogMessage.LogMessage(curMsg, $"Wrong com -> {_orderCachedMessages.Collection.First(x=>x.message.Id == curMsg.Id).actuallCombination}");
                AddFishReactionToMessage(currentMessage);
                break;
            case  FailureCase.NotCombination:
                _alphabetLogMessage.LogMessage(curMsg,"NaC");
                AddFishReactionToMessage(currentMessage);
                break;
            case FailureCase.None:
                _alphabetLogMessage.LogMessage(curMsg, "No Failure");
                RemoveFishReactionAsync(currentMessage);
                break;
            case FailureCase.NonExistent:
                break;
        }
        
        isFirst = false;
        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
    {
        _alphabetLogMessage.LogMessage(msg.Id,"Deleted");
        _orderCachedMessages.Delete(msg.Id);
        
        isFirst = false;
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        throw new NotImplementedException();
    }

    private async void InitFromChannel(ulong channelId)
    {
        
        var messages = _channel.GetMessagesAsync(30).FlattenAsync().Result.ToArray();
        List<AlphabetMessage<Combination, char>> alphabetMessages = [];
        for (var i = messages.Length - 1; i >= 0; i--)
            alphabetMessages.Add(new AlphabetMessage<Combination, char>(messages[i]));

        _orderCachedMessages = new OrderCachedMessages<Combination, char>(alphabetMessages);

        if (_orderCachedMessages.Count == 0) await _channel.SendMessageAsync("ZZZ");
    }

    private void AddFishReactionToMessage(AlphabetMessage<Combination, char> message)
    {
        message.AddReactionAsync(new Emoji("🐟"));
    }

    private async void RemoveFishReactionAsync(AlphabetMessage<Combination, char> message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"), Program._client.CurrentUser);
        // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }
}