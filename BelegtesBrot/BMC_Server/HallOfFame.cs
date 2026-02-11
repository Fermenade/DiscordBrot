using BelegtesBrot.FileSystem;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot.BMC_Server;

public class HallOfFame : JsonFile
{
    private const uint _count = 100;

    public HallOfFame(DirectoryInfo baseFolder) : base(baseFolder)
    {
    }

    public override string Name => "McScoreboard.json";

    public void AddEntry(TimeSpan time, string[] players)
    {
        List<ScoreEntry> entries;

        entries = LoadEntries();
        if (entries.Count < _count || time > entries.Min(e => e.Time))
        {
            if (entries.Count >= _count)
                // Entferne den kürzesten Eintrag
                entries.Remove(entries.OrderBy(e => e.Time).First());

            entries.Add(new ScoreEntry(DateTime.Now, time, players));
            entries = entries.OrderByDescending(e => e.Time).ToList();
            SaveEntries(entries);
        }
    }

    public IReadOnlyCollection<ScoreEntry>? GetEntries()
    {
        return LoadEntries();
    }

    private async void SaveEntries<T>(T entries)
    {
        await using var fs = new FileStream(FileInfo.FullName, FileMode.OpenOrCreate, FileAccess.Write);
        await JsonSerializer.SerializeAsync(fs, entries); //Sync oder Async
    }

    private List<ScoreEntry>? LoadEntries()
    {
        if (!File.Exists(FileInfo.FullName)) return null;
        var json = File.ReadAllText(FileInfo.FullName);
        return JsonConvert.DeserializeObject<List<ScoreEntry>>(json);
    }
}

[Serializable]
[method: JsonConstructor]
public class ScoreEntry(DateTime dateTime, TimeSpan time, string[] players)
{
    public DateTime DateTime => dateTime;
    public TimeSpan Time => time;
    public string[] Players => players;
}