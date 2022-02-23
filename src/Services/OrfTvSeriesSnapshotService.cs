using AustrianTvScrapper.Services.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSnapshotService
    {
        private readonly IDataDirectoryProvider _dataDirectoryProvider;
        private readonly IOrfTvSeriesScrapper _orfTvSeriesScrapper;

        public OrfTvSeriesSnapshotService(IDataDirectoryProvider dataDirectoryProvider, IOrfTvSeriesScrapper orfTvSeriesScrapper)
        {
            _dataDirectoryProvider = dataDirectoryProvider;
            _orfTvSeriesScrapper = orfTvSeriesScrapper;
        }

        public void CreateSnapshot()
        {
            var tvSeries = _orfTvSeriesScrapper.GetListOfTvSeries();

            CreateSnapshot(tvSeries);
        }

        public void CreateSnapshot(IReadOnlyCollection<OrfTvSeries> tvSeries)
        {
            var snapshot = new OrfTvSeriesSnapshot()
            {
                Timestamp = DateTime.Now,
                OrfTvSeries = new List<OrfTvSeries>(tvSeries)
            };

            var path = _GetSnapshotPath(snapshot.Timestamp);
            var writer = new OrfTvSeriesSnapshotWriter();
            writer.Save(path, snapshot);
        }

        public OrfTvSeriesSnapshot ReadSnapshot(string filename)
        {
            var path = Path.Combine(_dataDirectoryProvider.GetDataDirectory(), filename);

            var reader = new OrfTvSeriesSnapshotReader();

            return reader.Load(path);
        }

        private string _GetSnapshotPath(DateTime timestamp)
        {
            return Path.Combine(_dataDirectoryProvider.GetDataDirectory(), $"OrfTvSeries_{timestamp:yyyyMMdd_HHmmss}.json");
        }
    }
}
