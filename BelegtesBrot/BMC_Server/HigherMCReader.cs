using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BelegtesBrot.BMC_Server;

public class PlayerEventArgs(string playername)
{
    public string Playername => playername;
}

internal class MCReceivedMessage
{
    private readonly Server _server;

    public MCReceivedMessage(Server server)
    {
        _server = server;
        _server.ReceivedData += HandleReceivedServerData;
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
    }
}