using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot.BMC_Server;

public static class HallOfFame
{
    private static string FilePath = $"{Info.InfoFolder}/scoreboard.json";
    private const uint _count = 100;
    public static void AddEntry(TimeSpan time, string[] players)
    {
        List<ScoreEntry> entries;
        
        entries = LoadEntries();
        if (entries.Count < _count || time > entries.Min(e => e.Time))
        {
            if (entries.Count >= _count)
            {
                // Entferne den kürzesten Eintrag
                entries.Remove(entries.OrderBy(e => e.Time).First());
            }

            entries.Add(new ScoreEntry(DateTime.Now, time, players));
            entries = entries.OrderByDescending(e => e.Time).ToList();
            SaveEntries(entries);
        }
    }

    public static IReadOnlyCollection<ScoreEntry>? GetEntries()
    {
        return LoadEntries();
    }

    private static async void SaveEntries<T>(T entries)
    {
        await using FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
        await JsonSerializer.SerializeAsync(fs, entries); //Sync oder Async
    }

    private static List<ScoreEntry>? LoadEntries()
    {
        if (!File.Exists(FilePath)) return null;
        var json = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<List<ScoreEntry>>(json);
    }
}

[Serializable]
[method: JsonConstructor]
public class ScoreEntry (DateTime dateTime,TimeSpan time, string[] players)
{
    public DateTime DateTime => dateTime;
    public TimeSpan Time => time;
    public string[] Players => players;
}