using System;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisode
    {
        public OrfTvSeries TvSeries { get; set; }

        public DateTime Date { get; set; }

        public string DownloadUrl { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Description2 { get; set; }
    }
}
