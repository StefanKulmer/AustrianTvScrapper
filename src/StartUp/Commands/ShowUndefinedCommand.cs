using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowUndefinedCommand : Command
    {
        private readonly IOrfTvSeriesScrapper orfTvSeriesScrapper;

        public ShowUndefinedCommand(IOrfTvSeriesScrapper orfTvSeriesScrapper) 
            : base("showUndefined", "shows undefined series")
        {
            this.orfTvSeriesScrapper = orfTvSeriesScrapper;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private void _HandleCommand(string obj)
        {
            var tvSeries = orfTvSeriesScrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var unsubscribedService = new OrfTvSeriesSubscriptionService(
                new UserDocumentsDataDirectoryProvider(),
                $"OrfTvSeriesUnsubscribed.json"
                );
            var unsubscribed = unsubscribedService.GetSubscriptions();

            var undefinedService = new OrfTvSeriesSubscriptionService(
                new CustomDataDirectoryProvider(Directory.GetCurrentDirectory()),
                $"OrfTvSeriesUndefined_{DateTime.Now:yyyyMMdd-HHmmss}.json"
                );

            int total = 0;
            foreach (var item in tvSeries)
            {
                var isSubscribed = _HasSubscription(item, subscriptions);
                if (isSubscribed)
                    continue;

                var isUnsubscribed = _HasSubscription(item, unsubscribed);
                if (isUnsubscribed)
                    continue;

                _WriteTvSeries(item, false);

                var undefined = _CreateSubscription(item);
                undefinedService.AddSubscription(undefined);

                total++;
            }

            Console.WriteLine($"found {total} unknown");
        }

        private static OrfTvSeriesSubscription _CreateSubscription(OrfTvSeries series)
        {
            return new OrfTvSeriesSubscription()
            {
                Name = series.Title,
                OrfTvSeriesId = series.Id,
                DownloadSubDirectory = null,
                EpisodeNameFormat = null,
                EpisodeNameRemovals = null
            };
        }

        private static bool _HasSubscription(OrfTvSeries tvSeries, IReadOnlyCollection<OrfTvSeriesSubscription> subscriptions)
        {
            return subscriptions.Any(x => x.OrfTvSeriesId == tvSeries.Id);
        }

        private static void _WriteTvSeries(OrfTvSeries item, bool resetColorAfterTitle)
        {
            Console.WriteLine($"{item.Title} ({item.Id})");

            if (resetColorAfterTitle)
            {
                Console.ResetColor();
            }

            Console.WriteLine($"\t{item.Url}");
            Console.WriteLine($"\t{item.Description}");
            Console.WriteLine($"\t{item.Channel}");
            Console.WriteLine($"\t{item.Profile}");
        }
    }
}
