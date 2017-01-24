using System;
using System.Linq;
using Xunit;

namespace Tests
{
	public class Tests
	{
		[Fact]
		public void WithoutHttpAtStartOfLink_NoLinks()
		{
			var links = LinkChecker.GetLinks("", "<a href=\"google.com\" />");
			Assert.Equal(links.Count(), 0);
		}

		[Fact]
		public void WithHttpAtStartOfLink_LinkParses()
		{
			var links = LinkChecker.GetLinks("", "<a href=\"http://google.com\" />");
			Assert.Equal(links.Count(), 1);
			Assert.Equal(links.First(), "http://google.com");
		}
	}
}