using BelegtesBrot;
using BelegtesBrot.MinecraftServer;

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
        Session session = new Session(1234);
        DirectoryInfo directoryInfo = new(session.BaseFolder.DirectoryInfo + "/MinecraftServer");
        directoryInfo.Create();
        MinecraftServer minecraftServer = new MinecraftServer(directoryInfo,session);
        minecraftServer.StartServer();
        await Task.Delay(-1);
        Assert.Pass();
    }
}