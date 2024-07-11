using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO.Abstractions;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ImportSubscriptionsCommand : Command
    {
        private readonly IOrfDataProvider orfDataProvider;
        private readonly ISubscriptionManager subscriptionManager;
        private readonly IUnSubscriptionManager unSubscriptionManager;
        private readonly IFileSystem fileSystem;

        public ImportSubscriptionsCommand(
            IOrfDataProvider orfDataProvider,
            ISubscriptionManager subscriptionManager, 
            IUnSubscriptionManager unSubscriptionManager,
            IFileSystem fileSystem)
            : base("import-subscriptions", "imports subscriptions from a file")
        {
            this.orfDataProvider = orfDataProvider;
            this.subscriptionManager = subscriptionManager;
            this.unSubscriptionManager = unSubscriptionManager;
            this.fileSystem = fileSystem;
            AddOption(new Option<string>(new[] { "--source", "-s" }, getDefaultValue: () => null, "source file for import"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private async void _HandleCommand(string source)
        {
            var genres = orfDataProvider.GetGenres().Result;
            var lines = fileSystem.File.ReadAllLines(source);

            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                    continue;

                var firstChar = line[0];
                if (firstChar == '#')
                    continue;

                var secondSpace = line.IndexOf(' ', 2);
                if (secondSpace == -1)
                    continue;

                var idText = line.Substring(2, secondSpace - 2).Trim();
                if (!int.TryParse(idText, out var id))
                    continue;

                var profile = orfDataProvider.GetProfile(id).Result;
                if (profile == null)
                {
                    Console.WriteLine($"profile {id} doesn't exist.");
                    continue;
                }

                if (firstChar == 's')
                {
                    var subscriptions = subscriptionManager.GetSubscriptions();
                    if (subscriptions.Any(s => s.ProfileId == id))
                    {
                        Console.WriteLine($"subscription for {id} {profile.Title} already exists.");
                        continue;
                    }
                    var subscription = new Subscription.Model.Subscription()
                    {
                        ProfileId = id,
                        Name = profile.Title,
                        Created = DateTime.Now,
                    };

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

                    _RemoveFrom(unSubscriptionManager, id);
                }
                else if (firstChar == 'u')
                {
                    var subscriptions = unSubscriptionManager.GetSubscriptions();
                    if (subscriptions.Any(s => s.ProfileId == id))
                    {
                        Console.WriteLine($"unsubscription for {id} {profile.Title} already exists.");
                        continue;
                    }

                    var subscription = new Subscription.Model.Subscription()
                    {
                        ProfileId = id,
                        Name = profile.Title,
                        Created = DateTime.Now,
                    };

                    unSubscriptionManager.AddSubscription(subscription);

                    Console.WriteLine($"unsubscription for {id} {profile.Title} added.");

                    _RemoveFrom(unSubscriptionManager, id);
                }
            }
        }

        private void _RemoveFrom(ISubscriptionManager subscriptionManager, int id)
        {
            var subscriptions = subscriptionManager.GetSubscriptions();
            var subscription = subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription != null)
            {
                subscriptionManager.RemoveSubscription(subscription);
            }
        }
    }
}
