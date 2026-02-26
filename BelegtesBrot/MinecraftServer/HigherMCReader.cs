using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BelegtesBrot.MinecraftServer;

public class PlayerEventArgs(string playername)
{
    public string Playername => playername;
}

public class MCReceivedMessage
{
    private readonly Server _server;
    private MinecraftLogger _logger;

    public MCReceivedMessage(Server server, MinecraftLogger logger)
    {
        _server = server;
        _server.ReceivedData += HandleReceivedServerData;
        _logger = logger;
    }

    public event EventHandler? Ready;
    public event EventHandler? ShutdownComplete;
    public event EventHandler<PlayerEventArgs>? PlayerDisconnected;
    public event EventHandler<PlayerEventArgs>? PlayerConnected;

    public event EventHandler? ServerEmpty;

    private void HandleReceivedServerData(object? sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.Data)) return;

        var data = e.Data;


        //[02:14:50] [Server thread/INFO]:
        // string serverPrefix = @"$$(\\d{2}:\\d{2}:\\d{2})$$ $$Server thread/INFO$$:";
        // string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";
        var serverPrefix = @"\[(\d{2}:\d{2}:\d{2})\] \[Server thread/INFO\]:";
        var serverIsReady = @$"{serverPrefix} Done \((\d+(\.\d+)?)s\)! For help, type ""help""";

        //string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";

        var playerConnectPattern = @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) joined the game";
        var playerDiconnectPattern =
            @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) lost connection: Disconnected$";
        var serverIsEmpty = @$"{serverPrefix} There are 0 of a max of (\d+) players online:.*";
        // Done (0.838s)! For help, type \"help\"
        var serverShutdown = @$"{serverPrefix} Stopped IO worker!";
        //serverShutdown = @$"{serverPrefix} ThreadedAnvilChunkStorage: All dimensions are saved";
        var serverFailedToStart = @"\[(\d{2}:\d{2}:\d{2})\] [ServerMain/ERROR]: Failed to start the minecraft server";

        _logger.LogMessage(data);
        Match match;
        if (Regex.IsMatch(data, serverIsReady)) //Letzte nachricht, wenn der server hochgefahren wurde
        {
            Ready?.Invoke(_server, EventArgs.Empty);
        }
        else if (Regex.IsMatch(data, serverShutdown)) //Server Shutdown
        {
            ShutdownComplete?.Invoke(_server, EventArgs.Empty);
        }
        else if (Regex.IsMatch(data, playerDiconnectPattern)) //check if player disconnected
        {
            match = Regex.Match(data, playerDiconnectPattern);
            PlayerDisconnected?.Invoke(_server, new PlayerEventArgs(match.Groups["playername"].Value));
        }
        else if (Regex.IsMatch(data, playerConnectPattern)) //check if player connected
        {
            match = Regex.Match(data, playerConnectPattern);

            PlayerConnected?.Invoke(_server, new PlayerEventArgs(match.Groups["playername"].Value));
        }
        else if (Regex.IsMatch(data, serverIsEmpty)) //check if server is empty
        {
            ServerEmpty?.Invoke(_server, EventArgs.Empty);
        }
        else if (Regex.IsMatch(data, serverFailedToStart))
        {
            _logger.LogMessage("Failed to start the minecraft server, further details in the minecraft server logs.");
        }
    }
}