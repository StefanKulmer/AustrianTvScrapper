﻿using AustrianTvScrapper.Services;
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
        private readonly IOrfDataProvider _orfDataProvider;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IUnSubscriptionManager _unSubscriptionManager;
        private readonly IFileSystem _fileSystem;

        public ExportSubscriptionsCommand(
            IOrfDataProvider orfDataProvider, 
            Subscription.Services.ISubscriptionManager 
            subscriptionManager, Subscription.Services.IUnSubscriptionManager unSubscriptionManager,
            IFileSystem fileSystem)
            : base("export-subscriptions", "exports all profiles with subscription information. can be used for import")
        {
            _orfDataProvider = orfDataProvider;
            _subscriptionManager = subscriptionManager;
            _unSubscriptionManager = unSubscriptionManager;
            _fileSystem = fileSystem;
            AddOption(new Option<string>(new[] { "--target", "-t" }, getDefaultValue: () => null, "target file for export"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private async void _HandleCommand(string target)
        {
            var subscriptions = _subscriptionManager.GetSubscriptions();
            var unSubscriptions = _unSubscriptionManager.GetSubscriptions();

            using var fs = _fileSystem.File.OpenWrite(target);
            using var writer = new StreamWriter(fs);

            var genres = _orfDataProvider.GetGenres().Result;
            var profiles = _orfDataProvider.GetProfiles().Result;
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
