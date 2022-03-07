using HtmlAgilityPack;
using System;
using System.Linq;

namespace AustrianTvScrapper.Services
{
    public class OrfTvSeriesParser
    {
        public OrfTvSeries Parse(HtmlNode htmlNode)
        {
            var series = new OrfTvSeries();

            var teaserLink = htmlNode.Descendants().Where(x => x.HasClass("teaser-link")).FirstOrDefault();
            if (teaserLink == null)
            {
                return null;
            }
            
            series.Title = System.Net.WebUtility.HtmlDecode(teaserLink.GetAttributeValue("title", (string)null));
            var lastEpisodeUrl = teaserLink.GetAttributeValue("href", (string)null);
            series.Url = _GetBaseUrl(lastEpisodeUrl);
            series.Id = _GetIdFromUrl(lastEpisodeUrl);


            var textContainer = htmlNode.Descendants().Where(x => x.HasClass("text-container")).FirstOrDefault();
            if (textContainer != null)
            {
                series.Channel = _GetSubElement(textContainer, "channel");
                series.Profile = _GetSubElement(textContainer, "profile");
                series.Description = _GetSubElement(textContainer, "description");
            }

            return series;
        }

        private string _GetIdFromUrl(string lastEpisodeUrl)
        {
            var urlParts = lastEpisodeUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return urlParts[urlParts.Length - 3];
        }

        private string _GetBaseUrl(string lastEpisodeUrl)
        {
            var id = _GetIdFromUrl(lastEpisodeUrl);
            var idStartIndex = lastEpisodeUrl.IndexOf(id);
            return lastEpisodeUrl.Substring(0, idStartIndex + id.Length);
        }

        private string _GetSubElement(HtmlNode htmlNode, string @class)
        {
            var subElement = htmlNode.Descendants().Where(x => x.HasClass(@class)).FirstOrDefault();
            if (subElement != null)
            {
                var text = System.Net.WebUtility.HtmlDecode(subElement.InnerText);
                return text.Trim();
            }

            return null;
        }
    }
}
