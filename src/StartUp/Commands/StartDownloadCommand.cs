using Downloader.Services;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class StartDownloadCommand : Command
    {
        private readonly IDownloader _downloader;

        public StartDownloadCommand(IDownloader downloader)
            : base("startdownload", "starts download of queued episodes")
        {
            _downloader = downloader;

            Handler = CommandHandler.Create(_HandleCommand);
        }

        private void _HandleCommand()
        {
            _downloader.Start();
        }
    }
}
