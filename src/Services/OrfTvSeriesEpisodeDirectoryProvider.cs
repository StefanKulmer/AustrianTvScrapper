using System;
using System.IO;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodeDirectoryProvider
    {
        private const string BasePath = @"L:\03 Movies\Tv";
        private const string DefaultEpisodeNameFormat = "#Date #Title";
        private const string DefaultSubDirectory = "#Year\\#Name";

        public string GetDirectory(OrfTvSeriesSubscription subscription, OrfTvSeries series, OrfTvSeriesEpisode episode)
        {
            var episodeNameFormat = subscription.EpisodeNameFormat ?? DefaultEpisodeNameFormat;
            var episodeDirectory = episodeNameFormat.Replace("#DATE", string.Format("{0:yyyy-MM-dd}", episode.Date), StringComparison.OrdinalIgnoreCase);

            var title = episode.Title;
            if (subscription.EpisodeNameRemovals != null)
            {
                foreach (var replace in subscription.EpisodeNameRemovals)
                {
                    title = title.Replace(replace, "", StringComparison.OrdinalIgnoreCase);
                }
            }
            episodeDirectory = episodeDirectory.Replace("#TITLE", title, StringComparison.OrdinalIgnoreCase);
            episodeDirectory = episodeDirectory.Replace("#SERIES", series.Title, StringComparison.OrdinalIgnoreCase);

            var seriesDirectory = subscription.DownloadSubDirectory ?? DefaultSubDirectory;
            seriesDirectory = seriesDirectory.Replace("#NAME", subscription.Name ?? series.Title, StringComparison.OrdinalIgnoreCase);
            seriesDirectory = seriesDirectory.Replace("#YEAR", episode.Date.Year.ToString(), StringComparison.OrdinalIgnoreCase);

            var subDirectory = Path.Combine(seriesDirectory, episodeDirectory);
            subDirectory = string.Join(@"\", subDirectory.Split(@"\", StringSplitOptions.RemoveEmptyEntries).Select(x => DirectorySanitizer.Sanitize(x)));

            return Path.Combine(BasePath, subDirectory);
        }
    }
}
