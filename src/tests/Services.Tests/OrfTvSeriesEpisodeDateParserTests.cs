using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodeDateParserTests
    {
        [Fact]
        public void Parse_CentralEuropeanSummerTime()
        {
            var parsedDateTime = OrfTvSeriesEpisodeDateParser.Parse("2022-03-02CEST19:00:00");
            Assert.Equal(new System.DateTime(2022, 03, 2, 19, 0, 0), parsedDateTime);
        }

        [Fact]
        public void Parse_CentralEuropeanTime()
        {
            var parsedDateTime = OrfTvSeriesEpisodeDateParser.Parse("2021-05-09CET19:42:13");
            Assert.Equal(new System.DateTime(2021, 5, 9, 19, 42, 13), parsedDateTime);
        }
    }
}
