using System;
using Xunit;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodeDirectoryProviderTests
    {
        OrfTvSeriesSubscription subscription = new OrfTvSeriesSubscription
        {
            Name = "hicks"
        };

        BaseDirectoriesConfiguration configuration = new BaseDirectoriesConfiguration
        {
            BasePath = @"C:\"
        };

        OrfTvSeries series = new OrfTvSeries
        {
            Title = "bla"
        };

        OrfTvSeriesEpisode episode = new OrfTvSeriesEpisode
        {
            Date = new DateTime(2022, 02, 22),
            Title = "some"
        };

        [Fact]
        public void GetDirectory_SubscriptionIsNull_ThrowsException()
        {
            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var ex = Assert.Throws<ArgumentNullException>(() => _ = sut.GetDirectory(null, series, episode));
            Assert.Equal("subscription", ex.ParamName);
        }

        [Fact]
        public void GetDirectory_SeriesIsNull_ThrowsException()
        {
            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var ex = Assert.Throws<ArgumentNullException>(() => _ = sut.GetDirectory(subscription, null, episode));
            Assert.Equal("series", ex.ParamName);
        }

        [Fact]
        public void GetDirectory_EpisodeIsNull_ThrowsException()
        {
            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var ex = Assert.Throws<ArgumentNullException>(() => _ = sut.GetDirectory(subscription, series, null));
            Assert.Equal("episode", ex.ParamName);
        }

        [Fact]
        public void GetDirectory()
        {
            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var directoryInfo = sut.GetDirectory(subscription, series, episode);

            Assert.NotNull(directoryInfo);
            Assert.Equal(@"C:\2022\hicks\2022-02-22 some", directoryInfo.FullName);
        }

        [Theory]
        [InlineData('?')]
        [InlineData(':')]
        public void GetDirectory_WithSpecialCharacterInEpisodeTitle_ReplacesSpecialCharacter(char invalidChar)
        {
            episode = new OrfTvSeriesEpisode
            {
                Date = new DateTime(2022, 02, 22),
                Title = "some" + invalidChar
            };

            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var directoryInfo = sut.GetDirectory(subscription, series, episode);

            Assert.NotNull(directoryInfo);
            Assert.Equal(@"C:\2022\hicks\2022-02-22 some_", directoryInfo.FullName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetDirectory_SubscriptionHasNoName_UsesSeriesName(string subscriptionName)
        {
            subscription = new OrfTvSeriesSubscription
            {
                Name = subscriptionName
            };

            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var directoryInfo = sut.GetDirectory(subscription, series, episode);

            Assert.NotNull(directoryInfo);
            Assert.Equal(@"C:\2022\bla\2022-02-22 some", directoryInfo.FullName);
        }

        [Fact]
        public void GetDirectory_EpisodeYearIsNotCurrentYear_UsesEpisodeYear()
        {
            episode = new OrfTvSeriesEpisode
            {
                Date = new DateTime(2013, 3, 13),
                Title = "some"
            };

            var sut = new OrfTvSeriesEpisodeDirectoryProvider(configuration);

            var directoryInfo = sut.GetDirectory(subscription, series, episode);

            Assert.NotNull(directoryInfo);
            Assert.Equal(@"C:\2013\hicks\2013-03-13 some", directoryInfo.FullName);
        }
    }
}
