using Downloader.Services;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class StartDownloadCommand : Command
    {
        private readonly IDownloader _downloader;

        public StartDownloadCommand(IDownloader downloader)
            : base("startdownload", "starts download of queued episodes")
        {
            _downloader = downloader;

            this.SetHandler(() => _downloader.Start().GetAwaiter().GetResult());
        }

        private void _HandleCommand()
        {
            _downloader.Start().Wait();
        }
    }
}
