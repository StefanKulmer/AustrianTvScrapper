using System;
using System.Collections.Generic;
using System.IO;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSubscriptionService
    {
        private readonly IDataDirectoryProvider _dataDirectoryProvider;

        public OrfTvSeriesSubscriptionService(IDataDirectoryProvider dataDirectoryProvider)
        {
            _dataDirectoryProvider = dataDirectoryProvider;
        }

        public void AddSubscription(OrfTvSeriesSubscription subscription)
        {
            var allSubscriptions = _GetSubscriptions();
            var newSubscriptions = OrfTvSeriesSubscription.AddSubscription(allSubscriptions, subscription);

            var path = _GetSubscriptionDataPath();
            var writer = new IO.OrfTvSeriesSubscriptionWriter();
            writer.Save(path, newSubscriptions);
        }

        public IReadOnlyCollection<OrfTvSeriesSubscription> GetSubscriptions()
        {
            return _GetSubscriptions();
        }

        private IReadOnlyCollection<OrfTvSeriesSubscription> _GetSubscriptions()
        {
            string path = _GetSubscriptionDataPath();

            if (!File.Exists(path))
            {
                return new OrfTvSeriesSubscription[0];
            }
            var reader = new IO.OrfTvSeriesSubscriptionReader();
            return reader.Load(path);
        }

        private string _GetSubscriptionDataPath()
        {
            return Path.Combine(_dataDirectoryProvider.GetDataDirectory(), "OrfTvSeriesSubscriptions.json");
        }
    }
}
