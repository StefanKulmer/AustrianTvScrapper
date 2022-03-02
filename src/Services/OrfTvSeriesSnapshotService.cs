using AustrianTvScrapper.Services.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesSnapshotService : IOrfTvSeriesSnapshotService
    {
        private const string SnapshotPrefix = "OrfTvSeries_Snapshot";

        private readonly IDataDirectoryProvider _dataDirectoryProvider;
        private readonly IOrfTvSeriesScrapper _orfTvSeriesScrapper;

        public OrfTvSeriesSnapshotService(IDataDirectoryProvider dataDirectoryProvider, IUncachedService<IOrfTvSeriesScrapper> orfTvSeriesScrapper)
        {
            _dataDirectoryProvider = dataDirectoryProvider;
            _orfTvSeriesScrapper = orfTvSeriesScrapper.Instance;
        }

        public IReadOnlyCollection<OrfTvSeries> CreateSnapshot()
        {
            var tvSeries = _orfTvSeriesScrapper.GetListOfTvSeries();

            CreateSnapshot(tvSeries);

            return tvSeries;
        }

        public string CreateSnapshot(IReadOnlyCollection<OrfTvSeries> tvSeries)
        {
            var snapshot = new OrfTvSeriesSnapshot()
            {
                Timestamp = DateTime.Now,
                OrfTvSeries = new List<OrfTvSeries>(tvSeries)
            };

            var fileName = _GetSnapshotFileName(snapshot.Timestamp);
            var path = Path.Combine(_dataDirectoryProvider.GetDataDirectory(), fileName);

            var writer = new OrfTvSeriesSnapshotWriter();
            writer.Save(path, snapshot);

            return fileName;
        }

        public OrfTvSeriesSnapshot GetLastSnapshot()
        {
            var file = Directory.GetFiles(_dataDirectoryProvider.GetDataDirectory(), $"{SnapshotPrefix}*.json").OrderBy(x => x).LastOrDefault();
            if (file == null)
                return null;

            var reader = new OrfTvSeriesSnapshotReader();
            return reader.Load(file);
        }

        public OrfTvSeriesSnapshot ReadSnapshot(string filename)
        {
            var path = Path.Combine(_dataDirectoryProvider.GetDataDirectory(), filename);

            var reader = new OrfTvSeriesSnapshotReader();

            return reader.Load(path);
        }

        private string _GetSnapshotFileName(DateTime timestamp)
        {
            return $"{SnapshotPrefix}_{timestamp:yyyyMMdd_HHmmss}.json";
        }
    }
}
