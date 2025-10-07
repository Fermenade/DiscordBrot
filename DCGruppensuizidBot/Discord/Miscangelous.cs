using Discord;

namespace DGruppensuizidBot.Discord;

public class Miscangelous
{
    byte TeaThinkCounter = 0;
    private async Task UpdateStatusAsync(CancellationToken cancellationToken)
    {
        byte i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            var currentHour = DateTime.Now.Hour;

            // Set status based on the time of day
            if (_random.Next(2) == 1)
            {
                if (currentHour >= 8 && currentHour <= 22)
                {
                    await _client.SetStatusAsync(UserStatus.Online);
                }
                else
                {
                    await _client.SetStatusAsync(UserStatus.AFK);
                }
            }

            if (currentHour <= 4)
            {
                if (i == 0) continue;
                i = 0;
            }
            else if (currentHour <= 10)
            {
                if (i == 1) continue;
                i = 1;
            }
            else if (currentHour <= 14)
            {
                if (i == 2) continue;
                i = 2;
            }
            else if (currentHour <= 18)
            {
                if (i == 3) continue;
                i = 3;
            }
            else if (currentHour <= 23)
            {
                if (i == 4) continue;
                i = 4;
            }
            else
            {
                if (i == 5) continue;
                i = 5;
            }

            // Read messages from the text file
            CreateFileIfNotExists(Serverstuff.PathActivities);
            string[] lines = await File.ReadAllLinesAsync(Serverstuff.PathActivities);
            var filteredLines = lines.Where(x => x.Split("@")[0] == $"{i}").ToArray();

            if (filteredLines.Length > 0)
            {
                // Select a random line from the filtered lines
                string selectedLine = filteredLines[_random.Next(filteredLines.Length)];
                lines = selectedLine.Split("@");

                // Optionally replace lines with an empty array based on a random condition
                if (_random.Next(3) != 0)
                {
                    lines = []; // or "".Split() if you prefer
                }
            }
            else
            {
                lines = [];
            }

            if (lines[1] == " Denkt über Tee nach" && TeaThinkCounter != 6)
            {
                lines[1] = " Denkt über Tee nach";
            }
            else if (TeaThinkCounter == 6) TeaThinkCounter = 0;

            if (lines.Length == 3)
            {
                await _client.SetGameAsync(lines[2].Trim());
            }
            else
            {
                await _client.SetCustomStatusAsync(lines[1].Trim());
            }

            // Und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte und ich warte
            await Task.Delay(TimeSpan.FromMinutes(60), cancellationToken);
        }
    }
}