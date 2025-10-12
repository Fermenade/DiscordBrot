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
        public void MessageReceived(IMessage msg)
        {
            AlphabetMessage<Combination,char> message = new(msg);
            FailureCase failure = CachedMessages.Add(message);
            
            switch (failure)
            {
                case FailureCase.DuplicateAuthor:
                    break;
                case FailureCase.WrongCombination:
                    break;
                case FailureCase.NotCombination:
                    break;
                case FailureCase.None:
                    break;
            }
        }

        public void MessageUpdated(Cacheable<IMessage, ulong> preMsg, IMessage curMsg, ISocketMessageChannel channel)
        {
            AlphabetMessage<Combination, char> previousMessage = new(preMsg.Value);
            AlphabetMessage<Combination, char> currentMessage = new(curMsg);

            FailureCase failure = CachedMessages.Update(previousMessage, currentMessage);

            switch (failure)
            {
                case FailureCase.WrongCombination:
                    break;
                case FailureCase.NotCombination:
                    break;
                case FailureCase.None:
                    break;
            }
        }

        public void MessageDeleted(Cacheable<IMessage, ulong> msg, Cacheable<IMessageChannel, ulong> channel)
        {
            AlphabetMessage<Combination, char> deletedMessage = new(msg.Value);
            FailureCase failure = CachedMessages.Delete(deletedMessage);

        }

        public ulong Id { get; }
        public DateTimeOffset CreatedAt { get; }


    }
}
