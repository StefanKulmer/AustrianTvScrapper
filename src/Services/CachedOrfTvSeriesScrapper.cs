using AustrianTvScrapper.Services.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class CachedOrfTvSeriesScrapper : IOrfTvSeriesScrapper
    {
        private readonly IOrfTvSeriesSnapshotService snapshotService;

        public CachedOrfTvSeriesScrapper(IOrfTvSeriesSnapshotService snapshotService)
        {
            this.snapshotService = snapshotService;
        }

        public IReadOnlyCollection<OrfTvSeries> GetListOfTvSeries()
        {
            var lastSnapshot = snapshotService.GetLastSnapshot();
            if (_IsSnapshotValid(lastSnapshot))
            {
                return lastSnapshot.OrfTvSeries;
            }

            var newSnapshot = snapshotService.CreateSnapshot();
            return newSnapshot;
        }

        private bool _IsSnapshotValid(OrfTvSeriesSnapshot lastSnapshot)
        {
            if (lastSnapshot == null)
                return false;

            var old = DateTime.Now - lastSnapshot.Timestamp;
            return old.TotalMinutes < 15;
        }
    }
}
