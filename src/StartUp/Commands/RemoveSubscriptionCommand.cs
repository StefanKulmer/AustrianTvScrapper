using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class RemoveSubscriptionCommand : Command
    {
        private readonly ISubscriptionManager subscriptionManager;

        public RemoveSubscriptionCommand(Subscription.Services.ISubscriptionManager subscriptionManager)
            : base("remove-subscription", "removes a subscription")
        {
            this.subscriptionManager = subscriptionManager;
            AddOption(new Option<int>(new[] { "--id", "-id" }, "id of TV show"));

            Handler = CommandHandler.Create<int, string>(_HandleCommand);
        }

        private void _HandleCommand(int id, string downloadSubDirectory)
        {
            var subscriptions = subscriptionManager.GetSubscriptions();
            var subscription = subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription == null)
            {
                Console.WriteLine($"subscription for {id} doesn't exist.");
                return;
            }

            subscriptionManager.RemoveSubscription(subscription);
            Console.WriteLine($"removed subscription for {id}.");
        }
    }
}
