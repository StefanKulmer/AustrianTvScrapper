using Microsoft.Extensions.DependencyInjection;

namespace DownloadListCreator
{
    public static class DownloadListCreatorServiceCollectionExtensions
    {
        public static IServiceCollection AddDownloadListCreator(this IServiceCollection services)
        {
            services.AddTransient<Downloader.Services.IDirectoryProvider, Downloader.Services.DirectoryProvider>();
            services.AddTransient<Downloader.Services.IDownloadsProvider, Downloader.Services.DownloadsProvider>();
            services.AddTransient<Downloader.Services.IDownloader, Downloader.Services.Downloader>();
            services.AddTransient<OrfDataProvider.Services.IOrfDataProvider, OrfDataProvider.Services.OrfDataProvider>();
            services.AddTransient<IDownloadListCreator, DownloadListCreator>();

            return services;
        }
    }
}
