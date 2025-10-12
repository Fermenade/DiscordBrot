using System.Reflection;
using BelegtesBrot.Channels;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot
{
    internal interface IServer : IBaseCom
    {
        public IGuild Guild { get; }
        public List<IServerMessageChannel> MessageChannels { get; }


    }

    internal class Server : IServer
    {
        public IGuild Guild => guild;

        private readonly IGuild guild;
        public List<IServerMessageChannel> MessageChannels => new();

        public Server(IGuild guild)
        {
            this.guild = guild;

        }

        private void ReadConnectedChannels()
        {
            string serverGuildFile = $"{Guild.Id}.json";
            if (!File.Exists(serverGuildFile))return;
            string e = File.ReadAllText(serverGuildFile);
            List<LinkedChannels> x = JsonSerializer.Deserialize<List<LinkedChannels>>(e);

            foreach (LinkedChannels VARIABLE in x)
            {
                Type loadedType = Type.GetType(VARIABLE.Channel, throwOnError: true);

                // b) Using a constructor with arguments
                IServerMessageChannel instance = (IServerMessageChannel)Activator.CreateInstance(
                    loadedType,
                    new {VARIABLE.ChannelId}); // arguments must match a ctor signature
                MessageChannels.Add(instance);
            }

        }

        void AddCannel() //TODO: link this logic with a register command
        {
            LinkedChannels linkedChannels = new LinkedChannels();

            
        }
        public Task MessageReceived(IMessage message)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == message.Channel.Id)
                {
                    return VARIABLE.MessageReceived(message);
                }
            }
            return Task.CompletedTask;
        }
        public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == channel.Id)
                {
                    return VARIABLE.MessageUpdated(previousMessage, currentMessage, channel);
                }
            }
            return Task.CompletedTask;
        }
        public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            foreach (IServerMessageChannel VARIABLE in MessageChannels)
            {
                if (VARIABLE.Channel.Id == channel.Value.Id)
                {
                   return VARIABLE.MessageDeleted(message, channel);
                }
            }
            return Task.CompletedTask;
        }
    }
}
