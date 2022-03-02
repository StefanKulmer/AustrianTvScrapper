using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public static class OrfTvSeriesEpisodesParser
    {
        public static IReadOnlyCollection<OrfTvSeriesEpisode> Parse(HtmlDocument doc)
        {
            var result = new List<OrfTvSeriesEpisode>();

            var episodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("b-teaser"));

            foreach (var x in episodes)
            {
                var linkNode = x.Descendants().FirstOrDefault(x => x.HasClass("js-teaser-link"));

                var dateTimeNode = x.Descendants().FirstOrDefault(x => string.Compare(x.Name, "time", StringComparison.OrdinalIgnoreCase) == 0);
                var dateTimeString = dateTimeNode.Attributes["datetime"].Value.Replace("CET", " ").Replace("CEST", " ");
                var date = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                var orfEpisode = new OrfTvSeriesEpisode()
                {
                    Date = date,
                    DownloadUrl = linkNode.Attributes["href"].Value,
                    Title = linkNode.Attributes["title"].Value
                };
                result.Add(orfEpisode);
            }

            return result;
        }
    }
}
