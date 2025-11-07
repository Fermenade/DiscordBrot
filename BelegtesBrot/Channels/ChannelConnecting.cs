using Discord.WebSocket;
using Discord;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace BelegtesBrot.Channels;

public class ChannelConnecting
{
    private string _filePath;
    
    public ChannelConnecting(string filePath)
    {
        _filePath = filePath;
    }
    
    public List<IServerMessageChannel>? ReadConnectedChannels()
    {
        string serverGuildFile = _filePath;
        if (!File.Exists(serverGuildFile)) return null;
        string e = File.ReadAllText(serverGuildFile);
        
        
        List<LinkedChannel>? x = JsonSerializer.Deserialize<List<LinkedChannel>>(e);
        if(x == null) return null;
        
        List<IServerMessageChannel> serverMessageChannels = new List<IServerMessageChannel>();
        foreach (LinkedChannel linkedChannel in x)
        {
            IServerMessageChannel instance = GetMode(linkedChannel);
            serverMessageChannels.Add(instance);
        }
        return serverMessageChannels;
    }

    IServerMessageChannel GetMode(LinkedChannel channel)
    {
        Type? loadedType = Type.GetType(channel.Channel, throwOnError: true);
        if (loadedType == null)
        {
            throw new Exception($"Unknown type: {channel.Channel}");
        }
        if (loadedType.GetCustomAttributes(typeof(MessageChannelHandler), false).Length == 0)
        {
            throw new NotImplementedException($"Not marked as mode: {channel.Channel}");
        }
        
        // Using a constructor with arguments
        IServerMessageChannel? instance = (IServerMessageChannel?)Activator.CreateInstance(
            loadedType,
            new { channel.ChannelId }); // arguments must match a ctor signature
        return instance;
    }
    public IServerMessageChannel AddCannel(SocketTextChannel channel, IServerMessageChannel messageChannel) //TODO: link this logic with a register optionArgument
    {
        LinkedChannel linkedChannel = new LinkedChannel(channel, messageChannel);
        return GetMode(linkedChannel);
    }
}