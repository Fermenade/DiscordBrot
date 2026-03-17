using Discord;

namespace BelegtesBrot;

public class Blacklist
{
    private readonly string _filePath;
    private readonly HashSet<ulong> _items;

    public Blacklist(string filePath)
    {
        _filePath = filePath;
        _items = new HashSet<ulong>();

        LoadFromFile();
    }

    public IReadOnlyCollection<ulong> Items => _items;

    private void LoadFromFile()
    {
        if (!File.Exists(_filePath))
            return;

        foreach (var line in File.ReadLines(_filePath))
            if (ulong.TryParse(line, out var value))
                _items.Add(value);
    }

    private void SaveToFile()
    {
        // Write atomically to avoid corruption
        var tempFile = _filePath + ".tmp";

        File.WriteAllLines(tempFile, _items.Select(v => v.ToString()));
        File.Move(tempFile, _filePath, true);
    }

    public bool Contains(IGuild guild)
    {
        return Contains(guild.Id);
    }

    public bool Contains(ulong value)
    {
        return _items.Contains(value);
    }


    public bool Add(IGuild guild)
    {
        return Add(guild.Id);
    }

    public bool Add(ulong value)
    {
        if (!_items.Add(value)) return false;

        SaveToFile();
        return true;
    }


    public bool Remove(IGuild guild)
    {
        return Remove(guild.Id);
    }

    public bool Remove(ulong value)
    {
        if (_items.Remove(value))
        {
            SaveToFile();
            return true;
        }

        return false;
    }
}