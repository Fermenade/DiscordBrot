using BelegtesBrot;
using BelegtesBrot.MinecraftServer;
using Timer = System.Timers.Timer;

namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task Test1()
    {
        Assert.Pass();
        return;
        Session session = new Session(1234);
        DirectoryInfo directoryInfo = new(session.BaseFolder.DirectoryInfo + "/MinecraftServer");
        directoryInfo.Create();
        MinecraftServer minecraftServer = new MinecraftServer(directoryInfo,session);
        minecraftServer.StartServer();
        await Task.Delay(-1);
    }

    [Test]
    public async Task Test2()
    {
        Timer timer = new()
        {
            Interval = 1000,
            AutoReset = false,
            Enabled = false,
        };
        timer.Elapsed += (sender, args) =>
        {
            Console.WriteLine(DateTime.Now);
            Assert.Pass();
        };
        timer.Start();
        Task.Delay(-1).Wait();
        /*Session session = new Session(1234);
        HallOfFame hallOfFame = new HallOfFame(session.BaseFolder.DirectoryInfo);
        var e =hallOfFame.GetEntries();
        foreach (var VARIABLE in e)
        {
            Console.WriteLine(VARIABLE);
        }*/
    }
}