using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace CheckLinksConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var config = new Config(args);
			Directory.CreateDirectory(config.Output.GetReportDirectory());
			Console.WriteLine($"Saving report to {config.Output.GetReportFilePath()}");
			var client = new HttpClient();
			var body = client.GetStringAsync(config.Site);
			Console.WriteLine(body.Result);

			Console.WriteLine();
			Console.WriteLine("Links");
			var links = LinkChecker.GetLinks(body.Result);
			links.ToList().ForEach(Console.WriteLine);
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