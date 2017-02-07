
namespace CheckLinksConsole
{
    using System;
    using System.IO;
    using Hangfire;
    using Microsoft.AspNetCore.Hosting;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config(args);
            Logs.Init(config.ConfigurationRoot);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseLoggerFactory(Logs.Factory)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            //RecurringJob.AddOrUpdate<CheckLinkJob>("check-link", j => j.Execute(config.Site, config.Output), Cron.Minutely);
            //RecurringJob.Trigger("check-link");
            RecurringJob.AddOrUpdate(() => Console.Write("Simple!"), Cron.Minutely);

            host.Run();
        }
    }
}
