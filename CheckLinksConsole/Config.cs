using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CheckLinksConsole
{
	public class Config
	{
		public Config(string[] args)
		{
			var inMemory = new Dictionary<string, string>
			{
				{"site", "https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps"},
				{"output:folder", "reports"},
			};
			var configBuilder = new ConfigurationBuilder()
				.AddInMemoryCollection(inMemory)
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("checksettings.json", true)
				.AddCommandLine(args)
				.AddEnvironmentVariables();

			var configuration = configBuilder.Build();
			Site = configuration["site"];
			Output = configuration.GetSection("output").Get<OutputSettings>();
		}

		public string Site { get; set; }
		public OutputSettings Output { get; set; }	
	}
}