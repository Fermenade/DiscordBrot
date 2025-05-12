using System.Diagnostics;

namespace LogicTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Program().RunThisShit();
        }

        private void RunThisShit()
        {
            while (true)
            {
                UserCommand? command;
                string input = Console.ReadLine();
                try
                {
                    command = new UserCommand(input, null);
                    CommandManager.ExecuteCommand(command);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e);
                }
            }
        }
        // private void RunThisShit()
        // {
        //     while (true)
        //     {
        //         UserCommand? userCommand;
        //         string userInput = Console.ReadLine()!;
        //         try
        //         {
        //             //userCommand = new UserCommand(userInput!);
        //             string? exception;
        //             if (UserCommand.TryParse(userInput, out userCommand, out exception))
        //             {
        //                 CommandManager.ExecuteCommand(userCommand);
        //             }
        //             else
        //             {
        //                 Console.WriteLine(exception);
        //             }
        //         }
        //         catch (Exception e)
        //         {
        //             Console.WriteLine(e.Message);
        //         }
        //     }
        // }
    }
}