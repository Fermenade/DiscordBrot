using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot.BMC_Server;

public class HallOfFame
{

    private static string FilePath = $"{Info.InfoFolder}/scoreboard.json";
    private List<ScoreEntry> entries;
    //Warum ist dieser parameter optional??
    public HallOfFame(TimeSpan time, string[] description)
    {
        entries = LoadEntries();

        AddEntry(time, description);
    }

    private void AddEntry(TimeSpan time, string[] description)
    {
        // Füge neuen Eintrag hinzu, wenn er länger ist als der kürzeste Eintrag
        if (entries.Count < 5 || time > entries.Min(e => e.Time))
        {
            if (entries.Count >= 5)
            {
                // Entferne den kürzesten Eintrag
                entries.Remove(entries.OrderBy(e => e.Time).First());
            }

            // Füge den neuen Eintrag hinzu
            entries.Add(new ScoreEntry { Time = time, Description = description });
            // Sortiere die Liste
            entries = entries.OrderByDescending(e => e.Time).ToList();
            SaveEntries();
        }
    }

    public static IReadOnlyCollection<ScoreEntry> GetEntries()
    {
        return LoadEntries();
    }

    private void SaveEntries()
    {
        using FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
        JsonSerializer.SerializeAsync(fs, entries); //Sync oder Async
    }

    private static List<ScoreEntry> LoadEntries()
    {
        if (File.Exists(FilePath))
        {
            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<ScoreEntry>>(json) ?? new List<ScoreEntry>();
        }

        return new List<ScoreEntry>();
    }
}

public class ScoreEntry
{
    public TimeSpan Time { get; set; }
    public string[] Description { get; set; }
}