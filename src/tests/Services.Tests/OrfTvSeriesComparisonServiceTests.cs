using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesComparisonServiceTests
    {
        private OrfTvSeriesComparisonService sut = new OrfTvSeriesComparisonService();

        [Fact]
        public void Compare_UsesIdOnly()
        {
            var left = new List<OrfTvSeries>()
            {
                new OrfTvSeries(){Id="1",
                Title="A"
                }
            };

            var right = new List<OrfTvSeries>()
            {
                new OrfTvSeries(){
                    Id="1",
                Title="B"
                }
            };

            var result = sut.Compare(left, right);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Equal("1", result.First().Value.Id);
            Assert.Equal(ComparisonResult.ExistsOnBothSides, result.First().Key);
        }
    }
}
