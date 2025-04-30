// using Discord;
// using Discord.WebSocket;
//
// namespace DGruppensuizidBot.commands;
//
// internal static partial class Commands
// {
//     static EmbedBuilder BuildServerHalloffame()
//     {
//         EmbedBuilder embedBuilder = new EmbedBuilder()
//         {
//             Title = "Hall Of Fame",
//             Color = Color.Gold,
//         };
//         HallOfFame COLLECTION = new HallOfFame();
//         var s = COLLECTION.GetEntries();
//         foreach (ScoreEntry VARIABLE in s)
//         {
//             string a = "";
//             foreach (var VARIABLE1 in VARIABLE.Description)
//             {
//                 a += VARIABLE1 + " ";
//             }
//
//             if (string.IsNullOrWhiteSpace(a)) break;
//             embedBuilder.AddField(FormatDifference(VARIABLE.Time), a);
//         }
//
//         return embedBuilder;
//     }
//
//     static EmbedBuilder BuildServerstats()
//     {
//         var embed = new EmbedBuilder
//         {
//             Title = "Server Status",
//             Color = Color.Green,
//         };
//         embed.AddField("Status:", ServerInformation.GetServerState().ToString());
//         if (ServerInformation.GetServerOnline())
//         {
//             embed.AddField("Hobbylose:", $"{PlayerManager.GetCurrentOnlinePlayers()}");
//             embed.AddField("MC wird seit", $"{FormatDifference(CalculateDifference())} gesuchtet");
//         }
//
//         return embed;
//     }
//
//     static TimeSpan shutdownTime = TimeSpan.FromMinutes(10);
//
//     static TimeSpan CalculateDifference()
//     {
//         _endTime = DateTime.Now;
//         if (!ServerInformation.GetServerOnline())
//         {
//             _endTime = _endTime - shutdownTime;
//         }
//
//         // Calculate the difference
//         TimeSpan timeSpan = TimeSpan.FromSeconds(0);
//         if (_startTime != DateTime.MinValue)
//         {
//             timeSpan = _endTime - _startTime;
//         }
//
//         // Output the result
//         return timeSpan;
//     }
//
//     static string FormatDifference(TimeSpan timeSpan)
//     {
//         int years = 0;
//         int months = 0;
//         int days = timeSpan.Days;
//         int hours = timeSpan.Hours;
//         int minutes = timeSpan.Minutes; // Calculate minutes
//         int seconds = timeSpan.Seconds;
//
//         // Build the output string
//         string result = $"{FormatPlural(years, "Jahr")} {FormatPlural(months, "Monat")} {FormatPlural(days, "Tag")} " +
//                         $"{FormatPlural(hours, "Stunde")} {FormatPlural(minutes, "Minute")} {FormatPlural(seconds, "Sekunde")}";
//         return result;
//     }
//
//     static string FormatPlural(int count, string singular)
//     {
//         if (count == 0 && singular != "Sekunde") return "";
//         Dictionary<string, string> pluralForms = new Dictionary<string, string>
//         {
//             { "Jahr", "Jahre" },
//             { "Monat", "Monate" },
//             { "Tag", "Tage" },
//             { "Stunde", "Stunden" },
//             { "Minute", "Minuten" },
//             { "Sekunde", "Sekunden" }
//         };
//         return count == 1 ? $"{count} {singular}" : $"{count} {pluralForms[singular]}";
//     }
//
//     static bool CheckValidCommand(SocketMessage Message)
//     {
//         string command = Message.Content;
//         if (!string.IsNullOrEmpty(command))
//         {
//             if (command[0].ToString() == "!")
//             {
//                 return true;
//             }
//         }
//         return false;
//     }
// }