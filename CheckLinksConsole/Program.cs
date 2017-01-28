using System;

namespace CheckLinksConsole
{
	using Hangfire;
	using Hangfire.MemoryStorage;

	class Program
	{
		static void Main(string[] args)
		{
			var config = new Config(args);
			Logs.Init(config.ConfigurationRoot);

			GlobalConfiguration.Configuration.UseMemoryStorage();

			RecurringJob.AddOrUpdate<CheckLinkJob>("check-link", j => j.Execute(config.Site, config.Output), Cron.Minutely);
			RecurringJob.Trigger("check-link");

			using (var server = new BackgroundJobServer())
			{
				Console.WriteLine("Hangfire Server started. Press any key to exit...");
				Console.ReadKey();
			}

		}
	}
}
