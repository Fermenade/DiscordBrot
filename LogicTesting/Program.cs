using System.Diagnostics;
using DGruppensuizidBot.AlphabetThread;

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
            AlphabetCachedMessages<Combination, char> cache = new AlphabetCachedMessages<Combination, char>();
            Console.WriteLine("Initalized, first combination");
            while (true)
            {
                Console.WriteLine(cache.Add(new AlphabetMessage<Combination, char>(Console.ReadLine())));
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