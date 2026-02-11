using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Channels;

[ModeName("cli", "Dummy modus der warscheinlich entfernt wird")]
internal class CommandMode : IBaseCom
{
    private readonly IServer _session;

    public CommandMode(IServer session)
    {
        _session = session;
    }

    public async Task MessageReceived(IMessage message)
    {
        if (message.Content[0] != '!') await message.DeleteAsync();
    }

    public Task MessageUpdated(Cacheable<IMessage, ulong> previousMessage, IMessage currentMessage,
        IMessageChannel channel)
    {
        return Task.CompletedTask;
    }

    public Task MessageDeleted(Cacheable<IMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel)
    {
        return Task.CompletedTask;
    }

    public Task SlashCommandExecuted(SocketSlashCommand arg)
    {
        throw new NotImplementedException();
    }
}