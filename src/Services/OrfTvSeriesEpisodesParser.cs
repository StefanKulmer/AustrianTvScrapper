using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesEpisodesParser : IOrfTvSeriesEpisodesParser
    {
        public IReadOnlyCollection<OrfTvSeriesEpisode> Parse(HtmlDocument doc)
        {
            var result = new List<OrfTvSeriesEpisode>();

            var episodeNodes = doc.DocumentNode.Descendants().Where(x => x.HasClass("b-teaser"));
            return episodeNodes.Select(_ParseEpisodeNode).ToList();
        }

        private OrfTvSeriesEpisode _ParseEpisodeNode(HtmlNode episodeNode)
        {
            var episode = new OrfTvSeriesEpisode();

            var linkNode = episodeNode.Descendants().FirstOrDefault(x => x.HasClass("js-teaser-link"));
            episode.Title = System.Net.WebUtility.HtmlDecode(linkNode.Attributes["title"].Value);
            episode.DownloadUrl = linkNode.Attributes["href"].Value;

            var textContainerNode = linkNode.Descendants().FirstOrDefault(x => x.HasClass("text-container"));

            _ReadNode(textContainerNode, "channel", x => x.InnerText, x => episode.Channel = x);
            _ReadNode(textContainerNode, "description", x => x.InnerText.Trim(), x => episode.Description = x);
            _ReadNode(textContainerNode, "visible-duration", x => x.InnerText, x => episode.Duration = x);
            _ReadNode(textContainerNode, "datetime", x => x.Attributes["dateTime"].Value, x => episode.Date = OrfTvSeriesEpisodeDateParser.Parse(x));

            return episode;
        }

        private static void _ReadNode(HtmlNode parentNode, string className, Func<HtmlNode, string> readAction, Action<string> writeAction)
        {
            var node = parentNode.Descendants().FirstOrDefault(x => x.HasClass(className));
            if (node != null)
            {
                var value = readAction(node);
                value = System.Net.WebUtility.HtmlDecode(value.ToString());
                writeAction(value);
            }
        }
    }
}
