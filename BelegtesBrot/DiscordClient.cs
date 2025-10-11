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
            if (message.Channel is SocketGuildChannel channel)
            {
                foreach (var VARIABLE in _servers)
                {
                    if (VARIABLE.Guild.Id == channel.Guild.Id)
                    {
                        VARIABLE.MessageReceived(message);
                    }
                }
            }
        }
    }
}
