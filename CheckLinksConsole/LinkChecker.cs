using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CheckLinksConsole;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations.Schema;

public class LinkChecker
{
	protected static readonly ILogger<LinkChecker> Logger = Logs.Factory.CreateLogger<LinkChecker>();

	public static IEnumerable<string> GetLinks(string link, string page)
	{
		var htmlDocument = new HtmlDocument();
		htmlDocument.LoadHtml(page);
		var originalLinks = htmlDocument.DocumentNode.SelectNodes("//a[@href]")
			.Select(n => n.GetAttributeValue("href", string.Empty))
			.ToList();
		using (Logger.BeginScope($"Getting links from {link}"))
		{
			originalLinks.ForEach(l => Logger.LogTrace(100, "Original link: {link}", l));
		}
		var links = originalLinks
			.Where(l => !String.IsNullOrEmpty(l))
			.Where(l => l.StartsWith("http"));
		return links;
	}

	public static IEnumerable<LinkCheckResult> CheckLinks(IEnumerable<string> links)
	{
		var all = Task.WhenAll(links.Select(CheckLink));
		return all.Result;
	}

	public static async Task<LinkCheckResult> CheckLink(string link)
	{
		var result = new LinkCheckResult();
		result.Link = link;
		using (var client = new HttpClient())
		{
			var request = new HttpRequestMessage(HttpMethod.Head, link);
			try
			{
				var response = await client.SendAsync(request);
				result.Problem = response.IsSuccessStatusCode
					? null
					: response.StatusCode.ToString();
				return result;
			}
			catch (HttpRequestException exception)
			{
				Logger.LogTrace(0, exception, "Failed to retrieve {link}", link);
				result.Problem = exception.Message;
				return result;
			}
		}
	}
}

public class LinkCheckResult
{
    [Column("ID")]
	public int Id { get; set; }
	public bool Exists => String.IsNullOrWhiteSpace(Problem);
	public bool IsMissing => !Exists;
	public string Problem { get; set; }
	public string Link { get; set; }
	public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
}
