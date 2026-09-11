using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class RemoveSubscriptionCommand : Command
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public RemoveSubscriptionCommand(Subscription.Services.ISubscriptionManager subscriptionManager)
            : base("remove-subscription", "removes a subscription")
        {
            _subscriptionManager = subscriptionManager;
            var idOption = new Option<int>(new[] { "--id", "-id" }) { Description = "id of TV show" };
            AddOption(idOption);

            this.SetHandler((int id) => _HandleCommand(id), idOption);
        }

        private void _HandleCommand(int id)
        {
            var subscriptions = _subscriptionManager.GetSubscriptions();
            var subscription = subscriptions.FirstOrDefault(s => s.Id == id);
            if (subscription == null)
            {
                Console.WriteLine($"subscription for {id} doesn't exist.");
                return;
            }

            _subscriptionManager.RemoveSubscription(subscription);
            Console.WriteLine($"removed subscription for {id}.");
        }
    }
}
