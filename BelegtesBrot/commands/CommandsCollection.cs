using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;
using BelegtesBrot.BMC_Server;

namespace BelegtesBrot.commands;

public class CommandsCollection
{
    public RootCommand root = new RootCommand();
    public CommandsCollection(List<IServerMessageChannel> serverMessageChannels,PermissionManager permissionManager)
    {
        root.Add(new ServerCommand());
        root.Add(new Permission(permissionManager));
        root.Add(new SetMode(serverMessageChannels));
    }
}
public class ServerCommand : Command
{
    private readonly HigherMc _server;

    public ServerCommand() : base("server")
    {
        _server =  new HigherMc();
        Command startCommand = new Command("start")
        { };
        startCommand.SetAction(_ => StartServer());
        this.Add(startCommand);
        
        Command stopCommand = new Command("stop");
        stopCommand.SetAction(_ => StopServer());
        this.Add(stopCommand);
    }

    private void StartServer()
    {
        _server.StartServer();
    }

    private void StopServer()
    {
        throw new NotImplementedException();
    }
}
public class Permission : Command
{
    PermissionManager _permissionManager;
    public Permission(PermissionManager permissionManager) : base("perm")
    {
        _permissionManager = permissionManager;
        
        Command addPermission = new Command("add");
        addPermission.SetAction(AddPermission);
        Argument command = new Argument<string>("command");
        
        this.Add(addPermission);
        
        Command rmPermission = new Command("rm");
        rmPermission.SetAction(RemovePermission);
        this.Add(rmPermission);
    }

    private void AddPermission(ParseResult parseResult)
    {
        _permissionManager.Add();
        throw new NotImplementedException();
    }
    private void RemovePermission(ParseResult parseResult)
    {
        _permissionManager.Remove();
        throw new NotImplementedException();
    }
}

public class SetMode : Command
{
    private List<IServerMessageChannel> _serverMessageChannels;
    public SetMode(List<IServerMessageChannel> serverMessageChannels) : base("mode")
    {
        _serverMessageChannels = serverMessageChannels;
        
        Command subscribe = new Command("subscribe");
        subscribe.SetAction(Subscribe);
    
        Command unsubcribe = new Command("unsubscribe");   
        unsubcribe.SetAction(Unsubscribe);
    }
    Argument<ulong> channel = new("channel")
    {
        CustomParser = argumentResult =>
        {
            string Value = argumentResult.Tokens[0].Value;
            if (!GlobalCommandTools.GetChannel(Value, out ulong channelId))
            {
                argumentResult.AddError($"{Value} was not channel");
                return 0;
            }

            return channelId;
        }
    };
    Argument<string> channelHandler = new("handler")
    {
        CustomParser = argumentResult =>
        {
            string Value = argumentResult.Tokens[0].Value;
            if (!GlobalCommandTools.GetChannel(Value, out ulong channelId))
            {
                argumentResult.AddError($"{Value} was not channel");
                return 0;
            }

            return channelId;
        }
    };
    private void Subscribe(ParseResult parseResult)
    {
        parseResult.CommandResult.GetValue(channel);
    }
    private void Unsubscribe(ParseResult parseResult)
    {
        parseResult.CommandResult.GetValue(channel);
    }
}

public static partial class GlobalCommandTools
{
    public static bool GetRole(string userString, out ulong userId)
    {
        Regex regex = RoleRegex();
        Match e = regex.Match(userString);
        userId = ulong.Parse(e.Groups["userID"].Value);
        
        return e.Success;
    }
    public static bool GetChannel(string userString, out ulong userId)
    {
        Regex regex = ChannelRegex();
        Match e = regex.Match(userString);
        userId = ulong.Parse(e.Groups["userID"].Value);
        
        return e.Success;
    }
    public static bool GetUser(string userString, out ulong userId)
    {
        Regex regex = UserRegex();
        Match e = regex.Match(userString);
        userId = ulong.Parse(e.Groups["userID"].Value);
        
        return e.Success;
    }

    [GeneratedRegex(@"\<#(?<channelID>d*)>")]
    private static partial Regex ChannelRegex();
    [GeneratedRegex(@"\<@(?<userID>d*)>")]
    private static partial Regex UserRegex();
    [GeneratedRegex(@"\<@&(?<userID>d*)>")]
    private static partial Regex RoleRegex();
}