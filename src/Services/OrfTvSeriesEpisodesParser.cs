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
            episode.Title = linkNode.Attributes["title"].Value;
            episode.DownloadUrl = linkNode.Attributes["href"].Value;

            var textContainerNode = linkNode.Descendants().FirstOrDefault(x => x.HasClass("text-container"));

            _ReadNode(textContainerNode, "channel", x => episode.Channel = x.InnerText);
            _ReadNode(textContainerNode, "description", x => episode.Description = x.InnerText.Trim());
            _ReadNode(textContainerNode, "visible-duration", x => episode.Duration = x.InnerText);
            _ReadNode(textContainerNode, "datetime", x => episode.Date = OrfTvSeriesEpisodeDateParser.Parse(x.Attributes["dateTime"].Value));

            return episode;
        }

        private static void _ReadNode(HtmlNode parentNode, string className, Action<HtmlNode> readAction)
        {
            var node = parentNode.Descendants().FirstOrDefault(x => x.HasClass(className));
            if (node != null)
            {
                readAction(node);
            }
        }
    }
}
