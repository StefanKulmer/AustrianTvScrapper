using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodesProvider : IOrfTvSeriesEpisodesProvider
    {
        private readonly IOrfTvSeriesUrlProvider urlProvider;
        private readonly IHtmlDocumentLoader htmlDocumentLoader;
        private readonly IOrfTvSeriesEpisodesParser epsiodesParser;

        public OrfTvSeriesEpisodesProvider(IOrfTvSeriesUrlProvider urlProvider, IHtmlDocumentLoader htmlDocumentLoader, IOrfTvSeriesEpisodesParser episodesParser)
        {
            this.urlProvider = urlProvider;
            this.htmlDocumentLoader = htmlDocumentLoader;
            epsiodesParser = episodesParser;
        }

        public async Task<IReadOnlyCollection<OrfTvSeriesEpisode>> GetEpisodesAsync(OrfTvSeries orfTvSeries)
        {
            ArgumentNullException.ThrowIfNull(orfTvSeries);

            var url = urlProvider.GetEpisodesUrl(orfTvSeries);
            var htmlDocument = await htmlDocumentLoader.LoadDocumentAsync(url);
            return epsiodesParser.Parse(htmlDocument);
        }
    }
}
