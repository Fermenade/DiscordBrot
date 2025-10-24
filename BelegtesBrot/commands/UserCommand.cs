using System.CommandLine;
using System.Windows.Input;
using Discord;

namespace BelegtesBrot.commands;

public interface IUserCommand
{
    string UserInput { get;}
    IUser User { get; }
}

public class UserCommand(IMessage message) : IUserCommand
{
    public string UserInput => message.Content;
    public IUser User => message.Author;
}