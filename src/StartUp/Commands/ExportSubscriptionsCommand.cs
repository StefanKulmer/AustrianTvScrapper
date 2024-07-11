using AustrianTvScrapper.Services;
using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ExportSubscriptionsCommand : Command
    {
        private readonly IOrfDataProvider orfDataProvider;
        private readonly ISubscriptionManager subscriptionManager;
        private readonly IUnSubscriptionManager unSubscriptionManager;
        private readonly IFileSystem fileSystem;

        public ExportSubscriptionsCommand(
            IOrfDataProvider orfDataProvider, 
            Subscription.Services.ISubscriptionManager 
            subscriptionManager, Subscription.Services.IUnSubscriptionManager unSubscriptionManager,
            IFileSystem fileSystem)
            : base("export-subscriptions", "exports all profiles with subscription information. can be used for import")
        {
            this.orfDataProvider = orfDataProvider;
            this.subscriptionManager = subscriptionManager;
            this.unSubscriptionManager = unSubscriptionManager;
            this.fileSystem = fileSystem;
            AddOption(new Option<string>(new[] { "--target", "-t" }, getDefaultValue: () => null, "target file for export"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private async void _HandleCommand(string target)
        {
            var subscriptions = subscriptionManager.GetSubscriptions();
            var unSubscriptions = unSubscriptionManager.GetSubscriptions();

            using var fs = fileSystem.File.OpenWrite(target);
            using var writer = new StreamWriter(fs);

            var genres = orfDataProvider.GetGenres().Result;
            var profiles = orfDataProvider.GetProfiles().Result;
            foreach (var profile in profiles.OrderBy(p => p.Title))
            {
                var hasSubscription = subscriptions.Any(s => s.ProfileId == profile.Id);
                if (hasSubscription)
                {
                    writer.Write("s");
                }

                var hasUnSubscriptions = unSubscriptions.Any(s => s.ProfileId == profile.Id);
                if (hasUnSubscriptions)
                {
                    writer.Write("u");
                }

                if (!hasSubscription && !hasUnSubscriptions)
                {
                    writer.Write("n");
                }

                writer.Write(" ");

                var genre = genres.First(g => g.TheLinks.Self.TheHref == profile.Links.Genre.Href);
                writer.WriteLine(profile.Id + " " + profile.Title + " - " + profile.UpdatedAt.ToString("yyyy-MM-dd") + " - " + genre?.Title ?? "?");
            }
        }
    }
}
