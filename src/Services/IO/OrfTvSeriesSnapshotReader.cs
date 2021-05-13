using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AustrianTvScrapper.Services.IO
{
    public class OrfTvSeriesSnapshotReader
    {
        public OrfTvSeriesSnapshot Load(string path)
        {
            var jsonString = File.ReadAllText(path);
            return JsonSerializer.Deserialize<OrfTvSeriesSnapshot>(jsonString, OrfTvSeriesSnapshotWriter.JsonOptions);
        }
    }
}
