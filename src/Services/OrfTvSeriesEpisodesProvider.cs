using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodesProvider
    {
        public IEnumerable<OrfTvSeriesEpisode> GetAvailableEpisodes(OrfTvSeries tvSeries)
        {
            var episodesUrl = string.Format("{0}/episodes", tvSeries.Url);
            var web = new HtmlWeb();
            var doc = web.Load(episodesUrl);

            var episodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("b-teaser"));

            foreach (var x in episodes)
            {
                var linkNode = x.Descendants().FirstOrDefault(x => x.HasClass("js-teaser-link"));

                var dateTimeNode = x.Descendants().FirstOrDefault(x => string.Compare(x.Name, "time", StringComparison.OrdinalIgnoreCase) == 0);
                var dateTimeString = dateTimeNode.Attributes["datetime"].Value.Replace("CET", " ").Replace("CEST", " ");
                var date = DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                yield return new OrfTvSeriesEpisode()
                {
                    TvSeries = tvSeries,
                    Date = date,
                    DownloadUrl = linkNode.Attributes["href"].Value,
                    Title = linkNode.Attributes["title"].Value
                };
            }
        }
    }
}
