using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BelegtesBrot;

namespace BelegtesBrot.BMC_Server;

public class MCServer
{
    private static MCServer? mcServer;
    private const string ServerPath = $"{Info.InfoFolder}/MinecraftServer";
    private PlayerManager _playerManager = new PlayerManager();
    private Process _process;
    private ProcessStartInfo processInfo = new ProcessStartInfo
    {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell.exe" : @"/bin/bash",

        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"-ExecutionPolicy Bypass -File \"{ServerPath}/start.ps1\""
            : $"{ServerPath}/start.sh",
        RedirectStandardOutput = true,
        RedirectStandardInput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    };

    public bool Running => !_process.HasExited;
    public static MCServer GetMcServer()
    {
        if (mcServer != null) return mcServer;

        return mcServer = new MCServer();
    }

    private MCServer()
    {
        _process = new Process()
        {
            StartInfo = processInfo,
        };
        mcServer = this;
    }
    public void StartProcess()
    {
        _process.Start();
    }
}

public class HigherMC
{
    void StopServer()
    {
        //write to process "stop"
    }

    void ListPlayer()
    {

    }
    void 
}