namespace Subscription.Services
{
    public interface ISubscriptionManager
    {
        IReadOnlyCollection<Model.Subscription> GetSubscriptions();
        void AddSubscription(Model.Subscription subscription);
        void RemoveSubscription(Model.Subscription subscription);
    }
}
