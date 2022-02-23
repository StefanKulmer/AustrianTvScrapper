using System.Collections.Generic;

namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesScrapper
    {
        IReadOnlyCollection<OrfTvSeries> GetListOfTvSeries();
    }
}
