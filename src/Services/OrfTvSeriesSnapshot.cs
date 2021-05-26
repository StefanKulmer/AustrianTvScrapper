using System;
using System.Collections.Generic;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSnapshot
    {
        public DateTime Timestamp { get; set; }

        public List<OrfTvSeries> OrfTvSeries { get; set; }
    }
}
