
namespace CheckLinksConsole
{
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;

	public static class Logs
	{
		public static LoggerFactory Factory = new LoggerFactory();

		public static void Init(IConfiguration configuration)
		{
			//Factory.AddConsole(LogLevel.Trace, includeScopes: true);
			Factory.AddConsole(configuration.GetSection("Logging"));
			Factory.AddFile("logs/checklinks-{Date}.json",
				isJson: true,
				minimumLevel: LogLevel.Trace);
		}
	}
}
