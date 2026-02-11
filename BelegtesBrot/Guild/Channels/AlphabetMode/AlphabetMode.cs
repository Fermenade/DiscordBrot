using BelegtesBrot.Channels.Alphabet;
using BelegtesBrot.Channels.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels;

[ModeName("alphabet", "Count from ZZZ to AAA.")]
public class AlphabetMode : IBaseCom
{
    private OrderCachedMessages<Combination, char> _orderCachedMessages;

    public Task MessageReceived(IMessage msg)
    {
        Init(msg.Channel);

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
        Init(channel);

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
        Init(channel.Value);

        var failure = _orderCachedMessages.Delete(msg.Id);
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        throw new NotImplementedException();
    }

    private async void Init(IMessageChannel channel)
    {
        if (_orderCachedMessages == null!)
        {
            var x = channel.GetMessagesAsync(30);
            var messages = x.Flatten();

            List<AlphabetMessage<Combination, char>> alphabetMessages = new();
            foreach (var message in messages.ToEnumerable().Reverse())
                alphabetMessages.Add(new AlphabetMessage<Combination, char>(message));
            _orderCachedMessages = new OrderCachedMessages<Combination, char>(alphabetMessages);
            if (_orderCachedMessages.Collection.Last().message.Author.Id != Program._client.CurrentUser.Id)
                await channel.SendMessageAsync(_orderCachedMessages.Collection.Last().actuallCombination.GetCombo(-1)
                    .ToString());
        }
    }

    private void AddFishReactionToMessage(AlphabetMessage<Combination, char> message)
    {
        message.AddReactionAsync(new Emoji("🐟"));
    }

    private async void RemoveFishReactionAsync(AlphabetMessage<Combination, char> message)
    {
        await message.RemoveReactionAsync(new Emoji("🐟"),
            ((SocketGuildChannel)message.Channel).Guild
            .CurrentUser); // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
    }
}