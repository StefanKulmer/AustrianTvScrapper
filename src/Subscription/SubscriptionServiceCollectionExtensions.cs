using Microsoft.Extensions.DependencyInjection;
using Subscription.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription
{
    public static class SubscriptionServiceCollectionExtensions
    {
        public static IServiceCollection AddSubscription(this IServiceCollection services)
        {
            services.AddTransient<ISubscriptionManager>(s => new SubscriptionManager("Subscribed.json", s.GetService<ISubscriptionPersistenceService>()));
            services.AddTransient<IUnSubscriptionManager>(s => new SubscriptionManager("UnSubscribed.json", s.GetService<ISubscriptionPersistenceService>()));
            services.AddTransient<ISubscriptionPersistenceService, SubscriptionPersistenceService>();
            services.AddTransient<IDataDirectoryProvider, DataDirectoryProvider>();
            services.AddSingleton<IFileSystem, FileSystem>();

            return services;
        }
    }
}
