using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace CheckLinksConsole
{
	using Hangfire;
	using Hangfire.MemoryStorage;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	class Program
	{
		static void Main(string[] args)
		{
			//GlobalConfiguration.Configuration.UseMemoryStorage();

			var host = new WebHostBuilder()
			   .UseKestrel()
			   .UseContentRoot(Directory.GetCurrentDirectory())
			   .UseStartup<Startup>()
			   .Build();
			RecurringJob.AddOrUpdate<CheckLinks>(c => c.Check(), Cron.Minutely);

			host.Run();

			return;
			var config = new Config(args);
			Logs.Init(config.ConfigurationRoot);
			var logger = Logs.Factory.CreateLogger<Program>();
			Directory.CreateDirectory(config.Output.GetReportDirectory());

			logger.LogInformation(200, $"Saving report to {config.Output.GetReportFilePath()}");
			var client = new HttpClient();
			var body = client.GetStringAsync(config.Site);
			logger.LogDebug(body.Result);

			var links = LinkChecker.GetLinks(config.Site, body.Result);

			var checkedLinks = LinkChecker.CheckLinks(links);
			using (var file = File.CreateText(config.Output.GetReportFilePath()))
			using (var linksDb = new LinksDb())
			{
				foreach (var link in checkedLinks.OrderBy(l => l.Exists))
				{
					var status = link.IsMissing ? "missing" : "OK";
					file.WriteLine($"{status} - {link.Link}");
					linksDb.Links.Add(link);
				}
				linksDb.SaveChanges();
			}
		}
	}

	public class CheckLinks
	{
		private readonly ILogger<CheckLinks> _Logger;

		public CheckLinks(ILogger<CheckLinks> logger)
		{
			_Logger = logger;
		}

		public void Check()
		{
			_Logger.LogInformation("\n\n\nChecking\n\n\n");
		}
	}


	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddHangfire(x => x.UseMemoryStorage());
			services.AddTransient<CheckLinks>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseHangfireServer();
			app.UseHangfireDashboard();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}

}
