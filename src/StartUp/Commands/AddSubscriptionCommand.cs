using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class AddSubscriptionCommand : Command
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IOrfDataProvider _orfDataProvider;

        public AddSubscriptionCommand(Subscription.Services.ISubscriptionManager subscriptionManager, IOrfDataProvider orfDataProvider)
            : base("add-subscription", "adds a subscription")
        {
            _subscriptionManager = subscriptionManager;
            _orfDataProvider = orfDataProvider;
            var idOption = new Option<int>("--id", "-id") { Description = "id of TV show" };
            var dirOption = new Option<string>("--downloadSubDirectory", "-dir") { Description = "sub directory" };
            Add(idOption);
            Add(dirOption);

            this.SetAction(parseResult => _HandleCommand(
                parseResult.GetValue(idOption),
                parseResult.GetValue(dirOption)));
        }

        private async Task _HandleCommand(int id, string downloadSubDirectory)
        {
            var profile = await _orfDataProvider.GetProfile(id).ConfigureAwait(false);
            if (profile == null)
            {
                Console.WriteLine($"profile {id} doesn't exist.");
                return;
            }

            var subscriptions = _subscriptionManager.GetSubscriptions();
            if (subscriptions.Any(s => s.ProfileId == id))
            {
                Console.WriteLine($"subscription for {id} {profile.Title} already exists.");
                return;
            }

            var subscription = new Subscription.Model.Subscription()
            {
                ProfileId = id,
                Name = profile.Title,
                Created = DateTime.Now,
            };

            var genres = await _orfDataProvider.GetGenres().ConfigureAwait(false);
            var genre = genres.First(g => g.TheLinks.Self.TheHref == profile.Links.Genre.Href);
            string subDir = null;
            if (genre != null)
            {
                switch (genre.Title)
                {
                    case "ORF KIDS":
                        subDir = "(Kinder)";
                        break;
                    case "Serie":
                        subDir = "(Serien)";
                        break;
                    case "Film":
                        var episodes = await _orfDataProvider.GetEpisodesOfProfileAsync(id).ConfigureAwait(false);
                        if (episodes.Count > 1)
                        {
                            subDir = "(Serien)";
                        }
                        break;
                }
            }
            if (subDir != null)
            {
                subscription.DownloadSubDirectory = @$"#year\{subDir}\{profile.Title}";
            }
            else
            {
                subscription.DownloadSubDirectory = @$"#year\{profile.Title}";
            }

            _subscriptionManager.AddSubscription(subscription);

            Console.WriteLine($"subscription for {id} {profile.Title} added.");
        }
    }
}
