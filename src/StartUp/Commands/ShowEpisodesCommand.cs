using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowEpisodesCommand : Command
    {
        private readonly IOrfTvSeriesScrapper seriesScrapper;
        private readonly IOrfTvSeriesEpisodesProvider episodesProvider;

        public ShowEpisodesCommand(IOrfTvSeriesScrapper seriesScrapper, IOrfTvSeriesEpisodesProvider episodesProvider)
            : base("showEpisodes", "shows all epsiodes for a series")
        {
            this.seriesScrapper = seriesScrapper;
            this.episodesProvider = episodesProvider;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<int?>(new[] { "--id", "-id" }, "id of series"));
            AddOption(new Option(new[] { "--all", "-a" }, "show episodes for all subscribed series, overrides id"));
            AddOption(new Option(new[] { "--newOnly", "-no" }, "shows only new episodes"));

            Handler = CommandHandler.Create<string, int?, bool, bool>(_HandleCommand);
        }

        private async Task _HandleCommand(string channel, int? id, bool all, bool newOnly)
        {
            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var tvSeries = seriesScrapper.GetListOfTvSeries();

            if (!all)
            {
                var series = tvSeries.FirstOrDefault(s => s.Id == id.ToString());
                if (series == null)
                {
                    Console.WriteLine($"series with id {id} doesn't exist");
                    return;
                }

                var episodes = await episodesProvider.GetEpisodesAsync(series);
                _showEpisodes(series, episodes, subscriptions.FirstOrDefault(x => x.OrfTvSeriesId == series.Id), newOnly);
            }
            else
            {
                foreach (var subscription in subscriptions)
                {
                    var tvSeriesItem = tvSeries.FirstOrDefault(x => x.Id == subscription.OrfTvSeriesId);
                    if (tvSeriesItem == null)
                    {
                        continue;
                    }

                    var episodes = await episodesProvider.GetEpisodesAsync(tvSeriesItem);

                    _showEpisodes(tvSeriesItem, episodes, subscription, newOnly);
                }
            }
        }

        private void _showEpisodes(OrfTvSeries series, IReadOnlyCollection<OrfTvSeriesEpisode> episodes, OrfTvSeriesSubscription subscription, bool newOnly)
        {
            Console.WriteLine($"{series.Title} ({series.Id})");
            foreach (var episode in episodes)
            {
                string directory = null;
                var hasToWrite = true;
                if (subscription != null)
                {
                    var directoryProvider = new OrfTvSeriesEpisodeDirectoryProvider();
                    directory = directoryProvider.GetDirectory(subscription, series, episode);

                    if (newOnly && Directory.Exists(directory))
                    {
                        hasToWrite = false;
                    }
                }

                if (!hasToWrite)
                {
                    continue;
                }

                Console.WriteLine($"\t{episode.Date:g} {episode.Title} ({episode.Channel}, {episode.Duration})");
                Console.WriteLine($"\t\t{episode.DownloadUrl}");

                if (subscription != null)
                {
                    Console.Write("\t\t");
                    if (Directory.Exists(directory))
                    {
                        Console.Write("= ");
                    }
                    else
                    {
                        Console.Write("* ");
                    }
                    Console.WriteLine(directory);
                }

                if (!string.IsNullOrEmpty(episode.Description))
                {
                    Console.WriteLine($"\t\t{episode.Description}");
                }
            }
        }
    }
}
