using System;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisode
    {
        public OrfTvSeries TvSeries { get; set; }
        public string DownloadUrl { get; set; }
        public string Channel { get; set; }
        public string Profile { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Duration { get; set; }
    }
}
