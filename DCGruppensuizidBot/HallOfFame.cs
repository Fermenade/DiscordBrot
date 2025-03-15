using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DGruppensuizidBot;

public class HallOfFame
{
    
    private static string _prefixPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)?@"serverStuff\":@"serverStuff/";
    private static string FilePath = $"{_prefixPath}scoreboard.json";
            private List<ScoreEntry> entries;
    
            public HallOfFame([Optional]TimeSpan time, string[]? description = null)
            {
                entries = LoadEntries();
                if (description != null)
                {
                    AddEntry(time, description);
                }
            }
    
            public void AddEntry(TimeSpan time, string[] description)
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
    
            public List<ScoreEntry> GetEntries()
            {
                return entries;
            }
    
            private void SaveEntries()
            {
                using var fs = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
                 JsonSerializer.SerializeAsync(fs, entries);//Sync oder Async
            }
    
            private List<ScoreEntry> LoadEntries()
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