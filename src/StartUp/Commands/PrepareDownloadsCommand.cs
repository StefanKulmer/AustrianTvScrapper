using DownloadListCreator.Services;
using System.CommandLine;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class PrepareDownloadsCommand : Command
    {
        private readonly IDownloadListCreator _downloadListCreator;

        public PrepareDownloadsCommand(IDownloadListCreator downloadListCreator)
            : base("preparedl", "queues new found episodes in the downloader list")
        {
            _downloadListCreator = downloadListCreator;
            this.SetAction(_ =>
            {
                _downloadListCreator.Create();
                return Task.CompletedTask;
            });
        }

        private void _HandleCommand()
        {
            _downloadListCreator.Create();
        }
    }
}
