using DownloadListCreator;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class PrepareDownloadsCommand : Command
    {
        private readonly IDownloadListCreator downloadListCreator;

        public PrepareDownloadsCommand(DownloadListCreator.IDownloadListCreator downloadListCreator)
            : base("preparedl", "queues new found episodes in the downloader list")
        {
            this.downloadListCreator = downloadListCreator;
            Handler = CommandHandler.Create(_HandleCommand);
        }

        private void _HandleCommand()
        {
            downloadListCreator.Create();
        }
    }
}
