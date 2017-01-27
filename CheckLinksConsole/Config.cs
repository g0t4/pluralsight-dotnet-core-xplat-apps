using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CheckLinksConsole
{
	public class Config
	{
		public static IConfigurationRoot SetupConfig(string[] args)
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

			return configBuilder.Build();
		}

		public string Site { get; set; }
		public OutputSettings Output { get; set; }
	}
}