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
            //CreateScoreboardFile();
            //WriteScoreboard();
            //DeleteScoreboardFile();
        }
        private void WriteScoreboard()
        {
            Dictionary<ulong, List<object>> map = new();
            //map = JsonSerializer.Deserialize(stuff);
            S sd = new("user", 123);
            //map.Add(11233, sd);
            List<object> sdf = [];
            sdf.Add("user");
            sdf.Add(122323);
            map.Add(213123,sdf);
            FileStream fs = new(scorefilepath, FileMode.Open,FileAccess.Write);
            JsonSerializer.SerializeAsync(fs, map);
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
