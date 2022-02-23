using System.Collections.Generic;

namespace AustrianTvScrapper.Services
{
    internal interface IOrfTvSeriesScrapper
    {
        IReadOnlyCollection<OrfTvSeries> GetListOfTvSeries();
    }
}
