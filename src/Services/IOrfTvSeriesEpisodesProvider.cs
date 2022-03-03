using System.Collections.Generic;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesEpisodesProvider
    {
        Task<IReadOnlyCollection<OrfTvSeriesEpisode>> GetEpisodesAsync(OrfTvSeries orfTvSeries);
    }
}
