
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
                UserCommand userCommand;
                try
                {
                    userCommand = new (Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                CommandManager.ExecuteCommand(userCommand);
            }
        }
    }
}