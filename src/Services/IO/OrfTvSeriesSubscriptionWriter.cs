using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AustrianTvScrapper.Services.IO
{
    class OrfTvSeriesSubscriptionWriter
    {
        public static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        public void Save(string path, IReadOnlyCollection<OrfTvSeriesSubscription> data)
        {
            var json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, json);
        }
    }
}
