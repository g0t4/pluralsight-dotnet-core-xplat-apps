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
        private Config _Config;

        public void ConfigureServices(IServiceCollection services)
        {
            _Config = new Config();

            services.AddHangfire(c => c.UseMemoryStorage());
            services.AddTransient<CheckLinkJob>();
            services.Configure<OutputSettings>(_Config.ConfigurationRoot.GetSection("output"));
            services.Configure<SiteSettings>(_Config.ConfigurationRoot);
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            Logs.Factory = loggerFactory;
            Logs.Init(_Config.ConfigurationRoot);

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<CheckLinkJob>("check-link",
                j => j.Execute(_Config.Site),
                Cron.Minutely);
        }
    }
}