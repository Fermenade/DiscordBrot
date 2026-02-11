using Discord;
using Discord.WebSocket;

namespace BelegtesBrot;

internal class DiscordClient : IBaseCom
{
    public static DirectoryInfo _directoryInfo = new(Path.Combine(Environment.CurrentDirectory, "data"));

    private readonly DiscordSocketClient _client;

    private readonly List<IServer> _servers = new();


    public DiscordClient(DiscordSocketClient client)
    {
        _client = client;
    }

    /// <summary>
    ///     Handles a received message event.
    /// </summary>
    /// <param name="message">The received message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task MessageReceived(IMessage message)
    {
        while (true)
        {
            switch (message.Channel)
            {
                case IGuildChannel channel:
                {
                    foreach (var socketGuild in _servers.Where(socketGuild => socketGuild.Guild.Id == channel.Guild.Id))
                        return socketGuild.MessageReceived(message);

                    _servers.Add(new Server(channel.Guild));
                    continue;
                }
                case IDMChannel dmChannel:
                    break;
                default:
                    Console.WriteLine("Received message was neither from Guild or DM: " +
                                      message.Type); // To test if there is any other case.
                    break;
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    ///     Handles an updated message event.
    /// </summary>
    /// <param name="previousMessage">The previous cacheable message.</param>
    /// <param name="currentMessage">The current cacheable message.</param>
    /// <param name="channel">The message channel.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage,
        IMessageChannel channel)
    {
        while (true)
        {
            switch (channel)
            {
                case IGuildChannel chan:
                {
                    foreach (var socketGuild in _servers.Where(socketGuild => socketGuild.Guild.Id == chan.Guild.Id))
                        return socketGuild.MessageUpdated(previousMessage, currentMessage, channel);

                    _servers.Add(new Server(chan.Guild));
                    continue;
                }
                case IDMChannel dmChannel:
                    //Direct message.
                    break;
            }

            return Task.CompletedTask;
            break;
        }
    }

    /// <summary>
    ///     Handles a deleted message event.
    /// </summary>
    /// <param name="message">The cacheable deleted message.</param>
    /// <param name="channel">The cacheable deleted message channel.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        while (true)
        {
            switch (channel.Value)
            {
                case IGuildChannel chan:
                {
                    foreach (var socketGuild in _servers.Where(socketGuild => socketGuild.Guild.Id == chan.Guild.Id))
                        return socketGuild.MessageDeleted(message, channel);

                    _servers.Add(new Server(chan.Guild));
                    continue;
                }
                case IDMChannel dmChannel:
                    break;
            }

            return Task.CompletedTask;
            break;
        }
    }

    public Task SlashCommandExecuted(SocketSlashCommand command)
    {
        while (true)
        {
            switch (command.Channel)
            {
                case IGuildChannel chan:
                {
                    foreach (var socketGuild in _servers.Where(socketGuild => socketGuild.Guild.Id == chan.Guild.Id))
                        return socketGuild.SlashCommandExecuted(command);

                    _servers.Add(new Server(chan.Guild));
                    continue;
                }
                case IDMChannel dmChannel:
                    break;
            }

            return Task.CompletedTask;
            break;
        }
    }

    public void Ready()
    {
        _client.MessageReceived += MessageReceived;
        _client.MessageUpdated += MessageUpdated;
        _client.MessageDeleted += MessageDeleted;
        _client.SlashCommandExecuted += SlashCommandExecuted;
    }
}