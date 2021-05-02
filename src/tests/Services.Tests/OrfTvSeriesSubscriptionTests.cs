using System;
using System.Collections.Generic;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSubscriptionTests
    {
        [Fact]
        public void AddSubscriptionIfSubscriptionForSeriesNotExists()
        {

            var existingSubscriptions = new List<OrfTvSeriesSubscription>()
            {
                new OrfTvSeriesSubscription()
                {
                    OrfTvSeriesId = "1"
                },
            };

            var toWrite = OrfTvSeriesSubscription.AddSubscription(existingSubscriptions, new OrfTvSeriesSubscription() { OrfTvSeriesId = "2" });

            Assert.NotNull(toWrite);
            Assert.Equal(2, toWrite.Count);
        }

        [Fact]
        public void DoesNotAddSubscriptionIfSubscriptionForSeriesExists()
        {

            var existingSubscriptions = new List<OrfTvSeriesSubscription>()
            {
                new OrfTvSeriesSubscription()
                {
                    OrfTvSeriesId = "1"
                },
                new OrfTvSeriesSubscription()
                {
                    OrfTvSeriesId = "2"
                },
            };

            var toWrite =                 OrfTvSeriesSubscription.AddSubscription(existingSubscriptions, new OrfTvSeriesSubscription() { OrfTvSeriesId = "1" });

            Assert.NotNull(toWrite);
            Assert.Equal(2, toWrite.Count);
        }
    }
}
