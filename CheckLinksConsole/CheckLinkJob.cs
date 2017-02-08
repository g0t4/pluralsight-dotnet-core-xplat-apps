namespace CheckLinksConsole
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class CheckLinkJob
    {
        private ILogger _Logger;
		private OutputSettings _Output;
		private SiteSettings _Site;
        private LinkChecker _LinkChecker;

        public CheckLinkJob(ILogger<CheckLinkJob> logger, 
			IOptions<OutputSettings> outputOptions,
			IOptions<SiteSettings> siteOptions,
            LinkChecker linkChecker)
        {
            _Logger = logger;
            _Logger.LogInformation($"{Guid.NewGuid()}");
			_Output = outputOptions.Value;
			_Site = siteOptions.Value;
            _LinkChecker = linkChecker;
        }

        public void Execute()
        {
            Directory.CreateDirectory(_Output.GetReportDirectory());

            _Logger.LogInformation(200, $"Saving report to {_Output.GetReportFilePath()}");
            var client = new HttpClient();
            var body = client.GetStringAsync(_Site.Site);
            _Logger.LogDebug(body.Result);

            var links = _LinkChecker.GetLinks(_Site.Site, body.Result);

            var checkedLinks = _LinkChecker.CheckLinks(links);
            using (var file = File.CreateText(_Output.GetReportFilePath()))
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