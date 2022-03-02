using AustrianTvScrapper.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class AddCommand : Command
    {
        private readonly IOrfTvSeriesScrapper orfTvSeriesScrapper;

        public AddCommand(IOrfTvSeriesScrapper orfTvSeriesScrapper)
            : base("add", "adds a subscription")
        {
            this.orfTvSeriesScrapper = orfTvSeriesScrapper;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));
            AddOption(new Option<int>(new[] { "--id", "-id" }, "id of series"));
            AddOption(new Option<string>(new[] { "--downloadSubDirectory", "-dir" }, () => string.Empty, description: "sub directory"));

            Handler = CommandHandler.Create<string, int, string>(_HandleCommand);
        }

        private void _HandleCommand(string channel, int id, string downloadSubDirectory)
        {
            var tvSeries = orfTvSeriesScrapper.GetListOfTvSeries();

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
