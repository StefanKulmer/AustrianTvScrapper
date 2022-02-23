using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class IgnoreCommand : Command
    {
        public IgnoreCommand()
            : base("ignore", "ignores a series")
        {
            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<uint?>(new[] { "--id", "-id" }, "id of series"));
            AddOption(new Option(new[] { "--all", "-a" }, "ignoring all unknown, overrides id"));

            Handler = CommandHandler.Create<string, uint, bool>(_HandleCommand);
        }

        private static void _HandleCommand(string channel, uint id, bool all)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

            var unsubscribedService = new OrfTvSeriesSubscriptionService(
                new UserDocumentsDataDirectoryProvider(),
                $"OrfTvSeriesUnsubscribed.json"
                );
            var unsubscribed = unsubscribedService.GetSubscriptions();

            if (!all)
            {
                var series = tvSeries.FirstOrDefault(s => s.Id == id.ToString());
                if (series == null)
                {
                    Console.WriteLine($"series with id {id} doesn't exist");
                    return;
                }

                if (unsubscribed.Any(s => s.OrfTvSeriesId == id.ToString()))
                {
                    Console.WriteLine($"series with id {id} is already ignored");
                    return;
                }

                unsubscribedService.AddSubscription(OrfTvSeriesSubscription.CreateForTvSeries(series));
            }
            else
            {
                var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
                var subscriptions = subscriptionService.GetSubscriptions();

                var total = 0;
                var undefinedItems = new List<OrfTvSeriesSubscription>();
                foreach (var item in tvSeries)
                {
                    var isSubscribed = _HasSubscription(item, subscriptions);
                    if (isSubscribed)
                        continue;

                    var isUnsubscribed = _HasSubscription(item, unsubscribed);
                    if (isUnsubscribed)
                        continue;

                    var undefined = _CreateSubscription(item);
                    undefinedItems.Add(undefined);

                    Console.WriteLine("ignoring {0} {1} ...", item.Title, item.Id);

                    total++;
                }

                Console.WriteLine($"ignored {total} items.");

                undefinedItems.ForEach(x => unsubscribedService.AddSubscription(x));
            }
        }

        private static bool _HasSubscription(OrfTvSeries tvSeries, IReadOnlyCollection<OrfTvSeriesSubscription> subscriptions)
        {
            return subscriptions.Any(x => x.OrfTvSeriesId == tvSeries.Id);
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
    }
}
