using AustrianTvScrapper.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains the collection extensions for adding the CLI commands.
    /// </summary>
    public static class OrfTvSeriesCollectionExtensions
    {
        /// <summary>
        /// Adds the OrfTvSeries services to the DI container.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection, for chaining.</returns>
        public static IServiceCollection AddOrfTvSeriesCommands(this IServiceCollection services)
        {
            services.AddScoped<IOrfTvSeriesScrapper, CachedOrfTvSeriesScrapper>();
            services.AddScoped<IUncachedService<IOrfTvSeriesScrapper>, OrfTvSeriesScrapper>();
            services.AddScoped<IOrfTvSeriesSnapshotService, OrfTvSeriesSnapshotService>();
            services.AddScoped<IDataDirectoryProvider, UserDocumentsDataDirectoryProvider>();

            services.AddScoped<IHtmlDocumentLoader, HtmlDocumentLoader>();
            services.AddScoped<IOrfTvSeriesUrlProvider, OrfTvSeriesUrlProvider>();
            services.AddScoped<IOrfTvSeriesEpisodesParser, OrfTvSeriesEpisodesParser>();
            services.AddScoped<IOrfTvSeriesEpisodesProvider, OrfTvSeriesEpisodesProvider>();


            return services;
        }
    }
}
