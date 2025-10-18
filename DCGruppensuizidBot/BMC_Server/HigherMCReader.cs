using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BelegteBrot.BMC_Server;

public partial class HigherMC
{
        void HandleReceivedServerData(object? sender, DataReceivedEventArgs e)
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
            //Der Server soll gleich in den shutdown timer laufen lassen, damit falls keine person joinen sollte der server nicht online bleibt

            ServerState = ServerState.Online;
            ServerTimeMeasure.StartTime = DateTime.Now;
            ServerReady.Invoke();
            //leuten ne sperre verpassen, die den server hochfahren ohne das dieser benutzt wird.
        }
        else if (Regex.IsMatch(data, serverShutdown)) //Server Shutdown
        {
            //Here is the server online bool set to false, so that the 
            ServerState = ServerState.Offline;
            //See if Run comes into hall of fame
            HallOfFame s = new HallOfFame(ServerTimeMeasure.GetTimeTillnow(),
                PlayerManager.GetAllPlayers().OrderBy(x => x).ToArray());

        }
        else if (Regex.IsMatch(data, playerDiconnectPattern)) //check if player disconnected
        {
            match = Regex.Match(data, playerDiconnectPattern);

            PlayerManager.PlayerLogout(match.Groups["playername"].Value);

            if (!PlayerManager.GetCurrentOnlinePlayers().Any())
            {
                StartServerstopCountDownTimer();
            }
        }
        else if (Regex.IsMatch(data, playerConnectPattern)) //check if player connected
        {
            match = Regex.Match(data, playerConnectPattern);

            PlayerManager.PlayerLogin(match.Groups["playername"].Value);
        }
        //else if (Regex.IsMatch(data, serverIsEmpty)) //check if server is empty
        //{
        //    StartServerstopCountDownTimer();
        //}
    }
}