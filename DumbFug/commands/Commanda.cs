using Discord;
internal class Commanda
{
    // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
    internal SlashCommandBuilder CServer = new SlashCommandBuilder().WithName("server").WithDescription("Spricht den Server an.")
    .AddOptions(new SlashCommandOptionBuilder().WithName("query: ").AddChoice("start", 1).AddChoice("stop", 2).AddChoice("reload", 3).AddChoice("stats", 4).WithType(ApplicationCommandOptionType.Integer));

    SlashCommandBuilder CWelp;


    //[Command("ping"), Description("Pings, for debuging")]
    //public async Task Ping(CommandContext ctx)
    //{
    //    await ctx.RespondAsync($"Pong! {ctx.Client.Ping}ms");
    //}
    ////[Command("pingserver")]
    ////public async Task Ping(CommandContext ctx)
    ////{
    ////   server overload
    ////}
    //[Command("start")]
    //public async Task StartServer(CommandContext ctx)
    //{
    //    await ctx.RespondAsync("*you call the server*");
    //    Thread.Sleep(1500);
    //    await ctx.RespondAsync("...");
    //    Thread.Sleep(1000);
    //    await ctx.RespondAsync("But nobody came.");

    //    //public async Task ServStart(CommandContext ctx)
    //    //{
    //    //    await ctx.RespondAsync("Try to get it up!");
    //    //    try
    //    //    {
    //    //        System.Diagnostics.Process.Start(new ProcessStartInfo()); //server start
    //    //        //portforwarding selber?d
    //    //    }
    //    //    catch
    //    //    {
    //    //        await ctx.RespondAsync("There was an error \n（；´д｀）ゞ");
    //    //    }
    //    //}
    //}
}