using BelegtesBrot.FileSystem;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot.MinecraftServer;

public class HallOfFame : JsonFile
{
    private const uint _count = 100;

    public HallOfFame(DirectoryInfo baseFolder) : base(baseFolder)
    {
    }

    public override string Name => "McScoreboard.json";

    public void AddEntry(TimeSpan time, string[] players)
    {
        List<ScoreEntry> entries = LoadEntries() ?? [];
        if (entries.Count < _count || time > entries.Min(e => e.Time))
        {
            if (entries.Count >= _count)
                // Entferne den kürzesten Eintrag
                entries.Remove(entries.OrderBy(e => e.Time).First());

            entries.Add(new ScoreEntry(DateTime.Now, time, players));
            SaveEntries(entries.OrderBy(e => e.Time));
        }
    }
    public IReadOnlyCollection<ScoreEntry>? GetEntries() => LoadEntries();

    private async void SaveEntries(IEnumerable<ScoreEntry> entries)=> await SaveAsync(entries);

    private List<ScoreEntry>? LoadEntries() => LoadAsync<List<ScoreEntry>>().Result;
}

[Serializable]
[method: JsonConstructor]
public class ScoreEntry(DateTime dateTime, TimeSpan time, string[] players)
{
    public DateTime DateTime => dateTime;
    public TimeSpan Time => time;
    public string[] Players => players;
}