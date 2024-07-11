namespace Subscription.Services
{

    internal class SubscriptionManager : IUnSubscriptionManager
    {
        private readonly string fileName;
        private readonly ISubscriptionPersistenceService subscriptionPersistenceService;

        public SubscriptionManager(string fileName, ISubscriptionPersistenceService subscriptionPersistenceService)
        {
            this.fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            this.subscriptionPersistenceService = subscriptionPersistenceService;
        }

        public void AddSubscription(Model.Subscription subscription)
        {
            var allSubscriptions = subscriptionPersistenceService.LoadAll(fileName);
            if (allSubscriptions.Any(s => s.ProfileId == subscription.ProfileId))
            {
                return;
            }
            
            int maxId = 0;
            if (allSubscriptions.Count > 0)
            {
                maxId = allSubscriptions.Max(s => s.Id);
            }
            
            subscription.Id = maxId + 1;

            allSubscriptions.Add(subscription);

            subscriptionPersistenceService.SaveAll(fileName, allSubscriptions);
        }

        public IReadOnlyCollection<Model.Subscription> GetSubscriptions()
        {
            return subscriptionPersistenceService.LoadAll(fileName);
        }

        public void RemoveSubscription(Model.Subscription subscription)
        {
            var allSubscriptions = subscriptionPersistenceService.LoadAll(fileName);
            var existingSubscription = allSubscriptions.FirstOrDefault(s => s.ProfileId == subscription.ProfileId);
            if (existingSubscription == null)
            {
                return;
            }

            allSubscriptions.Remove(existingSubscription);
            subscriptionPersistenceService.SaveAll(fileName, allSubscriptions);
        }
    }
}
