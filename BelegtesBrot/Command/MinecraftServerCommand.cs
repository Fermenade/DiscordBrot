using System.Text;
using BelegtesBrot.BMC_Server;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.commands;

public class MinecraftServerCommand
{
    private readonly CommandSession _commandSession;
    private readonly DirectoryInfo _minecraftServerDirectory;

    private MinecraftServer? _minecraftServer;

    internal MinecraftServerCommand(CommandSession commandSession)
    {
        _commandSession = commandSession;
        _minecraftServerDirectory =
            new DirectoryInfo(Path.Combine(_commandSession._session.BaseFolder.DirectoryInfo.FullName,
                "MinecraftServer"));
    }

    public async Task ServerCommand(SocketSlashCommand command)
    {
        if (!_minecraftServerDirectory.Exists)
            await command.RespondAsync("MinecraftServer Feature disabled for this server.");
        var subCommand = command.Data.Options.First().Name;

        _minecraftServer ??= new MinecraftServer(_minecraftServerDirectory);
        switch (subCommand)
        {
            case "start":
                StartCommand(command);
                break;
            case "stop":
                if (!command.Permissions.Administrator)
                    await command.FollowupAsync("Permission denied.");
                _minecraftServer.WriteToProcess(new StringBuilder("stop"));
                await command.RespondAsync("Stopping...");
                break;
            case "status":
                await command.RespondAsync(embed: BuildServerstats().Build());
                break;
            case "stats":
                await command.RespondAsync("Hiiiii UwU :3c");
                break;
        }
    }

    private EmbedBuilder BuildServerstats()
    {
        var embed = new EmbedBuilder
        {
            Title = "Server Status",
            Color = Color.DarkGreen
        };
        embed.AddField("Status:", _minecraftServer?.ServerState.ToString() ?? nameof(ServerState.Offline));
        if (_minecraftServer?.ServerState == ServerState.Online)
        {
            embed.AddField("Hobbylose:", $"{_minecraftServer._playerManager.CurrentOnlinePlayers.Count}");
            embed.AddField("MC wird seit",
                $"{FormatDifference(_minecraftServer._serverTimeMeasure!.GetTimeTillnow())} gesuchtet");
        }

        return embed;
    }

    private static string FormatDifference(TimeSpan timeSpan)
    {
        var years = 0;
        var months = 0;
        var days = timeSpan.Days;
        var hours = timeSpan.Hours;
        var minutes = timeSpan.Minutes; // Calculate minutes
        var seconds = timeSpan.Seconds;

        // Build the output string
        var result = $"{FormatPlural(years, "Jahr")} {FormatPlural(months, "Monat")} {FormatPlural(days, "Tag")} " +
                     $"{FormatPlural(hours, "Stunde")} {FormatPlural(minutes, "Minute")} {FormatPlural(seconds, "Sekunde")}";
        return result;
    }

    private static string FormatPlural(int count, string singular)
    {
        if (count == 0 && singular != "Sekunde") return "";
        var pluralForms = new Dictionary<string, string>
        {
            { "Jahr", "Jahre" },
            { "Monat", "Monate" },
            { "Tag", "Tage" },
            { "Stunde", "Stunden" },
            { "Minute", "Minuten" },
            { "Sekunde", "Sekunden" }
        };
        return count == 1 ? $"{count} {singular}" : $"{count} {pluralForms[singular]}";
    }

    private async void StartCommand(SocketSlashCommand command)
    {
        _minecraftServer.StartServer();
        await command.RespondAsync("Started");

        _minecraftServer.McReceived.Ready += McReceivedOnReady;

        async void McReceivedOnReady(object? sender, EventArgs e)
        {
            _minecraftServer.McReceived.Ready -= McReceivedOnReady;

            var messageReference = new MessageReference(command.Id);
            await command.Channel.SendMessageAsync("Online!", messageReference: messageReference);
        }
    }
}