using System.Text;
using BelegtesBrot.Channels;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.commands;

public class ModeCommand
{
    private readonly CommandSession _commandSession;

    internal ModeCommand(CommandSession session)
    {
        _commandSession = session;
    }

    public async Task ModeCommandExecute(SocketSlashCommand command)
    {
        var subCommand = command.Data.Options.First();
        Server server;
        try
        {
            server = (Server)_commandSession._session;
        }
        catch (InvalidCastException e)
        {
            Console.WriteLine(e);
            return;
        }

        switch (subCommand.Name)
        {
            case "set":
                var channel = (IChannel)subCommand.Options.First().Value;
                var typeName = subCommand.Options.ElementAt(1).Value.ToString();
                server.MessageChannelManager.Add(channel.Id, typeName);
                await command.RespondAsync("Mode set.");
                break;
            case "unset":
                var channele = (IChannel)subCommand.Options.First().Value;
                if (server.MessageChannelManager.LinkedChannels.All(x => x.ChannelId != channele.Id))
                {
                    await command.RespondAsync("Channel didn't have a mode connected");
                    break;
                }

                server.MessageChannelManager.Remove(channele.Id);
                await command.RespondAsync("Mode unset.");
                break;
            case "list":
                var e = ModeChannelMapper.Map;
                var sb = new StringBuilder();
                foreach (var key in e)
                {
                    sb.AppendLine($"**{key.Key}:** " + key.Value.Description);
                    server.MessageChannelManager.LinkedChannels.Where(x => x.ModeName == key.Key).ToList()
                        .ForEach(x =>
                            sb.AppendLine(Common.FormatAsChannelMention(x.ChannelId)));
                }


                await command.RespondAsync(sb.ToString());
                break;
        }
    }
}