using System;
using System.IO;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodeDirectoryProvider : IOrfTvSeriesEpisodeDirectoryProvider
    {
        private readonly BaseDirectoriesConfiguration baseDirectoriesConfiguration;

        public OrfTvSeriesEpisodeDirectoryProvider(BaseDirectoriesConfiguration baseDirectoriesConfiguration)
        {
            this.baseDirectoriesConfiguration = baseDirectoriesConfiguration;
        }

        public DirectoryInfo GetDirectory(OrfTvSeriesSubscription subscription, OrfTvSeries series, OrfTvSeriesEpisode episode)
        {
            ArgumentNullException.ThrowIfNull(subscription);
            ArgumentNullException.ThrowIfNull(series);
            ArgumentNullException.ThrowIfNull(episode);

            var baseDirectory = _GetBasePath();
            var seriesDirectoryPart = _GetSeriesPart(subscription, series, episode);
            var episodePart = _GetEpisodePart(subscription, series, episode);

            var subDirectory = Path.Combine(seriesDirectoryPart, episodePart);
            var sanitizedSubDirectory = _SanitizeDirectory(subDirectory);

            var directory = Path.Combine(baseDirectory, sanitizedSubDirectory);

            return new DirectoryInfo(directory);
        }

        private string _GetBasePath()
        {
            return baseDirectoriesConfiguration.BasePath;
        }

        private string _GetSeriesPart(OrfTvSeriesSubscription subscription, OrfTvSeries series, OrfTvSeriesEpisode episode)
        {
            var seriesDirectory = subscription.DownloadSubDirectory ?? baseDirectoriesConfiguration.DefaultSubDirectory;

            var nameInDirectory = string.IsNullOrEmpty(subscription.Name)
                ? series.Title
                : subscription.Name;
            seriesDirectory = seriesDirectory.Replace("#NAME", nameInDirectory, StringComparison.OrdinalIgnoreCase);
            seriesDirectory = seriesDirectory.Replace("#YEAR", episode.Date.Year.ToString(), StringComparison.OrdinalIgnoreCase);

            return seriesDirectory;
        }

        private string _GetEpisodePart(OrfTvSeriesSubscription subscription, OrfTvSeries series, OrfTvSeriesEpisode episode)
        {
            var episodeNameFormat = subscription.EpisodeNameFormat ?? baseDirectoriesConfiguration.DefaultEpisodeNameFormat;
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

            return episodeDirectory;
        }

        private string _GetFullDirectory(string baseDirectory, string seriesDirectoryPart, string episodePart)
        {
            return Path.Combine(baseDirectory, seriesDirectoryPart, episodePart);
        }


        private string _SanitizeDirectory(string directory)
        {
            return string.Join(Path.DirectorySeparatorChar, directory.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).Select(x => DirectorySanitizer.Sanitize(x)));
        }

       
    }
}
