using AustrianTvScrapper.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace AustrianTvScrapper.StartUp
{
    class Program
    {
        static void Main(string[] args)
        {
            var showSeriesCommand = new Command("showSeries", "shows available series")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
                new Option<bool>(new[]{"--subscriptionInfo","-si"}, getDefaultValue: () => true, "shows subscription info"),
                new Option<bool>(new[]{"--showSubscribed", "-ss"}, getDefaultValue: () => true, "show subscribed"),
                new Option<bool>(new[]{"--showUnsubscribed", "-su"}, getDefaultValue: () => true, "show unsubscribed"),
                new Option<bool>(new[]{"--writeSnapshot", "-ws"}, getDefaultValue: () => true, "writes a snapshot"),
                new Option<string>(new[]{ "--compareSnapshotFilename", "-cs"}, getDefaultValue: () => null, "compares to a snapshot"),
                new Option<bool>(new[]{ "--showNewOnly", "-sno"}, getDefaultValue: () => true, "show new series only when comparing to snapshot"),
            };
            showSeriesCommand.Handler = CommandHandler.Create<string, bool, bool, bool, bool, string, bool>(_HandleShowSeries);

            var writeUnsubscribed = new Command("writeUnsubscribed", "writes file with unsubscribed")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
            };
            writeUnsubscribed.Handler = CommandHandler.Create<string>(_HandleWriteUnsubscribed);

            var showUndefined = new Command("showUndefined", "shows undefined descriptions")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
            };
            showUndefined.Handler = CommandHandler.Create<string>(_HandleShowUndefined);

            var ignore = new Command("ignore", "ignores a series")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
                new Option<uint?>(new[]{ "--id", "-id"}, "id of series"),
                new Option(new[]{ "--all", "-a"}, "ignoring all unknown, overrides id"),
            };
            ignore.Handler = CommandHandler.Create<string, uint, bool>(_HandleIgnore);

            var add = new Command("add", "adds a subscription")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
                new Option<uint>(new[]{ "--id", "-id"}, "id of series"),
                new Option<string>(new[]{ "--downloadSubDirectory", "-dir"}, string.Empty, description: "sub directory"),
            };
            add.Handler = CommandHandler.Create<string, uint, string>(_HandleAdd);

            var getSubscribedEpisodes = new Command("getSubscribedEpisodes", "gets all epsiodes for the subscriptions")
            {
                new Argument<string>("channel", getDefaultValue: ()=> "Orf"),
            };
            getSubscribedEpisodes.Handler = CommandHandler.Create<string>(_GetSubscribedEpisodes);

            var rootCommand = new RootCommand()
            {
                showSeriesCommand,
                writeUnsubscribed,
                showUndefined,
                ignore,
                add,
                getSubscribedEpisodes
            };
            rootCommand.Description = "Austrian TV Series Scrapper";

            rootCommand.Invoke(args);

            //_CreateSubscriptionForAll();
        }

        private static void _GetSubscribedEpisodes(string channel)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var episodeProvider = new OrfTvSeriesEpisodesProvider();

            foreach (var subscription in subscriptions)
            {
                var tvSeriesItem = tvSeries.FirstOrDefault(x => x.Id == subscription.OrfTvSeriesId);
                if (tvSeriesItem == null)
                {
                    continue;
                }

                var episodes = episodeProvider.GetAvailableEpisodes(tvSeriesItem);

                foreach (var episode in episodes)
                {
                    Console.WriteLine($"{tvSeriesItem.Title}:{episode.Date} {episode.Title}");
                }
            }
        }

        private static void _HandleIgnore(string channel, uint id, bool all)
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

        private static void _HandleAdd(string channel, uint id, string downloadSubDirectory)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var series = tvSeries.FirstOrDefault(s => s.Id == id.ToString());
            if (series == null)
            {
                Console.WriteLine($"series with id {id} doesn't exist");
                return;
            }

            if (subscriptions.Any(s => s.OrfTvSeriesId == id.ToString()))
            {
                Console.WriteLine($"series with id {id} is already added");
                return;
            }

            var subscription = OrfTvSeriesSubscription.CreateForTvSeries(series);

            if (!string.IsNullOrEmpty(downloadSubDirectory))
            {
                subscription.DownloadSubDirectory = downloadSubDirectory;
            }

            subscriptionService.AddSubscription(subscription);
        }

        private static void _HandleShowUndefined(string obj)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

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

        private static void _HandleWriteUnsubscribed(string channel)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var unsubscribedService = new OrfTvSeriesSubscriptionService(
                new CustomDataDirectoryProvider(Directory.GetCurrentDirectory()),
                $"OrfTvSeriesUnsubscribed_{DateTime.Now:yyyyMMdd-HHmmss}.json"
                );

            foreach (var item in tvSeries)
            {
                var isSubscribed = _HasSubscription(item, subscriptions);

                if (isSubscribed)
                    continue;

                var unsubscribed = _CreateSubscription(item);
                unsubscribedService.AddSubscription(unsubscribed);
            }
        }

        private static void _HandleShowSeries(string channel, bool subscriptionInfo, bool showSubscribed, bool showUnsubscribed, bool writeSnapshot, string compareSnapshotFilename, bool showNewOnly)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

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

        private static void _CreateSubscriptionForAll()
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();

            var subscriptions = tvSeries.Select(_CreateSubscription);

            var subscriptionService = new OrfTvSeriesSubscriptionService(new CustomDataDirectoryProvider(Directory.GetCurrentDirectory()));
            foreach (var subscription in subscriptions)
            {
                subscriptionService.AddSubscription(subscription);
            }
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
