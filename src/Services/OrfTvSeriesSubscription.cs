using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSubscription
    {
        public string OrfTvSeriesId { get; set; }
        public OrfTvSeries OrfTvSeries { get; set; }
        public string Name { get; set; }
        public string DownloadSubDirectory { get; set; }
        public DateTime? TimestampCreated { get; set; }

        public static IReadOnlyCollection<OrfTvSeriesSubscription> AddSubscription(IReadOnlyCollection<OrfTvSeriesSubscription> allSubscriptions, OrfTvSeriesSubscription subscription)
        {
            var newList = new List<OrfTvSeriesSubscription>(allSubscriptions);
            if (newList.All(x => x.OrfTvSeriesId != subscription.OrfTvSeriesId))
            {
                newList.Add(subscription);
            }

            return newList;
        }

        public string EpisodeNameFormat { get; set; }
        public string[] EpisodeNameRemovals { get; set; }

        public static OrfTvSeriesSubscription CreateForTvSeries(OrfTvSeries tvSeries)
        {
            return new OrfTvSeriesSubscription()
            {
                OrfTvSeriesId = tvSeries.Id,
                OrfTvSeries = tvSeries,
                Name = tvSeries.Title,
                TimestampCreated = DateTime.Now
            };
        }
    }
}
