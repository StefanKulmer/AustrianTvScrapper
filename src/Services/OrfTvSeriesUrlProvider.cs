namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesUrlProvider : IOrfTvSeriesUrlProvider
    {
        public string GetEpisodesUrl(OrfTvSeries orfTvSeries)
        {
            return string.Format("{0}/episodes", orfTvSeries.Url);
        }
    }
}
