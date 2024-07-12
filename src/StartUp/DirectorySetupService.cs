using Microsoft.Extensions.Hosting;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using IDirectoryProvider = DownloadListCreator.Services.IDirectoryProvider;

namespace AustrianTvScrapper.StartUp
{
    internal class DirectorySetupService : IHostedService
    {
        private readonly IDirectoryProvider _directoryProvider;

        public DirectorySetupService(IDirectoryProvider directoryProvider)
        {
            _directoryProvider = directoryProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _EnsureExists(_directoryProvider.Succeeded);
            _EnsureExists(_directoryProvider.Failed);
            _EnsureExists(_directoryProvider.Queue);

            return Task.CompletedTask;
        }

        private void _EnsureExists(IDirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
