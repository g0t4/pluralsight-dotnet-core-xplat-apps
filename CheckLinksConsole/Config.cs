using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CheckLinksConsole
{
	using System;
	using System.Linq;

	public class Config
	{
		public static IConfigurationRoot SetupConfig()
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
				.AddCommandLine(Environment.GetCommandLineArgs().Skip(1).ToArray())
				.AddEnvironmentVariables();

			return configBuilder.Build();
		}

		public string Site { get; set; }
		public OutputSettings Output { get; set; }
	}
}