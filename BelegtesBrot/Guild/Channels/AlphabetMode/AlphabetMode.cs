using BelegtesBrot.Channels;
using BelegtesBrot.Channels.Alphabet;
using BelegtesBrot.Channels.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Guild.Channels.AlphabetMode;

[ModeName("alphabet", "Count from ZZZ to AAA.")]
internal class AlphabetMode : IBaseCom
{
    private IMessageChannel _channel;
    private OrderCachedMessages<Combination, char> _orderCachedMessages;

    public AlphabetMode(ulong channelId)
    {
        InitFromChannel(channelId);
    }

    public Task MessageReceived(IMessage msg)
    {
        AlphabetLogMessage.LogMessage(msg,"New");
        AlphabetMessage<Combination, char> message = new(msg);
        
        switch (_orderCachedMessages.Add(message))
        {
            case FailureCase.DuplicateAuthor:
                AlphabetLogMessage.LogMessage(msg,$"{message.Author} was same as prev {_orderCachedMessages.Collection[^2]}");
                message.DeleteAsync();
                break;
            case FailureCase.NotCombination:
                AlphabetLogMessage.LogMessage(msg,"NaC");
                message.DeleteAsync();
                break;
            case FailureCase.WrongCombination:
                AlphabetLogMessage.LogMessage(msg, $"Wrong com -> {_orderCachedMessages.Collection.Last().actuallCombination.GetCombo(-1)}");
                AddFishReactionToMessage(message);
                break;
            case FailureCase.None:
                AlphabetLogMessage.LogMessage(msg, "No Failure");
                break;
        }

        return Task.CompletedTask;
    }


    public Task MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, IMessageChannel channel)
    {
        AlphabetLogMessage.LogMessage(curMsg,"Updated");
        AlphabetMessage<Combination, char> currentMessage = new(curMsg);

        var failure = _orderCachedMessages.Update(preMsg.Id, currentMessage);
        switch (failure)
        {
            case FailureCase.WrongCombination:
                AlphabetLogMessage.LogMessage(curMsg, $"Wrong com -> {_orderCachedMessages.Collection.First(x=>x.message.Id == curMsg.Id).actuallCombination}");
                AddFishReactionToMessage(currentMessage);
                break;
            case  FailureCase.NotCombination:
                AlphabetLogMessage.LogMessage(curMsg,"NaC");
                AddFishReactionToMessage(currentMessage);
                break;
            case FailureCase.None:
                AlphabetLogMessage.LogMessage(curMsg, "No Failure");
                RemoveFishReactionAsync(currentMessage);
                break;
        }

        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
    {
        AlphabetLogMessage.LogMessage(msg.Id,"deleted");
        _orderCachedMessages.Delete(msg.Id);
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        throw new NotImplementedException();
    }

    private async void InitFromChannel(ulong channelId)
    {
        _channel = (IMessageChannel)Program._client.GetChannel(channelId);
        Logger.LogMessage($"Alphabet mode of channel '{_channel.Name}' initializing");

        var messages = (await _channel.GetMessagesAsync(30).FlattenAsync()).ToArray();
        List<AlphabetMessage<Combination, char>> alphabetMessages = new();
        for (var i = messages.Length - 1; i >= 0; i--)
            alphabetMessages.Add(new AlphabetMessage<Combination, char>(messages[i]));

        _orderCachedMessages = new OrderCachedMessages<Combination, char>(alphabetMessages);

        if (_orderCachedMessages.Count == 0) await _channel.SendMessageAsync("ZZZ");

        var x = _orderCachedMessages.Collection.Last();
        if (x.message.Author.Id != Program._client.CurrentUser.Id)
            await _channel.SendMessageAsync(x.actuallCombination.GetCombo(-1).ToString());
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