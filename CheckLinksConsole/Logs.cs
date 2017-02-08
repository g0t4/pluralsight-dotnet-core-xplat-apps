
namespace CheckLinksConsole
{
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.Logging;

	public static class Logs
	{
		public static void Init(ILoggerFactory factory, IConfiguration configuration)
		{
			//Factory.AddConsole(LogLevel.Trace, includeScopes: true);
			factory.AddConsole(configuration.GetSection("Logging"));
			factory.AddFile("logs/checklinks-{Date}.json",
				isJson: true,
				minimumLevel: LogLevel.Trace);
		}
	}
}
