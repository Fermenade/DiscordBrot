using System.Security.Cryptography.X509Certificates;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot
{
    internal class DiscordClient
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
        }

        Task MessageReceived(IMessage message)
        {
            if (_servers.Count != _client.Guilds.Count)
            {
                // Ensure _servers only contains servers that exist in _client.Guilds
                // 1. Add missing guilds
                foreach (var guild in _client.Guilds)
                {
                    if (_servers.All(s => s.Guild.Id != guild.Id))
                    {
                        _servers.Add(new Server(guild));
                    }
                }

                // 2. Remove servers whose guilds are no longer present
                _servers.RemoveAll(s => _client.Guilds.All(g => g.Id != s.Guild.Id));
            }

            if (message.Channel is SocketGuildChannel channel)
            {
                foreach (var socketGuild in _servers)
                {
                    if (socketGuild.Guild.Id == channel.Guild.Id)
                    {
                        socketGuild.MessageReceived(message);
                    }
                }
            }
        }
    }
}
