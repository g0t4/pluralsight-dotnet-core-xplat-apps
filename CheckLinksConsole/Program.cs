using System;
using System.Linq;
using System.Net.Http;

class Program
{
	static void Main(string[] args)
	{
		var site = "https://g0t4.github.io/pluralsight-dotnet-core-xplat-apps";
		var client = new HttpClient();
		var body = client.GetStringAsync(site);
		Console.WriteLine(body.Result);

		Console.WriteLine();
		Console.WriteLine("Links");
		var links = LinkChecker.GetLinks(body.Result);
		links.ToList().ForEach(Console.WriteLine);
	}
}