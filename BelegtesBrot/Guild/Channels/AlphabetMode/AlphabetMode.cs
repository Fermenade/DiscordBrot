using System.Diagnostics;
using BelegtesBrot.Channels.Alphabet;
using BelegtesBrot.Channels.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels;

[ModeName("alphabet", "Count from ZZZ to AAA.")]
internal class AlphabetMode : IBaseCom
{
    private OrderCachedMessages<Combination, char> _orderCachedMessages;
    private IMessageChannel _channel;
    public AlphabetMode(ulong channelId)
    {
        LoadMessagesFromChannel(channelId);
    }
    private async void LoadMessagesFromChannel(ulong channelId)
    {
        _channel = (IMessageChannel)Program._client.GetChannel(channelId);
        Logger.LogMessage($"Alphabet mode of channel '{_channel.Name}' initializing");
        
        var messages = (await _channel.GetMessagesAsync(30).FlattenAsync()).ToArray();
        List<AlphabetMessage<Combination, char>> alphabetMessages = new();
        for (int i = messages.Length - 1; i >= 0; i--)
        {
            alphabetMessages.Add(new AlphabetMessage<Combination, char>(messages[i]));
        }
        
        _orderCachedMessages = new OrderCachedMessages<Combination, char>(alphabetMessages);
        
        if(_orderCachedMessages.Count == 0) await _channel.SendMessageAsync("ZZZ");
        
        var x = _orderCachedMessages.Collection.Last();
        if (x.message.Author.Id != Program._client.CurrentUser.Id)
        {
            await _channel.SendMessageAsync(x.actuallCombination.GetCombo(-1).ToString());
        }
    }

    public Task MessageReceived(IMessage msg)
    {
        AlphabetMessage<Combination, char> message = new(msg);
        if (msg.Content.StartsWith("???"))
        {
            if (_orderCachedMessages.Collection.Last().message.Author.Id != Program._client.CurrentUser.Id)
            {
                _channel.SendMessageAsync(_orderCachedMessages.Collection.Last().actuallCombination.GetCombo(-1).ToString());
            }
            message.DeleteAsync();
        }
        else
        {
            var failure = _orderCachedMessages.Add(message);
            switch (failure)
            {
                case FailureCase.DuplicateAuthor or FailureCase.NotCombination:
                    message.DeleteAsync();
                    break;
                case FailureCase.WrongCombination:
                    AddFishReactionToMessage(message);
                    break;
                case FailureCase.None:
                    break;
            }
        }
        return Task.CompletedTask;
    }


    public Task MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, IMessageChannel channel)
    {
        
        AlphabetMessage<Combination, char> currentMessage = new(curMsg);

        var failure = _orderCachedMessages.Update(preMsg.Id, currentMessage);
        switch (failure)
        {
            case FailureCase.WrongCombination or FailureCase.NotCombination:
                AddFishReactionToMessage(currentMessage);
                break;
            case FailureCase.None:
                RemoveFishReactionAsync(currentMessage);
                break;
        }

        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
    {
        var failure = _orderCachedMessages.Delete(msg.Id);
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        throw new NotImplementedException();
    }

    private void AddFishReactionToMessage(AlphabetMessage<Combination, char> message)
    {
        message.AddReactionAsync(new Emoji("🐟"));
    }

    private async void RemoveFishReactionAsync(AlphabetMessage<Combination, char> message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"),Program._client.CurrentUser);
        // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }
}