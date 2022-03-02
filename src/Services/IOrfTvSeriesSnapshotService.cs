using System.Collections.Generic;

namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesSnapshotService
    {
        IReadOnlyCollection<OrfTvSeries> CreateSnapshot();
        string CreateSnapshot(IReadOnlyCollection<OrfTvSeries> tvSeries);
        OrfTvSeriesSnapshot ReadSnapshot(string filename);
        OrfTvSeriesSnapshot GetLastSnapshot();
    }
}