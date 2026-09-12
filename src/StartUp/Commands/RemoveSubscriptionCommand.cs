using OrfDataProvider.Services;
using Subscription.Services;
using System;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class RemoveSubscriptionCommand : Command
    {
        private readonly ISubscriptionManager _subscriptionManager;

        public RemoveSubscriptionCommand(Subscription.Services.ISubscriptionManager subscriptionManager)
            : base("remove-subscription", "removes a subscription")
        {
            _subscriptionManager = subscriptionManager;
            var idOption = new Option<int>("--id", "-id") { Description = "id of TV show" };
            Add(idOption);

            this.SetAction(parseResult =>
            {
                _HandleCommand(parseResult.GetValue(idOption));
                return Task.CompletedTask;
            });
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
