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
        SendMessage();
    }

    private async void SendMessage()
    {
        var x = _orderCachedMessages.Collection.Last();
        if (x.message.Author.Id != Program._client.CurrentUser.Id)
        {
            await _channel.SendMessageAsync(x.actuallCombination.GetCombo(-1).ToString());
        }
    }

    private async void LoadMessagesFromChannel(ulong channelId)
    {
        _channel = (IMessageChannel)Program._client.GetChannel(channelId);
        var messages = await _channel.GetMessagesAsync(30).FlattenAsync();
        
        List<AlphabetMessage<Combination, char>> alphabetMessages = new();
        foreach (var message in messages)
            alphabetMessages.Add(new AlphabetMessage<Combination, char>(message));
        _orderCachedMessages = new OrderCachedMessages<Combination, char>(alphabetMessages);
    }

    public Task MessageReceived(IMessage msg)
    {
        AlphabetMessage<Combination, char> message = new(msg);
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
        await message.RemoveReactionAsync(new Emoji("🐟"),
            ((SocketGuildChannel)message.Channel).Guild.CurrentUser);
        // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }
}