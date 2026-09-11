using AustrianTvScrapper.Services;
using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ExportSubscriptionsCommand : Command
    {
        private readonly IOrfDataProvider _orfDataProvider;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IUnSubscriptionManager _unSubscriptionManager;
        private readonly IFileSystem _fileSystem;

        public ExportSubscriptionsCommand(
            IOrfDataProvider orfDataProvider, 
            Subscription.Services.ISubscriptionManager subscriptionManager, Subscription.Services.IUnSubscriptionManager unSubscriptionManager,
            IFileSystem fileSystem)
            : base("export-subscriptions", "exports all profiles with subscription information. can be used for import")
        {
            _orfDataProvider = orfDataProvider;
            _subscriptionManager = subscriptionManager;
            _unSubscriptionManager = unSubscriptionManager;
            _fileSystem = fileSystem;
            var targetOption = new Option<string>(new[] { "--target", "-t" }) { Description = "target file for export" };
            var allOption = new Option<bool>(new[] { "--all", "-a" }) { Description = "export all; if not specified, only new one will be exported" };
            AddOption(targetOption);
            AddOption(allOption);

            this.SetHandler(async (string target, bool all) => await _HandleCommand(target, all), targetOption, allOption);
        }

        private async System.Threading.Tasks.Task _HandleCommand(string target, bool all)
        {
            var subscriptions = _subscriptionManager.GetSubscriptions();
            var unSubscriptions = _unSubscriptionManager.GetSubscriptions();

            var fileInfo = _fileSystem.FileInfo.New(target);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            using var fs = fileInfo.OpenWrite();
            using var writer = new StreamWriter(fs);

            var genres = _orfDataProvider.GetGenres().Result;
            var profiles = _orfDataProvider.GetProfiles().Result;
            foreach (var profile in profiles.OrderBy(p => p.Title))
            {
                bool isExisting = true;
                var hasSubscription = subscriptions.Any(s => s.ProfileId == profile.Id);
                if (hasSubscription && all)
                {
                    writer.Write("s");
                }

                var hasUnSubscriptions = unSubscriptions.Any(s => s.ProfileId == profile.Id);
                if (hasUnSubscriptions && all)
                {
                    writer.Write("u");
                }

                if (!hasSubscription && !hasUnSubscriptions)
                {
                    writer.Write("n");
                    isExisting = false;
                }

                if (!isExisting || all)
                {
                    writer.Write(" ");
                    var genre = genres.First(g => g.TheLinks.Self.TheHref == profile.Links.Genre.Href);
                    writer.WriteLine(profile.Id + " " + profile.Title + " - " + profile.UpdatedAt.ToString("yyyy-MM-dd") + " - " + genre?.Title ?? "?");
                }
            }
        }
    }
}
