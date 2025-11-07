using BelegtesBrot.Channels.Alphabet;
using BelegtesBrot.Channels.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels
{
    [MessageChannelHandler]
    internal class AlphabetMode : IServerMessageChannel
    {
        public string Name { get; }
        public SocketTextChannel Channel => channel;


        private OrderCachedMessages<Combination, char> _orderCachedMessages =
            new OrderCachedMessages<Combination, char>();

        private readonly SocketTextChannel channel;

        public AlphabetMode(SocketTextChannel channel)
        {
            this.channel = channel;
        }

        public Task MessageReceived(IMessage msg)
        {
            AlphabetMessage<Combination, char> message = new(msg);
            FailureCase failure = _orderCachedMessages.Add(message);

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


        public Task MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, ISocketMessageChannel channel)
        {
            AlphabetMessage<Combination, char> previousMessage = new(preMsg.Value);
            AlphabetMessage<Combination, char> currentMessage = new(curMsg);

            FailureCase failure = _orderCachedMessages.Update(previousMessage, currentMessage);

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
            AlphabetMessage<Combination, char> deletedMessage = new(msg.Value);
            FailureCase failure = _orderCachedMessages.Delete(deletedMessage);
            return Task.CompletedTask;
        }
        private void AddFishReactionToMessage(AlphabetMessage<Combination, char> message)
        {
            message.AddReactionAsync(new Emoji("🐟"));
        }
        private async void RemoveFishReactionAsync(AlphabetMessage<Combination, char> message)
        {
            await message.RemoveReactionAsync(new Emoji("🐟"),
                ((SocketGuildChannel)message.Channel).Guild.CurrentUser); // soll der fehler wieder abgezogen werden, wenn der fehler ausgebessert wird?
        }


        public ulong Id { get; }
        public DateTimeOffset CreatedAt { get; }


    }
}
