using System.Reflection;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot
{
    internal interface IServer : IBaseCom
    {
        public IGuild Guild { get; }
        public List<IServerMessageChannel> MessageChannels { get; }


    }

    internal interface IBaseCom
    {
        public void MessageReceived(IMessage message);
        public void MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel);
        public void MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessage, ulong> message1);
    }
    internal class Server(IGuild guild) : IServer
    {
        public IGuild Guild => guild;
        public List<IServerMessageChannel> MessageChannels => getMessageChannels();
        public void MessageReceived(IMessage message)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == message.Channel.Id)
                {
                    VARIABLE.MessageReceived(message);
                }
            }
        }
        public void MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == channel.Id)
                {
                    VARIABLE.MessageUpdated(previousMessage, currentMessage, channel);
                }
            }
        }

        public void MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessage, ulong> message1)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == message.Channel.Id)
                {
                    VARIABLE.MessageDeleted(message);
                }
            }
        }

        private List<IServerMessageChannel> getMessageChannels()
        {
            return getInterfaceInheritingClasses<IServerMessageChannel>();
        }
        List<T> getInterfaceInheritingClasses<T>()
        {
            List<T> channelList = new List<T>();

            IEnumerable<Type> types = Assembly.GetExecutingAssembly()
                .GetTypes();

            foreach (Type commandType in types)
            {
                if (!typeof(T).IsAssignableFrom(commandType)) continue;

                if (Activator.CreateInstance(commandType) is T commandInstance)
                {
                    channelList.Add(commandInstance);
                }
            }
        }
    }
}
