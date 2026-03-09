using System.Text;
using BelegtesBrot.MinecraftServer;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.Command;

public class MinecraftServerCommand
{
    private CommandSession _commandSession;
    private readonly DirectoryInfo _minecraftServerDirectory;

    private MinecraftServer.MinecraftServer? _minecraftServer;

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
        {
            await command.RespondAsync("MinecraftServer Feature disabled for this server.");
            return;
        }

        var subCommand = command.Data.Options.First().Name;

        _minecraftServer ??= new MinecraftServer.MinecraftServer(_minecraftServerDirectory,_commandSession._session);
        switch (subCommand)
        {
            case "start":
                StartCommand(command);
                break;
            case "status":
                var e = BuildServerstats().Build();
                await command.RespondAsync(embed: e);
                break;
            case "stats":
                var x = BuildHallOfFameStats().Build();
                await command.RespondAsync(embed: x);
                break;
        }
    }

    private EmbedBuilder BuildHallOfFameStats()
    {
        if (_minecraftServer == null)
        { 
             _commandSession._session.Logger.LogMessage("Tried to start server but Minecraft Server was not initalized");
            return null!;
        }
        var e = _minecraftServer.HallOfFame.GetEntries()!.SkipLast(5);
        var embed = new EmbedBuilder().WithTitle("Hall of Fame").WithColor(Color.Gold);
        foreach (var entry in e)
        {
            embed.AddField(FormatDifference(entry.Time),$"{string.Join(", ",entry.Players)}");
        }
        return embed;
    }
    private EmbedBuilder BuildServerstats()
    {
        var embed = new EmbedBuilder
        {
            Title = "Server Status",
            Color = Color.DarkGreen
        };
        embed.AddField("Status:", _minecraftServer!.MinecraftServerState?.ToString() ?? nameof(ServerState.Offline));
        if (_minecraftServer?.MinecraftServerState == ServerState.Online)
        {
            embed.AddField("Hobbylose:", $"{_minecraftServer.PlayerManager.CurrentOnlinePlayers.Count}");
            embed.AddField("MC wird seit",
                $"{FormatDifference(_minecraftServer.ServerTimeMeasure!.GetTimeTillnow())} gesuchtet");
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
        if (_minecraftServer == null)
        { 
           await _commandSession._session.Logger.LogMessage("Tried to start server but Minecraft Server was not initalized");
           return;
        }
        
        _minecraftServer.StartServer();
        await command.RespondAsync("Started");

        _minecraftServer.McReceived.Ready += McReceivedOnReady;

        async void McReceivedOnReady(object? sender, EventArgs e)
        {
            _minecraftServer.McReceived.Ready -= McReceivedOnReady;
            
            await command.Channel.SendMessageAsync($"{command.User.Mention} Online!");
        }
    }
}