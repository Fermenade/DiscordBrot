using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DGruppensuizidBot;

public static class Serverstuff
{
    public static string PrefixPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"serverStuff\" : @"serverStuff/";//TODO:Read all the defaultpaths from file
    public static string PathActivities = $"{PrefixPath}activities.txt";
    public static string PathCommands = $"{PrefixPath}alphabetMessages.txt";
    public static string PathToken = $"{PrefixPath}token.txt";
    public static string ServerPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
        @$"{Serverstuff.PrefixPath}\MinecraftServer\"
        : $"{Serverstuff.PrefixPath}/MinecraftServer/";
    public static string ScriptPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? @$".\{PrefixPath}BMC_Server\start.ps1"
        : @$"./{PrefixPath}BMC_Server/start.sh"; //TODO: Make method that looks up the OS correct form
    public static string Scorefilepath = $"{PrefixPath}DiscordUserData.json"; //TODO: Rewrite AlphabetScore to UserData
    
    /// <summary>
    /// Shutdowntimer time in Seconds
    /// </summary>
    public static ushort ShutdownTime = 60 * 5;
#if DEBUG
    public static readonly ulong _TBoardGeneral = 1352363887441477744;
    public static ulong _ThreadAlphabetBack = 1352364208343355534;
    public static ulong _TBoardCommands = 1352364391340834857;
#elif RELEASE
    public static readonly ulong _TBoardGeneral = 849240846125367378;
    public static ulong _ThreadAlphabetBack = 1215011525195075636;
    public static ulong _TBoardChernobil = 1186307797453918259;
    public static ulong _TBoardCommands = 1341062745306431518;
#endif
    static Serverstuff()
    {
        if (!Directory.Exists(PrefixPath))
        {
            Directory.CreateDirectory(PrefixPath);
        }
    }

    
}