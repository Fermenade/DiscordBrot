namespace BelegtesBrot.BMC_Server;

public class PlayerManager
{
    // Liste der aktuell online Spieler
    private HashSet<string> _currentOnlinePlayers = new(10);

    // Liste der insgesamt online gewesenen Spieler
    private HashSet<string> _allPlayers = new(10); //Maybe die Maxplayeranzahl dynamisch aus der server config auslesen
    // Spieler anmelden
    public void PlayerLogin(string playerName)
    {
        if (_currentOnlinePlayers.Add(playerName))
        {
            _allPlayers.Add(playerName);
        }
        else
        {
            throw new Exception($"Player {playerName} is already online");
        }
    }

    // Spieler abmelden
    public void PlayerLogout(string playerName)
    {
        _currentOnlinePlayers.Remove(playerName);
    }

    // Aktuell online Spieler abrufen
    public IEnumerable<string> GetCurrentOnlinePlayers()
    {
        return _currentOnlinePlayers;
    }

    // Insgesamt online gewesene Spieler abrufen
    public IEnumerable<string> GetAllPlayers()
    {
        return _allPlayers;
    }
}