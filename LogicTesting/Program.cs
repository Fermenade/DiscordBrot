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
                string input = Console.ReadLine();
                if (input == "delete")
                {
                    Console.WriteLine("Deleted Message from cache");
                }
                Console.WriteLine(cache.Add(new AlphabetMessage<Combination, char>(input)));
            }
        }
    }
}