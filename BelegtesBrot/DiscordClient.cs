using System.Security.Cryptography.X509Certificates;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot
{
    internal class DiscordClient : IBaseCom
    {
        private DiscordSocketClient _client;
        private List<IServer> _servers;

        public DiscordClient(DiscordSocketClient client)
        {
            _client = client;
        }

        void _Ready()
        {
            _client.MessageReceived += MessageReceived;
            _client.MessageUpdated += MessageUpdated;
        }
        public Task MessageReceived(IMessage message)
        {
            CheckServerListUpToDate();
            if (message.Channel is SocketGuildChannel channel)
            {
                foreach (IServer socketGuild in _servers)
                {
                    if (socketGuild.Guild.Id == channel.Guild.Id)
                    {
                       return socketGuild.MessageReceived(message);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage, ISocketMessageChannel channel)
        {
            CheckServerListUpToDate();
            if (channel is SocketGuildChannel chan)
            {
                foreach (IServer socketGuild in _servers)
                {
                    if (socketGuild.Guild.Id == chan.Guild.Id)
                    {
                        return socketGuild.MessageUpdated(previousMessage, currentMessage, channel);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
        {
            CheckServerListUpToDate();
            if (channel.Value is SocketGuildChannel chan)
            {
                foreach (IServer socketGuild in _servers)
                {
                    if (socketGuild.Guild.Id == chan.Guild.Id)
                    {
                        return socketGuild.MessageDeleted(message, channel);
                    }
                }
            }

            return Task.CompletedTask;
        }

        void CheckServerListUpToDate()
        {
            if (_servers.Count != _client.Guilds.Count)
            {
                // Ensure _servers only contains servers that exist in _client.Guilds
                // 1. Add missing guilds
                foreach (SocketGuild? guild in _client.Guilds)
                {
                    if (_servers.All(s => s.Guild.Id != guild.Id))
                    {
                        _servers.Add(new Server(guild));
                    }
                }

                // 2. Remove servers whose guilds are no longer present
                _servers.RemoveAll(s => _client.Guilds.All(g => g.Id != s.Guild.Id));
            }
        }
    }
}
