using HtmlAgilityPack;
using System;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodesParserTests
    {
        [Fact]
        public void Test1()
        {
            var x = new HtmlDocument();
            x.Load("orf_Steiermark-heute_70020_episodes.html");

            var epsisodes = OrfTvSeriesEpisodesParser.Parse(x);

            Assert.NotEmpty(epsisodes);
        }
    }
}
