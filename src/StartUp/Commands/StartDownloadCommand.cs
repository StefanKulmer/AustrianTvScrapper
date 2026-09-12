using Downloader.Services;
using System.CommandLine;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class StartDownloadCommand : Command
    {
        private readonly IDownloader _downloader;

        public StartDownloadCommand(IDownloader downloader)
            : base("startdownload", "starts download of queued episodes")
        {
            _downloader = downloader;

            this.SetAction(async _ => await _downloader.Start().ConfigureAwait(false));
        }
    }
}
