using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.IO.Abstractions;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ImportSubscriptionsCommand : Command
    {
        private readonly IOrfDataProvider _orfDataProvider;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IUnSubscriptionManager _unSubscriptionManager;
        private readonly IFileSystem _fileSystem;

        public ImportSubscriptionsCommand(
            IOrfDataProvider orfDataProvider,
            ISubscriptionManager subscriptionManager, 
            IUnSubscriptionManager unSubscriptionManager,
            IFileSystem fileSystem)
            : base("import-subscriptions", "imports subscriptions from a file")
        {
            _orfDataProvider = orfDataProvider;
            _subscriptionManager = subscriptionManager;
            _unSubscriptionManager = unSubscriptionManager;
            _fileSystem = fileSystem;
            var sourceOption = new Option<string>("--source", "-s") { Description = "source file for import" };
            Add(sourceOption);

            this.SetAction(parseResult => _HandleCommand(parseResult.GetValue(sourceOption)));
        }

        private async System.Threading.Tasks.Task _HandleCommand(string source)
        {
            var genres = await _orfDataProvider.GetGenres().ConfigureAwait(false);
            var lines = _fileSystem.File.ReadAllLines(source);

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

                var profile = await _orfDataProvider.GetProfile(id).ConfigureAwait(false);
                if (profile == null)
                {
                    Console.WriteLine($"profile {id} doesn't exist.");
                    continue;
                }

                if (firstChar == 's')
                {
                    var subscriptions = _subscriptionManager.GetSubscriptions();
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

                    _RemoveFrom(_unSubscriptionManager, id);
                }
                else if (firstChar == 'u')
                {
                    var subscriptions = _unSubscriptionManager.GetSubscriptions();
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

                    _unSubscriptionManager.AddSubscription(subscription);

                    Console.WriteLine($"unsubscription for {id} {profile.Title} added.");

                    _RemoveFrom(_unSubscriptionManager, id);
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
