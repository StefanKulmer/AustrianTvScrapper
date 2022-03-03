using HtmlAgilityPack;
using System.Collections.Generic;

namespace AustrianTvScrapper.Services
{
    public interface IOrfTvSeriesEpisodesParser
    {
        IReadOnlyCollection<OrfTvSeriesEpisode> Parse(HtmlDocument htmlDocument);
    }
}