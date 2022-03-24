using System.IO;

namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesEpisodeDirectoryProvider
    {
        DirectoryInfo GetDirectory(OrfTvSeriesSubscription subscription, OrfTvSeries series, OrfTvSeriesEpisode episode);
    }
}
