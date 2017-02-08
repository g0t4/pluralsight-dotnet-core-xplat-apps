
namespace CheckLinksConsole
{
    using System;
    using System.IO;
    using Hangfire;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;

    class Program
    {
        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            RecurringJob.Trigger("check-link");

            host.Run();
        }
    }
}
