namespace BelegtesBrot.BMC_Server;

public class PlayerManager
{
    public PlayerManager(int capacity)
    {
        _currentOnlinePlayers = new HashSet<string>(capacity);
        _allPlayers = new HashSet<string>(capacity);
    }
    // Liste der aktuell online Spieler
    private readonly HashSet<string> _currentOnlinePlayers;

    // Liste der insgesamt online gewesenen Spieler
    private readonly HashSet<string> _allPlayers;
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

    /// <summary>
    /// Remove a player from current online players
    /// </summary>
    /// <param name="playerName">The player name of the player</param>
    public void PlayerLogout(string playerName)
    {
        _currentOnlinePlayers.Remove(playerName);
    }

    /// <summary>
    /// Get all players that are currently online.
    /// </summary>
    /// <returns>Collection of all player names</returns>
    public IReadOnlyCollection<string> GetCurrentOnlinePlayers()
    {
        return _currentOnlinePlayers;
    }

    /// <summary>
     /// Get all players that where online during this session.
    /// </summary>
    /// <returns>Collection of all player names</returns>
    public IReadOnlyCollection<string> GetAllPlayers()
    {
        return _allPlayers;
    }
}