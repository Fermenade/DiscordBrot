using System.Text.Json;
using BelegtesBrot.Guild.Channels;
using Discord;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BelegtesBrot.Command;

public class InitCommands
{
    public static List<ApplicationCommandProperties> ApplicationCommandOptionPropertiesList = new();
    public static FileInfo fileInfo = new(Path.Combine(DiscordClient._directoryInfo.FullName, "CommandHashes.json"));

    public static async Task Init()
    {
        return;
        // -----------------------------------------------------
        // Minecraft server command
        // -----------------------------------------------------

        var serverCommand = new SlashCommandBuilder().WithName("server")
            .WithDescription("Minecraft server related commands")
            .AddOption(new SlashCommandOptionBuilder().WithName("start").WithDescription("Start the server")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder().WithName("stop").WithDescription("Stop the server")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder().WithName("status").WithDescription("Show status of server")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder().WithName("stats").WithDescription("Show stats of server")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .WithContextTypes(InteractionContextType.Guild);

        ApplicationCommandOptionPropertiesList.Add(serverCommand.Build());

        // -----------------------------------------------------
        // Mode selection command
        // -----------------------------------------------------

        var modeSelection = new SlashCommandOptionBuilder()
            .WithName("mode").WithDescription("The mode")
            .WithRequired(true);
        foreach (var modeChannelName in ModeChannelMapper.Map.Keys)
            modeSelection.AddChoice(modeChannelName, modeChannelName);

        modeSelection.WithType(ApplicationCommandOptionType.String);


        var channelSelection = new SlashCommandOptionBuilder()
            .WithName("channel")
            .WithDescription("Select a channel the mode should apply to")
            .WithType(ApplicationCommandOptionType.Channel)
            .AddChannelType(ChannelType.Text)
            .AddChannelType(ChannelType.PrivateThread)
            .AddChannelType(ChannelType.PublicThread)
            .WithRequired(true);

        var modeCommand = new SlashCommandBuilder().WithName("mode").WithDescription("Mode related commands")
            .AddOption(new SlashCommandOptionBuilder().WithName("set").WithDescription("Set a channel to mode")
                .WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(channelSelection)
                .AddOption(modeSelection))
            .AddOption(new SlashCommandOptionBuilder().WithName("unset")
                .WithDescription("Unset a channel from mode").WithType(ApplicationCommandOptionType.SubCommand)
                .AddOption(channelSelection))
            .AddOption(new SlashCommandOptionBuilder().WithName("list").WithDescription("List modes")
                .WithType(ApplicationCommandOptionType.SubCommand))
            .WithContextTypes(InteractionContextType.Guild)
            .WithDefaultMemberPermissions(GuildPermission.ManageChannels);

        ApplicationCommandOptionPropertiesList.Add(modeCommand.Build());

        //Program._client.Rest.BulkOverwriteGlobalCommands([]);
        Program._client.Rest.BulkOverwriteGuildCommands(ApplicationCommandOptionPropertiesList.ToArray(),
            849240846125367376);


        //var hashSet = ApplicationCommandOptionPropertiesList.ToDictionary(x => x.Name.Value, x => x.GetHashCode());

        /*// Save initial hashes
        SaveHashes(hashSet);


        // Load old hashes
        var oldHashes = LoadHashes();

        // Compare and detect changes
        foreach (var obj in hashSet)
            if (oldHashes.TryGetValue(obj.Key, out var oldHash))
            {
                if (oldHash != obj.Value)
                {
                    Console.WriteLine($"Object {obj.Key} has changed.");
                    Program._client.Rest.CreateGuildCommand(
                        ApplicationCommandOptionPropertiesList.First(x => x.Name.Value == obj.Key), 1);
                }
            }
            else
            {
                Program._client.Rest.CreateGuildCommand(
                    ApplicationCommandOptionPropertiesList.First(x => x.Name.Value == obj.Key), 1);
            }

        foreach (var key in oldHashes.Keys)
            if (!hashSet.ContainsKey(key))
                Console.WriteLine($"Object {key} was removed.");

        // Update file with new hashes
        SaveHashes(hashSet);*/
    }

    private static void SaveHashes(Dictionary<string, int> objects)
    {
        File.WriteAllText(fileInfo.FullName,
            JsonSerializer.Serialize(objects, new JsonSerializerOptions { WriteIndented = true }));
    }

    private static Dictionary<string, int> LoadHashes()
    {
        if (!fileInfo.Exists) return new Dictionary<string, int>();
        var json = File.ReadAllText(fileInfo.FullName);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
    }
}