namespace CheckLinksConsole
{
	using System.IO;
	using System.Linq;
	using System.Net.Http;
	using Microsoft.Extensions.Logging;

	public class CheckLinkJob
	{
		public void Execute(string site, OutputSettings output)
		{
			var logger = Logs.Factory.CreateLogger<CheckLinkJob>();
			Directory.CreateDirectory(output.GetReportDirectory());

			logger.LogInformation(200, $"Saving report to {output.GetReportFilePath()}");
			var client = new HttpClient();
			var body = client.GetStringAsync(site);
			logger.LogDebug(body.Result);

			var links = LinkChecker.GetLinks(site, body.Result);

			var checkedLinks = LinkChecker.CheckLinks(links);
			using (var file = File.CreateText(output.GetReportFilePath()))
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