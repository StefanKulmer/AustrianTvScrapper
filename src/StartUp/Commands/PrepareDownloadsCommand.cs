using DownloadListCreator.Services;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class PrepareDownloadsCommand : Command
    {
        private readonly IDownloadListCreator _downloadListCreator;

        public PrepareDownloadsCommand(IDownloadListCreator downloadListCreator)
            : base("preparedl", "queues new found episodes in the downloader list")
        {
            _downloadListCreator = downloadListCreator;
            Handler = CommandHandler.Create(_HandleCommand);
        }

        private void _HandleCommand()
        {
            _downloadListCreator.Create();
        }
    }
}
