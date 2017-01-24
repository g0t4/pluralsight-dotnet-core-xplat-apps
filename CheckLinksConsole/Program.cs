using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

class Program
{
	public class OutputSettings
	{
		public OutputSettings()
		{
			File = "file.txt";
		}
		public string Folder { get; set; }
		public string File { get; set; }
	}

	static void Main(string[] args)
	{
		var inMemory = new Dictionary<string, string>
		{
			{"site", "https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps" },
			{"output:folder", "reports" },
		};
		var configBuilder = new ConfigurationBuilder()
			.AddInMemoryCollection(inMemory)
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("checksettings.json",true)
			.AddCommandLine(args)
			.AddEnvironmentVariables();

		var configuration = configBuilder.Build();
		var site = configuration["site"];

		var outputSettings = new OutputSettings();
		configuration.GetSection("output").Bind(outputSettings);
		var currentDirectory = Directory.GetCurrentDirectory();
		var outputPath = Path.Combine(currentDirectory, outputSettings.Folder, outputSettings.File);
		var directory = Path.GetDirectoryName(outputPath);
		Directory.CreateDirectory(directory);
		Console.WriteLine($"Saving report to {outputPath}");
		var client = new HttpClient();
		var body = client.GetStringAsync(site);
		Console.WriteLine(body.Result);

		Console.WriteLine();
		Console.WriteLine("Links");
		var links = LinkChecker.GetLinks(body.Result);
		links.ToList().ForEach(Console.WriteLine);
		// write out links
		//File.WriteAllLines(outputPath, links);
		var checkedLinks = LinkChecker.CheckLinks(links);
		using (var file = File.CreateText(outputPath))
		{
			foreach (var link in checkedLinks.OrderBy(l => l.Exists))
			{
				var status = link.IsMissing ? "missing" : "OK";
				file.WriteLine($"{status} - {link.Link}");
			}
		}
	}
}