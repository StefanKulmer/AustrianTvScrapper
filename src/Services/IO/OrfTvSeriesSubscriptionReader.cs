using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AustrianTvScrapper.Services.IO
{
    public class OrfTvSeriesSubscriptionReader
    {
        public IReadOnlyCollection<OrfTvSeriesSubscription> Load(string path)
        {
            var jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<OrfTvSeriesSubscription[]>(jsonString, OrfTvSeriesSubscriptionWriter.JsonOptions);
        }
    }
}
