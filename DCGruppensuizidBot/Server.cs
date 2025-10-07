using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DGruppensuizidBot.AlphabetThread;

namespace DGruppensuizidBot
{
    internal class Server
    {
        public IGuild guid;
        private List<IServerMessageChannel> messageChannels;
        public Server()
        {
            messageChannels = getMessageChannels();
        }

        public void MessageReceived(IMessage message)
        {
            foreach (var VARIABLE in messageChannels)
            {
                if (VARIABLE.Id == message.Channel.Id)
                {
                    VARIABLE.MessageReceived(message);
                }
            }
        }
        public void MessageUpdated(IMessage previousMessage, IMessage currentMessage,ISocketMessageChannel channel)
        {
            foreach (var VARIABLE in messageChannels)
            {
                if (VARIABLE.Id == channel.Id)
                {
                    VARIABLE.MessageUpdated(previousMessage, currentMessage, channel);
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
