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
            _client.MessageDeleted += MessageDeleted;
        }

        /// <summary>
        /// Handles a received message event.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
            else if (message.Channel is SocketDMChannel)
            {
                //TODO: handle direct messages.
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles an updated message event.
        /// </summary>
        /// <param name="previousMessage">The previous cacheable message.</param>
        /// <param name="currentMessage">The current cacheable message.</param>
        /// <param name="channel">The message channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Handles a deleted message event.
        /// </summary>
        /// <param name="message">The cacheable deleted message.</param>
        /// <param name="channel">The cacheable deleted message channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Checks if the server list is up to date with the guilds available in the Discord client.
        /// </summary>
        void CheckServerListUpToDate()
        {
            if (_servers.Count == _client.Guilds.Count) return;
            
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
