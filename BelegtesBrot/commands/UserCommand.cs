using System.CommandLine.Parsing;
using System.Windows.Input;
using Discord;
using Discord.WebSocket;

namespace BelegtesBrot.commands;

public class UserCommand(IMessage message, PermissionManager permissionManager) : IUserCommand
{
    public string UserInput => message.Content;

    public object Author => _author is not IUser
        ? throw new ArgumentException($"Author was not of type '{nameof(IUser)}'")
        : _author;

    private object _author => message.Author;

    public PermissionManager PermissionManager => permissionManager;
    public bool HasAccess(Guid permissionId)
    {
        if (Author is SocketGuildUser socketGuildUser)
        {
            foreach (SocketRole role in socketGuildUser.Roles)
            {
                return PermissionManager.HasPermission(role.Id,permissionId);
            }
        }
        if (Author is IUser user)
        {
            return PermissionManager.HasPermission(user.Id, permissionId);
        }

        throw new ArgumentException($"Author was not of type {nameof(IUser)}");
    }
}