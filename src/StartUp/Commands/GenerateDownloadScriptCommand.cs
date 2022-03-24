using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class GenerateDownloadScriptCommand : Command
    {
        private readonly IOrfTvSeriesScrapper seriesScrapper;
        private readonly IOrfTvSeriesEpisodesProvider episodesProvider;
        private readonly IOrfTvSeriesEpisodeDirectoryProvider directoryProvider;

        public GenerateDownloadScriptCommand(IOrfTvSeriesScrapper seriesScrapper, IOrfTvSeriesEpisodesProvider episodesProvider, IOrfTvSeriesEpisodeDirectoryProvider directoryProvider)
            : base("generateDownloadScript", "generates a download script for episodes of subscribed series")
        {
            this.seriesScrapper = seriesScrapper;
            this.episodesProvider = episodesProvider;
            this.directoryProvider = directoryProvider;
            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<string>(new[] { "--targetPath", "-t" }, "target path of script"));
            AddOption(new Option<string>(new[] { "--targetPathInfo", "-ti" }, "target path of info file"));

            Handler = CommandHandler.Create<string, string, string>(_HandleCommand);
        }

        private async Task _HandleCommand(string channel, string targetPath, string targetPathInfo)
        {
            using StreamWriter sw = new StreamWriter(File.Open(targetPath, FileMode.Create));
            sw.WriteLine(@"$dl=""C:\Users\Stefan\Downloads\youtube-dl.exe""");
            sw.WriteLine("$defaultParameters=\"--write-description --write-annotations --write-all-thumbnails --write-info-json\"");
            sw.WriteLine();

            using StreamWriter infoWriter =
                string.IsNullOrEmpty(targetPathInfo) ?
                    StreamWriter.Null :
                    new StreamWriter(targetPathInfo);
            
            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var tvSeries = seriesScrapper.GetListOfTvSeries();

            //Console.WriteLine($"subscriptions before sorting: {subscriptions.Count}");
            //subscriptions = subscriptions.Where(s => tvSeries.Any(t => t.Id == s.OrfTvSeriesId)).OrderBy(s => tvSeries.FirstOrDefault(t => t.Id == s.OrfTvSeriesId)?.Title).ToList();
            //Console.WriteLine($"subscriptions after sorting: {subscriptions.Count}");
            //return;

            foreach (var subscription in subscriptions)
            {
                var tvSeriesItem = tvSeries.FirstOrDefault(x => x.Id == subscription.OrfTvSeriesId);
                if (tvSeriesItem == null)
                {
                    continue;
                }

                var episodes = await episodesProvider.GetEpisodesAsync(tvSeriesItem);

                _writeEpisodes(sw, infoWriter, tvSeriesItem, episodes, subscription);
            }
        }

        private void _writeEpisodes(StreamWriter scriptWriter, StreamWriter infoWriter, OrfTvSeries series, IReadOnlyCollection<OrfTvSeriesEpisode> episodes, OrfTvSeriesSubscription subscription)
        {
            Console.WriteLine($"{series.Title} ({series.Id})");
            foreach (var episode in episodes)
            {
                var directory = directoryProvider.GetDirectory(subscription, series, episode);
                if (directory.Exists)
                {
                    continue;
                }

                Console.WriteLine($"\t{episode.Date:g} {episode.Title} ({episode.Channel}, {episode.Duration})");
                Console.WriteLine($"\t\t{episode.DownloadUrl}");

                if (!string.IsNullOrEmpty(episode.Description))
                {
                    Console.WriteLine($"\t\t{episode.Description}");
                }
                var directoryBackup = new DirectoryInfo(directory.FullName);
                var needsShortPath = false;
                if ((directory.FullName.Length + episode.Title.Length) > 150)
                {
                    directory = new DirectoryInfo(Path.Combine(directory.Parent.FullName, directory.Name.Substring(0, 10)));
                    needsShortPath = true;
                }

                scriptWriter.WriteLine($"New-Item \"{directory.FullName}\" -ItemType \"directory\"");
                scriptWriter.WriteLine($"Start-Process -FilePath $dl -WorkingDirectory \"{directory.FullName}\" -ArgumentList \"$defaultParameters {episode.DownloadUrl}\" -NoNewWindow -Wait");

                if (needsShortPath)
                {
                    scriptWriter.WriteLine($"Rename-Item \"{directory.FullName}\" \"{directoryBackup}\"");
                    directory = directoryBackup;
                }

                scriptWriter.WriteLine();

                infoWriter.WriteLine($"{series.Title}: {episode.Title} ({episode.Date:g}, {episode.Channel}) - {episode.Duration}");
                if (!string.IsNullOrEmpty(episode.Description))
                {
                    infoWriter.WriteLine($"\t{_FormatDescription(episode.Description)}");
                }
                infoWriter.WriteLine($"\t{directory.FullName}");
            }
        }

        private string _FormatDescription(string description)
        {
            return description.ReplaceLineEndings(" ");
        }
    }
}
