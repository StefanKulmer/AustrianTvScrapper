using HtmlAgilityPack;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public interface IHtmlDocumentLoader
    {
        Task<HtmlDocument> LoadDocumentAsync(string url);
    }
}