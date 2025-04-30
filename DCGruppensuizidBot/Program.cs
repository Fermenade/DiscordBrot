namespace DGruppensuizidBot;

public class Program
{
    static void Main(string[] args)
    {
        try
        {
            new Programm().RunBotAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            PanicMode(ex);
        }

        void PanicMode(Exception ex)
        {
            //Save all info to files
            //Save log to file
        }
    }
}