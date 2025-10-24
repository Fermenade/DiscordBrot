using System.CommandLine;
using System.CommandLine.Parsing;
using BelegtesBrot.BMC_Server;

namespace BelegtesBrot.commands;

public class Commands
{
    public RootCommand root = new RootCommand();

    public Commands()
    {
        root.Add(new ServerCommand());
    }
}
public class ServerCommand : Command
{
    private readonly HigherMc _server;

    public ServerCommand() : base("server")
    {
        _server =  new HigherMc();
        Command startCommand = new Command("start");
        startCommand.SetAction(_ => StartServer());
        this.Add(startCommand);
            
        Command stopCommand = new Command("stop");
        stopCommand.SetAction(_ => StopServer());
        this.Add(stopCommand);
    }

    private void StartServer()
    {
        _server.StartServer();
    }

    private void StopServer()
    {
        throw new NotImplementedException();
    }
}