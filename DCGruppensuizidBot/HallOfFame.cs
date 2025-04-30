using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DGruppensuizidBot;

public class HallOfFame
{
    /*TODO: fix this error
     Unhandled exception. Newtonsoft.Json.JsonReaderException: Additional text encountered after finished reading JSON content: d. Path '', line 1, position 423.
   at Newtonsoft.Json.JsonTextReader.Read()
   at Newtonsoft.Json.Serialization.JsonSerializerInternalReader.Deserialize(JsonReader reader, Type objectType, boolean checkAdditionalContent)
   at Newtonsoft.Json.JsonSerializer.DeserializeInternal(JsonReader reader, Type objectType)
   at Newtonsoft.Json.JsonConvert.DeserializeObject(String value, Type type, JsonSerializerSettings settings)
   at Newtonsoft.Json.JsonConvert.DeserializeObject[T](String value, JsonSerializerSettings settings)
   at Newtonsoft.Json.JsonConvert.DeserializeObject[T](String value)
   at DGruppensuizidBot.HallOfFame.LoadEntries() in /mnt/0_WorkWindows/Users/dumblecore/source/repos/GruppensuizidDC/DCGruppensuizidBot/HallOfFame.cs:line 58
   at DGruppensuizidBot.HallOfFame..ctor(TimeSpan time, String[] description) in /mnt/0_WorkWindows/Users/dumblecore/source/repos/GruppensuizidDC/DCGruppensuizidBot/HallOfFame.cs:line 17
   at Program.HandleRecivedServerData(Object sender, DataReceivedEventArgs e) in /mnt/0_WorkWindows/Users/dumblecore/source/repos/GruppensuizidDC/DCGruppensuizidBot/Program.cs:line 494
   at System.Diagnostics.AsyncStreamReader.FlushMessageQueue(boolean rethrowInNewThread)
--- End of stack trace from previous location ---
   at System.Threading.ThreadPoolWorkQueue.Dispatch()
   at System.Threading.PortableThreadPool.WorkerThread.WorkerThreadStart()
Aborted (core dumped)
     */
    private static string FilePath = $"{Serverstuff.PrefixPath}scoreboard.json";
    private List<ScoreEntry> entries;
//Warum ist dieser parameter optional??
    public HallOfFame([Optional] TimeSpan time, string[]? description = null)
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
        using FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
        JsonSerializer.SerializeAsync(fs, entries); //Sync oder Async
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