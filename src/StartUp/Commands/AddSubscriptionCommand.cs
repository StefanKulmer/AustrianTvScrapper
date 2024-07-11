using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class AddSubscriptionCommand : Command
    {
        private readonly ISubscriptionManager subscriptionManager;
        private readonly IOrfDataProvider orfDataProvider;

        public AddSubscriptionCommand(Subscription.Services.ISubscriptionManager subscriptionManager, IOrfDataProvider orfDataProvider)
            : base("add-subscription", "adds a subscription")
        {
            this.subscriptionManager = subscriptionManager;
            this.orfDataProvider = orfDataProvider;
            AddOption(new Option<int>(new[] { "--id", "-id" }, "id of TV show"));
            AddOption(new Option<string>(new[] { "--downloadSubDirectory", "-dir" }, () => string.Empty, description: "sub directory"));

            Handler = CommandHandler.Create<int, string>(_HandleCommand);
        }

        private void _HandleCommand(int id, string downloadSubDirectory)
        {
            var profile = orfDataProvider.GetProfile(id).Result;
            if (profile == null)
            {
                Console.WriteLine($"profile {id} doesn't exist.");
                return;
            }

            var subscriptions = subscriptionManager.GetSubscriptions();
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

            var genres = orfDataProvider.GetGenres().Result;
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
                        var episodes = orfDataProvider.GetEpisodesOfProfileAsync(id).Result;
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

            subscriptionManager.AddSubscription(subscription);

            Console.WriteLine($"subscription for {id} {profile.Title} added.");
        }
    }
}
