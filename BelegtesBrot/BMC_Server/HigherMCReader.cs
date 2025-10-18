using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BelegtesBrot.BMC_Server;
    public class PlayerEventArgs(string playername)
    {
        public string Playername => playername;
    }
public class MCReceivedMessage
{

    public event EventHandler? Ready;
    public event EventHandler? ShutdownComplete;
    public event EventHandler<PlayerEventArgs>? PlayerDisconnected;
    public event EventHandler<PlayerEventArgs>? PlayerConnected;


    private MCServer mcServer;
    public MCReceivedMessage(MCServer mcServer)
    {
        this.mcServer = mcServer;
        this.mcServer.ReceivedData += HandleReceivedServerData;
    }


    private void HandleReceivedServerData(object? sender, DataReceivedEventArgs e)
        {
        if (string.IsNullOrWhiteSpace(e.Data)) return;

        string data = e.Data;


        //[02:14:50] [Server thread/INFO]:
        // string serverPrefix = @"$$(\\d{2}:\\d{2}:\\d{2})$$ $$Server thread/INFO$$:";
        // string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";
        string serverPrefix = @"\[(\d{2}:\d{2}:\d{2})\] \[Server thread/INFO\]:";
        string serverIsReady = @$"{serverPrefix} Done \((\d+(\.\d+)?)s\)! For help, type ""help""";

        //string serverIsReady = @$"{serverPrefix} Done $(\\d+(\\.\\d+)?)s$! For help, type ""help""";

        string playerConnectPattern = @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) joined the game";
        string playerDiconnectPattern =
            @$"{serverPrefix} (?<playername>[a-zA-Z0-9_]+) lost connection: Disconnected$";
        string serverIsEmpty = @$"{serverPrefix} There are 0 of a max of (\d+) players online:.*";
        // Done (0.838s)! For help, type \"help\"
        string serverShutdown = @$"{serverPrefix} Stopped IO worker!";
        //serverShutdown = @$"{serverPrefix} ThreadedAnvilChunkStorage: All dimensions are saved";


        Match match;
        if (Regex.IsMatch(data, serverIsReady)) //Letzte nachricht, wenn der server hochgefahren wurde
        {
            Ready?.Invoke(mcServer, EventArgs.Empty);
        }
        else if (Regex.IsMatch(data, serverShutdown)) //Server Shutdown
        {
            ShutdownComplete?.Invoke(mcServer, EventArgs.Empty);
        }
        else if (Regex.IsMatch(data, playerDiconnectPattern)) //check if player disconnected
        {
            match = Regex.Match(data, playerDiconnectPattern);

            PlayerDisconnected?.Invoke(mcServer,new PlayerEventArgs(match.Groups["playername"].Value));
        }
        else if (Regex.IsMatch(data, playerConnectPattern)) //check if player connected
        {
            match = Regex.Match(data, playerConnectPattern);

            PlayerConnected?.Invoke(mcServer, new PlayerEventArgs(match.Groups["playername"].Value));
        }
        //else if (Regex.IsMatch(data, serverIsEmpty)) //check if server is empty
        //{
        //    StartServerstopCountDownTimer();
        //}
    }
}