using HtmlAgilityPack;
using System;
using System.Linq;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodesParserTests
    {
        [Fact]
        public void Parse()
        {
            var x = new HtmlDocument();
            x.Load("orf_Steiermark-heute_70020_episodes.html");

            var epsisodes = new OrfTvSeriesEpisodesParser().Parse(x);

            Assert.NotEmpty(epsisodes);
            Assert.Equal(7, epsisodes.Count);
            Assert.Equal(new DateTime(2022, 03, 02, 19, 00, 00), epsisodes.ToList()[0].Date);
            Assert.Equal("ORF 2", epsisodes.ToList()[0].Channel);
            Assert.Equal("18:55 Min.", epsisodes.ToList()[0].Duration);
            Assert.Equal("https://tvthek.orf.at/profile/Steiermark-heute/70020/Steiermark-heute/14126428", epsisodes.ToList()[0].DownloadUrl);

            Assert.Equal(new DateTime(2022, 2, 24, 19, 00, 00), epsisodes.ToList()[6].Date);
            Assert.Equal("ORF 2", epsisodes.ToList()[6].Channel);
            Assert.Equal("18:25 Min.", epsisodes.ToList()[6].Duration);
            Assert.Equal("https://tvthek.orf.at/profile/Steiermark-heute/70020/Steiermark-heute/14125622", epsisodes.ToList()[6].DownloadUrl);

        }
    }
}
