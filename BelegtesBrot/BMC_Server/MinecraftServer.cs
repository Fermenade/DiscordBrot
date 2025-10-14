using DGruppensuizidBot.BMC_Server;
using System.Diagnostics;
using System.Runtime.InteropServices;

public class MCServer
{
    private PlayerManager _playerManager = new PlayerManager();
    private Process _process;
    private ProcessStartInfo processInfo = new ProcessStartInfo
    {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : @"/bin/bash",

        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"-ExecutionPolicy Bypass -File \"{Serverstuff.ScriptPath}\""
            : Serverstuff.ScriptPath,
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };
    public MCServer()
    {
        _process = new Process()
        {

        };
    }

    public void StartServer()
    {

    }
}