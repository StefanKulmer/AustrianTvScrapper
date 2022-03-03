using HtmlAgilityPack;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public class HtmlDocumentLoader : IHtmlDocumentLoader
    {
        public Task<HtmlDocument> LoadDocumentAsync(string url)
        {
            var web = new HtmlWeb();
            return web.LoadFromWebAsync(url);
        }
    }
}
