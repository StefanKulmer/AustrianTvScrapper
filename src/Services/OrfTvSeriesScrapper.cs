using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesScrapper : IOrfTvSeriesScrapper, IUncachedService<IOrfTvSeriesScrapper>
    {
        public IOrfTvSeriesScrapper Instance => this;

        public IReadOnlyCollection<OrfTvSeries> GetListOfTvSeries()
        {
            var rootUrl = "https://tvthek.orf.at/profiles";

            var web = new HtmlWeb();
            var doc = web.Load(rootUrl);

            var tvSeriesElements = doc.DocumentNode.Descendants().Where(x => x.HasClass("b-teaser"));
            
            var parser = new OrfTvSeriesParser();
            return tvSeriesElements.Select(x => parser.Parse(x)).Where(x => x != null).ToList();
        }
    }
}
