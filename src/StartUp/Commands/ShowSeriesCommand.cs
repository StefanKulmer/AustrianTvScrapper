using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowSeriesCommand : Command
    {
        private readonly IOrfTvSeriesScrapper orfTvSeriesScrapper;

        public ShowSeriesCommand(IOrfTvSeriesScrapper orfTvSeriesScrapper)
            : base("showSeries", "shows available series")
        {
            this.orfTvSeriesScrapper = orfTvSeriesScrapper;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<bool>(new[] { "--subscriptionInfo", "-si" }, getDefaultValue: () => true, "shows subscription info"));
            AddOption(new Option<bool>(new[] { "--showSubscribed", "-ss" }, getDefaultValue: () => true, "show subscribed"));
            AddOption(new Option<bool>(new[] { "--showUnsubscribed", "-su" }, getDefaultValue: () => true, "show unsubscribed"));
            AddOption(new Option<bool>(new[] { "--writeSnapshot", "-ws" }, getDefaultValue: () => true, "writes a snapshot"));
            AddOption(new Option<string>(new[] { "--compareSnapshotFilename", "-cs" }, getDefaultValue: () => null, "compares to a snapshot"));
            AddOption(new Option<bool>(new[] { "--showNewOnly", "-sno" }, getDefaultValue: () => true, "show new series only when comparing to snapshot"));

            Handler = CommandHandler.Create<string, bool, bool, bool, bool, string, bool>(_HandleCommand);
        }

        private void _HandleCommand(string channel, bool subscriptionInfo, bool showSubscribed, bool showUnsubscribed, bool writeSnapshot, string compareSnapshotFilename, bool showNewOnly)
        {
            var tvSeries = orfTvSeriesScrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var snapshotService = new OrfTvSeriesSnapshotService(new UserDocumentsDataDirectoryProvider(), scrapper);

            IReadOnlyCollection<KeyValuePair<ComparisonResult, OrfTvSeries>> comparisonResult = null;
            if (compareSnapshotFilename != null)
            {
                var compareSnapshot = snapshotService.ReadSnapshot(compareSnapshotFilename);

                var comparisonService = new OrfTvSeriesComparisonService();
                comparisonResult = comparisonService.Compare(tvSeries, compareSnapshot.OrfTvSeries);
            }

            foreach (var item in tvSeries)
            {
                var isSubscribed = _HasSubscription(item, subscriptions);

                if (isSubscribed && !showSubscribed)
                {
                    continue;
                }

                if (!isSubscribed && !showUnsubscribed)
                {
                    continue;
                }

                if (subscriptionInfo)
                {
                    if (isSubscribed)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }

                if (compareSnapshotFilename != null)
                {
                    var result = comparisonResult.FirstOrDefault(x => string.Compare(x.Value.Id, item.Id) == 0);
                    if (result.Key == ComparisonResult.ExistsOnlyOnLeftSide)
                    {
                        Console.Write("NEW ");
                    }
                    else if (showNewOnly)
                    {
                        continue;
                    }
                }

                _WriteTvSeries(item, subscriptionInfo);
            }

            // show series which are no longer existing
            if (compareSnapshotFilename != null && !showNewOnly)
            {
                foreach (var comparisonEntry in comparisonResult)
                {
                    if (comparisonEntry.Key == ComparisonResult.ExistsOnlyOnRightSide)
                    {
                        Console.Write("OLD ");
                        _WriteTvSeries(comparisonEntry.Value, true);
                    }
                }
            }

            if (writeSnapshot)
            {
                snapshotService.CreateSnapshot(tvSeries);
            }
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

        private static bool _HasSubscription(OrfTvSeries tvSeries, IReadOnlyCollection<OrfTvSeriesSubscription> subscriptions)
        {
            return subscriptions.Any(x => x.OrfTvSeriesId == tvSeries.Id);
        }
    }
}
