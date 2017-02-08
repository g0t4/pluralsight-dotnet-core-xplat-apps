namespace CheckLinksConsole
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;

    public class CheckLinkJob
    {
        private ILogger _Logger;

        public CheckLinkJob(ILogger<CheckLinkJob> logger)
        {
            _Logger = logger;
            _Logger.LogInformation($"{Guid.NewGuid()}");
        }

        public void Execute(string site, OutputSettings output)
        {
            Directory.CreateDirectory(output.GetReportDirectory());

            _Logger.LogInformation(200, $"Saving report to {output.GetReportFilePath()}");
            var client = new HttpClient();
            var body = client.GetStringAsync(site);
            _Logger.LogDebug(body.Result);

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