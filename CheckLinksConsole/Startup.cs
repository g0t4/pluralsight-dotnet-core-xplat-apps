using System;
using System.Linq;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheckLinksConsole
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(c => c.UseMemoryStorage());
            services.AddTransient<CheckLinkJob>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            var config = new Config();
            Logs.Factory = loggerFactory;
            Logs.Init(config.ConfigurationRoot);

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<CheckLinkJob>("check-link",
                j => j.Execute(config.Site),
                Cron.Minutely);
        }
    }
}