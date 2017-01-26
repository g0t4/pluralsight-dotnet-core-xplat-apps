using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace CheckLinksConsole
{
	using Microsoft.Extensions.Logging;

	class Program
	{
		static void Main(string[] args)
		{
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
}
