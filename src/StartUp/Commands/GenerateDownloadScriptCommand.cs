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

        public GenerateDownloadScriptCommand(IOrfTvSeriesScrapper seriesScrapper, IOrfTvSeriesEpisodesProvider episodesProvider)
            : base("generateDownloadScript", "generates a download script for episodes of subscribed series")
        {
            this.seriesScrapper = seriesScrapper;
            this.episodesProvider = episodesProvider;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<string>(new[] { "--targetPath", "-t" }, "target path of script"));
            AddOption(new Option<string>(new[] { "--targetPathInfo", "-ti" }, "target path of info file"));

            Handler = CommandHandler.Create<string, string, string>(_HandleCommand);
        }

        private async Task _HandleCommand(string channel, string targetPath, string targetPathInfo)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //var enc = Encoding.GetEncodings().Where(x => x.Name.Contains("852"));
            //Console.WriteLine(string.Join(Environment.NewLine, enc.Select(x => x.Name)));

            //using StreamWriter sw = new StreamWriter(File.Open(targetPath, FileMode.Create), Encoding.GetEncoding("ibm852"));
            using StreamWriter sw = new StreamWriter(File.Open(targetPath, FileMode.Create), Encoding.GetEncoding(852));
            sw.WriteLine(@"set dl=C:\Users\Stefan\Downloads\youtube-dl.exe --write-description --write-annotations --write-all-thumbnails --write-info-json");

            using StreamWriter infoWriter =
                string.IsNullOrEmpty(targetPathInfo) ?
                    StreamWriter.Null :
                    new StreamWriter(targetPathInfo);
            
            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var tvSeries = seriesScrapper.GetListOfTvSeries();

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
                string directory = null;

                var directoryProvider = new OrfTvSeriesEpisodeDirectoryProvider();
                directory = directoryProvider.GetDirectory(subscription, series, episode);

                if (Directory.Exists(directory))
                {
                    continue;
                }

                Console.WriteLine($"\t{episode.Date:g} {episode.Title} ({episode.Channel}, {episode.Duration})");
                Console.WriteLine($"\t\t{episode.DownloadUrl}");

                if (!string.IsNullOrEmpty(episode.Description))
                {
                    Console.WriteLine($"\t\t{episode.Description}");

                    scriptWriter.WriteLine($"set theDir=\"{directory}\"");
                    scriptWriter.WriteLine("md \"%theDir%\"");
                    scriptWriter.WriteLine("cd /d %theDir%");
                    scriptWriter.WriteLine($"%dl% {episode.DownloadUrl}");
                    scriptWriter.WriteLine();

                    infoWriter.WriteLine($"{series.Title}: {episode.Title} ({episode.Date:g}) - {episode.Duration}");
                    if (!string.IsNullOrEmpty(episode.Description))
                    {
                        infoWriter.WriteLine($"\t{_FormatDescription(episode.Description)}");
                    }
                }
            }
        }

        private string _FormatDescription(string description)
        {
            return description.ReplaceLineEndings(" ");
        }
    }
}
