using Downloader.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp
{
    internal class DirectorySetupService : IHostedService
    {
        private readonly IDirectoryProvider directoryProvider;

        public DirectorySetupService(IDirectoryProvider directoryProvider)
        {
            this.directoryProvider = directoryProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _EnsureExists(directoryProvider.Succeeded);
            _EnsureExists(directoryProvider.Failed);
            _EnsureExists(directoryProvider.Queue);
            _EnsureExists(directoryProvider.DownloadDirectory);
            _EnsureExists(directoryProvider.SubscriptionsDirectory);

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
