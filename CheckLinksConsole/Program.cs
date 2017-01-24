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
			var logger = Logs.Factory.CreateLogger<Program>();
			var config = new Config(args);
			Directory.CreateDirectory(config.Output.GetReportDirectory());
			
			logger.LogInformation($"Saving report to {config.Output.GetReportFilePath()}");
			var client = new HttpClient();
			var body = client.GetStringAsync(config.Site);
			logger.LogDebug(body.Result);

			var links = LinkChecker.GetLinks(body.Result);
			return;
			// write out links
			var checkedLinks = LinkChecker.CheckLinks(links);
			using (var file = File.CreateText(config.Output.GetReportFilePath()))
			{
				foreach (var link in checkedLinks.OrderBy(l => l.Exists))
				{
					var status = link.IsMissing ? "missing" : "OK";
					file.WriteLine($"{status} - {link.Link}");
				}
			}
		}
	}
}