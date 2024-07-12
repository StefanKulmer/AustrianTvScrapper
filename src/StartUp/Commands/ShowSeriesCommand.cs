using AustrianTvScrapper.Services;
using OrfDataProvider.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowSeriesCommand : Command
    {
        private readonly IOrfTvSeriesScrapper _orfTvSeriesScrapper;
        private readonly IOrfTvSeriesSnapshotService _orfTvSeriesSnapshotService;
        private readonly IOrfDataProvider _orfDataProvider;

        public ShowSeriesCommand(IOrfTvSeriesScrapper orfTvSeriesScrapper, IOrfTvSeriesSnapshotService orfTvSeriesSnapshotService, IOrfDataProvider orfDataProvider)
            : base("showSeries", "shows available series")
        {
            _orfTvSeriesScrapper = orfTvSeriesScrapper;
            _orfTvSeriesSnapshotService = orfTvSeriesSnapshotService;
            _orfDataProvider = orfDataProvider;

            AddOption(new Option<bool>(new[] { "--episodes", "-e" }, getDefaultValue: () => false, "shows episodes"));

            //AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            //AddOption(new Option<bool>(new[] { "--subscriptionInfo", "-si" }, getDefaultValue: () => true, "shows subscription info"));
            //AddOption(new Option<bool>(new[] { "--showSubscribed", "-ss" }, getDefaultValue: () => true, "show subscribed"));
            //AddOption(new Option<bool>(new[] { "--showUnsubscribed", "-su" }, getDefaultValue: () => true, "show unsubscribed"));
            //AddOption(new Option<bool>(new[] { "--writeSnapshot", "-ws" }, getDefaultValue: () => true, "writes a snapshot"));
            //AddOption(new Option<string>(new[] { "--compareSnapshotFilename", "-cs" }, getDefaultValue: () => null, "compares to a snapshot"));
            //AddOption(new Option<bool>(new[] { "--showNewOnly", "-sno" }, getDefaultValue: () => true, "show new series only when comparing to snapshot"));

            Handler = CommandHandler.Create<bool>(_HandleCommand);
        }

        private async void _HandleCommand(bool showEpisodes)
        {
            var genres = _orfDataProvider.GetGenres().Result;
            var profiles = _orfDataProvider.GetProfiles().Result;
            foreach (var profile in profiles.OrderBy(p => p.Title))
            {
                var genre = genres.First(g => g.TheLinks.Self.TheHref == profile.Links.Genre.Href);
                Console.WriteLine(profile.Id + " " + profile.Title + " - " + profile.UpdatedAt.ToString("yyyy-MM-dd") + " - " + genre?.Title ?? "?");
                Console.WriteLine("\t" + profile.Description);

                if (showEpisodes)
                {
                    var episodes = _orfDataProvider.GetEpisodesOfProfileAsync(profile.Id).Result;
                    Console.WriteLine("\t{0} episodes", episodes.Count);
                    Console.WriteLine("\t{0:yyyy-MM-dd} {1}", episodes.First().ReleaseDate, episodes.First().Name);
                }
            }
        }

        //private void _HandleCommand(string channel, bool subscriptionInfo, bool showSubscribed, bool showUnsubscribed, bool writeSnapshot, string compareSnapshotFilename, bool showNewOnly)
        //{
        //    var tvSeries = orfTvSeriesScrapper.GetListOfTvSeries();

        //    var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
        //    var subscriptions = subscriptionService.GetSubscriptions();

        //    IReadOnlyCollection<KeyValuePair<ComparisonResult, OrfTvSeries>> comparisonResult = null;
        //    if (compareSnapshotFilename != null)
        //    {
        //        var compareSnapshot = orfTvSeriesSnapshotService.ReadSnapshot(compareSnapshotFilename);

        //        var comparisonService = new OrfTvSeriesComparisonService();
        //        comparisonResult = comparisonService.Compare(tvSeries, compareSnapshot.OrfTvSeries);
        //    }

        //    foreach (var item in tvSeries)
        //    {
        //        var isSubscribed = _HasSubscription(item, subscriptions);

        //        if (isSubscribed && !showSubscribed)
        //        {
        //            continue;
        //        }

        //        if (!isSubscribed && !showUnsubscribed)
        //        {
        //            continue;
        //        }

        //        if (subscriptionInfo)
        //        {
        //            if (isSubscribed)
        //            {
        //                Console.ForegroundColor = ConsoleColor.Green;
        //            }
        //            else
        //            {
        //                Console.ForegroundColor = ConsoleColor.Red;
        //            }
        //        }

        //        if (compareSnapshotFilename != null)
        //        {
        //            var result = comparisonResult.FirstOrDefault(x => string.Compare(x.Value.Id, item.Id) == 0);
        //            if (result.Key == ComparisonResult.ExistsOnlyOnLeftSide)
        //            {
        //                Console.Write("NEW ");
        //            }
        //            else if (showNewOnly)
        //            {
        //                continue;
        //            }
        //        }

        //        _WriteTvSeries(item, subscriptionInfo);
        //    }

        //    // show series which are no longer existing
        //    if (compareSnapshotFilename != null && !showNewOnly)
        //    {
        //        foreach (var comparisonEntry in comparisonResult)
        //        {
        //            if (comparisonEntry.Key == ComparisonResult.ExistsOnlyOnRightSide)
        //            {
        //                Console.Write("OLD ");
        //                _WriteTvSeries(comparisonEntry.Value, true);
        //            }
        //        }
        //    }
        //}

        //private static void _WriteTvSeries(OrfTvSeries item, bool resetColorAfterTitle)
        //{
        //    Console.WriteLine($"{item.Title} ({item.Id})");

        //    if (resetColorAfterTitle)
        //    {
        //        Console.ResetColor();
        //    }

        //    Console.WriteLine($"\t{item.Url}");
        //    Console.WriteLine($"\t{item.Description}");
        //    Console.WriteLine($"\t{item.Channel}");
        //    Console.WriteLine($"\t{item.Profile}");
        //}

        //private static bool _HasSubscription(OrfTvSeries tvSeries, IReadOnlyCollection<OrfTvSeriesSubscription> subscriptions)
        //{
        //    return subscriptions.Any(x => x.OrfTvSeriesId == tvSeries.Id);
        //}
    }
}
