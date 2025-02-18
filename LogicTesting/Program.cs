using System.Text.Json;

namespace LogicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program().RunThisShit();
        }
        private void RunThisShit()
        {
            CreateScoreboardFile();
            WriteScoreboard();
            //DeleteScoreboardFile();
            //WriteScoreboard(null);
        }
        private async void WriteScoreboard(Dictionary<ulong, object[]> map)
        {
            FileStream fs = new(scorefilepath, FileMode.Open, FileAccess.Write);
            map ??= [];//wenn file korrupt oder (noch) leer
            if (!map.ContainsKey(324324)) map.Add(324324, ["Username", 0]);
            //else map[user.Id] = [user.Username, (ushort)map[user.Id][1] + 1];
            await JsonSerializer.SerializeAsync(fs, "map");//Sync oder Async
        }
        private async void WriteScoreboard()
        {
            Dictionary<ulong, object[]> map = new();
            //map = JsonSerializer.Deserialize(stuff);
            S sd = new("user", 123);
            //map.Add(11233, sd);
            object[] sdf = ["User", 123123];
            map.Add(2123123,sdf);
            FileStream fs = new(scorefilepath, FileMode.Open,FileAccess.Write);
            await JsonSerializer.SerializeAsync(fs, map);
            fs.Close();
        }
        string scorefilepath = "scores.json";
        private void CreateScoreboardFile()
        {
            if (!File.Exists(scorefilepath)) File.Create(scorefilepath);
        }
        private void DeleteScoreboardFile()
        {
            if (File.Exists(scorefilepath)) File.Delete(scorefilepath);
        }
        struct S(string usern, ushort count)
        {
            string username = usern;
            ushort failurecount = count;
        }
    }
}
