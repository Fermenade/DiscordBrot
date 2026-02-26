using Discord.WebSocket;

namespace BelegtesBrot.Command;

//Create a session class that has all base functionality of a session?
internal class CommandSession
{
    private static readonly List<Session> _handledSessions = new();

    private readonly MinecraftServerCommand _minecraftServerCommand;
    private readonly ModeCommand _modeCommand;
    internal readonly Session _session;

    public CommandSession(Session session)
    {
        if (_handledSessions.Any(x => x == session))
            throw new InvalidOperationException("A Session can only have one CommandSession");
        _handledSessions.Add(session);
        _session = session;

        _minecraftServerCommand = new MinecraftServerCommand(this);
        _modeCommand = new ModeCommand(this);
    }

    public Task Command(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "server":
                return _minecraftServerCommand.ServerCommand(command);
            case "mode":
                return _modeCommand.ModeCommandExecute(command);
            default:
                return command.RespondAsync(
                    "Now you caught me there >:3 \n You found a command that wasn't implemented.\n" +
                    "You did it! Now go whining to the developer about it.");
        }
    }
}