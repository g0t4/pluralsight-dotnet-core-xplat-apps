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
	using Microsoft.Extensions.Options;

	class Program
	{
		static void Main(string[] args)
		{
			var host = new WebHostBuilder()
			   .UseKestrel()
			   .UseContentRoot(Directory.GetCurrentDirectory())
			   .UseStartup<Startup>()
			   .Build();

			//todo is this a good spot?
			RecurringJob.AddOrUpdate<CheckLinks>(c => c.Check(), Cron.Minutely);

			host.Run();
		}
	}

	public class CheckLinks
	{
		private readonly ILogger<CheckLinks> _Logger;
		private readonly LinkChecker _Checker;
		private readonly Config _Config;

		public CheckLinks(ILogger<CheckLinks> logger, IOptions<Config> config, LinkChecker checker)
		{
			_Logger = logger;
			_Checker = checker;
			_Config = config.Value;
		}

		public void Check()
		{
			_Logger.LogInformation("\n\n\nChecking\n\n\n");
			Directory.CreateDirectory(_Config.Output.GetReportDirectory());

			_Logger.LogInformation(200, $"Saving report to {_Config.Output.GetReportFilePath()}");
			var client = new HttpClient();
			var body = client.GetStringAsync(_Config.Site);
			_Logger.LogDebug(body.Result);

			var links = _Checker.GetLinks(_Config.Site, body.Result);

			var checkedLinks = _Checker.CheckLinks(links);
			using (var file = File.CreateText(_Config.Output.GetReportFilePath()))
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


	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			// todo args console
			Configuration = Config.SetupConfig(new string[0]);
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddHangfire(x => x.UseMemoryStorage());
			services.AddTransient<CheckLinks>();
			services.AddTransient<LinkChecker>();
			services.Configure<Config>(Configuration);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddFile("logs/checklinks-{Date}.json",
				isJson: true,
				minimumLevel: LogLevel.Trace);

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
