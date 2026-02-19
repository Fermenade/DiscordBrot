namespace BelegtesBrot.BMC_Server;

public class PlayerManager
{
    private readonly HashSet<string> _allPlayers;
    private readonly HashSet<string> _currentOnlinePlayers;

    public PlayerManager(int capacity)
    {
        _currentOnlinePlayers = new HashSet<string>(capacity);
        _allPlayers = new HashSet<string>(capacity);
    }

    public IReadOnlyCollection<string> CurrentOnlinePlayers => _currentOnlinePlayers;
    public IReadOnlyCollection<string> AllPlayers => _allPlayers;

    public void PlayerLogin(string playerName)
    {
        if (_currentOnlinePlayers.Add(playerName))
            _allPlayers.Add(playerName);
        else
            throw new Exception($"Player {playerName} is already online");
    }

    /// <summary>
    ///     Remove a player from current online players
    /// </summary>
    /// <param name="playerName">The player name of the player</param>
    public void PlayerLogout(string playerName)
    {
        _currentOnlinePlayers.Remove(playerName);
    }
}