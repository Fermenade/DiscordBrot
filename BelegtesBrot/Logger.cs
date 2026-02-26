namespace BelegtesBrot;

public class Logger
{
    private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

    static Logger()
    {
        if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
    }

    public static async Task LogMessage(string message)
    {
        var fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
        var filePath = Path.Combine(LogDirectory, fileName);

        // Append the message with timestamp
        Console.WriteLine(message);

        await File.AppendAllTextAsync(filePath, $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}");

        // Clean up old files
        DeleteOldFiles();
    }

    private static void DeleteOldFiles()
    {
        var files = Directory.GetFiles(LogDirectory, "*.log");

        foreach (var file in files)
        {
            var creationTime = File.GetCreationTime(file);
            if (creationTime < DateTime.Now.AddDays(-5)) File.Delete(file);
        }
    }
}