
namespace CheckLinksConsole
{
	using Microsoft.Extensions.Logging;

	public static class Logs
	{
		public static LoggerFactory Factory = new LoggerFactory();

		static Logs()
		{
			Factory.AddConsole(LogLevel.Trace, includeScopes: true);
			Factory.AddFile("logs/checklinks-{Date}.txt", minimumLevel: LogLevel.Trace);
		}
	}
}
