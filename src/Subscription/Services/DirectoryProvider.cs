using Microsoft.Extensions.Options;
using Subscription.Model;
using System.IO.Abstractions;

namespace Subscription.Services
{
    internal interface IDirectoryProvider
    {
        IDirectoryInfo GetDataDirectory();
    }

    public class DirectoryProvider : IDirectoryProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly SubscriptionOptions _subscriptionOptions;

        public DirectoryProvider(IFileSystem fileSystem, IOptions<SubscriptionOptions> subscriptionOptions)
        {
            _fileSystem = fileSystem;
            _subscriptionOptions = subscriptionOptions.Value;
        }

        public IDirectoryInfo GetDataDirectory()
        {
            return _fileSystem.DirectoryInfo.New(_subscriptionOptions.DataDirectory);
        }
    }
}
