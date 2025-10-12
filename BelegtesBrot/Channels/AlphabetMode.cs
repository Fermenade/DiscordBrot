using BelegtesBrot.Channels.Alphabet;
using BelegtesBrot.Channels.Cache;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels
{
    internal class AlphabetMode : IServerMessageChannel
    {
        public ChannelType ChannelType { get; }
        public string Name { get; }
        public IGuildChannel Channel { get; }

        private AlphabetCachedMessages<Combination, char> CachedMessages =
            new AlphabetCachedMessages<Combination, char>();
        public Task MessageReceived(IMessage msg)
        {
            AlphabetMessage<Combination,char> message = new(msg);
            FailureCase failure = CachedMessages.Add(message);
            
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


        public Task MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, ISocketMessageChannel channel)
        {
            AlphabetMessage<Combination, char> previousMessage = new(preMsg.Value);
            AlphabetMessage<Combination, char> currentMessage = new(curMsg);
            
            FailureCase failure = CachedMessages.Update(previousMessage, currentMessage);

            switch (failure)
            {
                case FailureCase.WrongCombination or FailureCase.NotCombination:
                    AddFishReactionToMessage(currentMessage);
                    break;
                case FailureCase.None:
                    RemoveFishReactionAsync(currentMessage);
                    break;
            }
        }
        public Task MessageDeleted(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
        {
            AlphabetMessage<Combination, char> deletedMessage = new(msg.Value);
            FailureCase failure = CachedMessages.Delete(deletedMessage);

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
