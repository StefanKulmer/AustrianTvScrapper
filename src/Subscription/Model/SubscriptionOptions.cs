namespace Subscription.Model
{
    public class SubscriptionOptions
    {
        public string DataDirectory { get; set; }

        public static SubscriptionOptions Default = new SubscriptionOptions
        {
            DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AustrianTvScrapper")
        };
    }
}
