using DownloadListCreator.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadListCreator
{
    public static class DownloadListCreatorServiceCollectionExtensions
    {
        public static IServiceCollection AddDownloadListCreator(this IServiceCollection services)
        {
            services.AddTransient<IDirectoryProvider, DirectoryProvider>();
            services.AddTransient<IDownloadsProvider, DownloadsProvider>();
            services.AddTransient<IDownloadsProvider, DownloadsProvider>();
            services.AddTransient<IDownloadListCreator, Services.DownloadListCreator>();

            return services;
        }
    }
}
