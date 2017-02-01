
namespace CheckLinksConsole
{
    using System;
    using System.IO;
    using Hangfire;
    using Hangfire.MemoryStorage;
    using Microsoft.AspNetCore.Hosting;

    class Program
    {
        static void Main(string[] args)
        {
            var config = new Config(args);
            Logs.Init(config.ConfigurationRoot);

            GlobalConfiguration.Configuration.UseMemoryStorage();

            RecurringJob.AddOrUpdate<CheckLinkJob>("check-link", j => j.Execute(config.Site, config.Output), Cron.Minutely);
            RecurringJob.Trigger("check-link");

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            using (var server = new BackgroundJobServer())
            {
                Console.WriteLine("Hangfire Server started.");
                //Console.ReadKey();
                host.Run();
            }

        }
    }
}
