using System.CommandLine.Parsing;
using System.Windows.Input;
using Discord;

namespace BelegtesBrot.commands;

public class UserCommand(IMessage message) : IUserCommand
{
    public string UserInput => message.Content;
    public ulong UserId => message.Author.Id;
}