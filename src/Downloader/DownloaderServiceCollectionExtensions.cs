using Downloader.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadListCreator
{
    public static class DownloaderServiceCollectionExtensions
    {
        public static IServiceCollection AddDownloader(this IServiceCollection services)
        {
            services.AddTransient<IDownloader, Downloader.Services.Downloader>();
            services.AddTransient<IDirectoryProvider, DirectoryProvider>();
            services.AddTransient<IDataDownloader, DataDownloader>();

            return services;
        }
    }
}
