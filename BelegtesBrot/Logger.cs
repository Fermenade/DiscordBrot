namespace BelegtesBrot;

public class Logger
{
    private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    private static Lock _lock = new();
    static Logger()
    {
        if (!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
    }

    public static Task LogMessage(string message)
    {
        lock (_lock)
        {
            var fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            var filePath = Path.Combine(LogDirectory, fileName);

            // Append the message with timestamp
            string logMessage = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
            Console.Write(logMessage);

            File.AppendAllTextAsync(filePath, logMessage);
        }
        
        // Clean up old files
        DeleteOldFiles();
        return Task.CompletedTask;
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