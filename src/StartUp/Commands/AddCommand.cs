using AustrianTvScrapper.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class AddCommand : Command
    {
        public AddCommand()
            : base("add", "adds a subscription")
        {
            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<uint>(new[] { "--id", "-id" }, "id of series"));
            AddOption(new Option<string>(new[] { "--downloadSubDirectory", "-dir" }, () => string.Empty, description: "sub directory"));

            Handler = CommandHandler.Create<string, uint, string>(_HandleCommand);
        }

        private static void _HandleCommand(string channel, uint id, string downloadSubDirectory)
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
    }
}
