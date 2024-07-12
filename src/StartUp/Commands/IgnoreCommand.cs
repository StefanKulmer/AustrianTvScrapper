using AustrianTvScrapper.Services;
using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class IgnoreCommand : Command
    {
        private readonly IOrfTvSeriesScrapper _orfTvSeriesScrapper;
        private readonly IUnSubscriptionManager _unSubscriptionManager;
        private readonly IOrfDataProvider _orfDataProvider;

        public IgnoreCommand(Subscription.Services.IUnSubscriptionManager unSubscriptionManager, IOrfDataProvider orfDataProvider)
            : base("ignore", "ignores a series")
        {
            AddOption(new Option<int>(new[] { "--id", "-id" }, "id of TV show"));

            _unSubscriptionManager = unSubscriptionManager;
            _orfDataProvider = orfDataProvider;

            Handler = CommandHandler.Create<int>(_HandleCommand);
        }

        private void _HandleCommand(int id)
        {
            var profile = _orfDataProvider.GetProfile(id).Result;
            if (profile == null)
            {
                Console.WriteLine($"profile {id} doesn't exist.");
                return;
            }

            var subscriptions = _unSubscriptionManager.GetSubscriptions();
            if (subscriptions.Any(s => s.ProfileId == id))
            {
                Console.WriteLine($"unsubscription for {id} {profile.Title} already exists.");
                return;
            }

            var subscription = new Subscription.Model.Subscription()
            {
                ProfileId = id,
                Name = profile.Title,
                Created = DateTime.Now,
            };

            _unSubscriptionManager.AddSubscription(subscription);

            Console.WriteLine($"unsubscription for {id} {profile.Title} added.");
        }
    }
}
