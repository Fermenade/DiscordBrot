using Discord;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using BelegtesBrot.Channels;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot
{
    internal class Server : IServer
    {
        public IGuild Guild => _guild;

        private readonly IGuild _guild;
        
        public List<IServerMessageChannel> MessageChannels => _messageChannels;
        private readonly List<IServerMessageChannel> _messageChannels;
        
        private ChannelConnecting channelConnecting;
        private CommandSession _commandSession;

        public Server(IGuild guild)
        {
            _guild = guild;
            channelConnecting = new ChannelConnecting($"{_guild}.json");
            _messageChannels = channelConnecting.ReadConnectedChannels() ?? [];
            _commandSession = new CommandSession();
        }

        public Task MessageReceived(IMessage message)
        {
            foreach (IServerMessageChannel messageChannel in MessageChannels)
            {
                if (messageChannel.Channel.Id == message.Channel.Id)
                {
                    return messageChannel.MessageReceived(message);
                }
            }

            return Task.CompletedTask;
        }

        public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            foreach (IServerMessageChannel messageChannel in MessageChannels)
            {
                if (messageChannel.Channel.Id == channel.Id)
                {
                    return messageChannel.MessageUpdated(previousMessage, currentMessage, channel);
                }
            }
            return Task.CompletedTask;
        }
        public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            foreach (IServerMessageChannel messageChannel in MessageChannels)
            {
                if (messageChannel.Channel.Id == channel.Value.Id)
                {
                    return messageChannel.MessageDeleted(message, channel);
                }
            }
            return Task.CompletedTask;
        }
    }
}
