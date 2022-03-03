namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesUrlProvider
    {
        string GetEpisodesUrl(OrfTvSeries orfTvSeries);
    }
}
