using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowAvailableEpisodesCommand : Command
    {
        private readonly IOrfTvSeriesScrapper seriesScrapper;
        private readonly IOrfTvSeriesEpisodesProvider episodesProvider;

        public ShowAvailableEpisodesCommand(IOrfTvSeriesScrapper seriesScrapper, IOrfTvSeriesEpisodesProvider episodesProvider)
            : base("showEpisodes", "shows all epsiodes for a series")
        {
            this.seriesScrapper = seriesScrapper;
            this.episodesProvider = episodesProvider;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<int?>(new[] { "--id", "-id" }, "id of series"));
            AddOption(new Option(new[] { "--all", "-a" }, "ignoring all unknown, overrides id"));

            Handler = CommandHandler.Create<string, int?, bool>(_HandleCommand);
        }

        private async Task _HandleCommand(string channel, int? id, bool all)
        {
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
                _showEpisodes(series, episodes);
            }
            else
            {
                var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
                var subscriptions = subscriptionService.GetSubscriptions();

                foreach (var subscription in subscriptions)
                {
                    var tvSeriesItem = tvSeries.FirstOrDefault(x => x.Id == subscription.OrfTvSeriesId);
                    if (tvSeriesItem == null)
                    {
                        continue;
                    }

                    var episodes = await episodesProvider.GetEpisodesAsync(tvSeriesItem);

                    _showEpisodes(tvSeriesItem, episodes);
                }
            }
        }

        private void _showEpisodes(OrfTvSeries series, IReadOnlyCollection<OrfTvSeriesEpisode> episodes)
        {
            Console.WriteLine($"{series.Title} ({series.Id})");
            foreach (var episode in episodes)
            {
                Console.WriteLine($"\t{episode.Date:g} {episode.Title} ({episode.Channel}, {episode.Duration})");
                if (!string.IsNullOrEmpty(episode.Description))
                {
                    Console.WriteLine($"\t\t{episode.Description}");
                }
                Console.WriteLine($"\t\t{episode.DownloadUrl}");
            }
        }
    }
}
