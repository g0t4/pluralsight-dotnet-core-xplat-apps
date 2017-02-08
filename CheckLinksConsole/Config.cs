using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace CheckLinksConsole
{
	public class Config
	{
		public static IConfigurationRoot Build()
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
	}
}