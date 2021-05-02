using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace AustrianTvScrapper.Services.IO
{
    public class OrfTvSeriesSubscriptionReaderWriterTests
    {
        [Fact]
        public void ReadWrite()
        {
            var data = new List<OrfTvSeriesSubscription>()
            {
                new OrfTvSeriesSubscription()
                {
                    OrfTvSeriesId = "2",
                    Name = "bla"
                },
                new OrfTvSeriesSubscription()
                {
                    OrfTvSeriesId = "snap4",
                    Name = "kilo"
                },
            };

            var dataPath = Guid.NewGuid().ToString() + ".json";

            var writer = new OrfTvSeriesSubscriptionWriter();
            writer.Save(dataPath, data);

            Assert.True(File.Exists(dataPath));

            try
            {
                var reader = new OrfTvSeriesSubscriptionReader();
                var loadedData = reader.Load(dataPath);

                Assert.NotNull(loadedData);
                Assert.Equal(2, loadedData.Count);
                Assert.Contains(loadedData, x => x.OrfTvSeriesId == "2");
                Assert.Contains(loadedData, x => x.OrfTvSeriesId == "snap4");
            }
            finally
            {
                File.Delete(dataPath);
            }
        }
    }
}
