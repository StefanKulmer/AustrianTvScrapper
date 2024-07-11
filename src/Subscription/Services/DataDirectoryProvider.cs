using System.IO.Abstractions;

namespace Subscription.Services
{
    internal interface IDataDirectoryProvider
    {
        IDirectoryInfo GetDataDirectory();
    }

    public class DataDirectoryProvider : IDataDirectoryProvider
    {
        private readonly IFileSystem fileSystem;

        public DataDirectoryProvider(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        public IDirectoryInfo GetDataDirectory()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AustrianTvScrapper");
            var pathExpanded = Environment.ExpandEnvironmentVariables(path);
            return fileSystem.DirectoryInfo.New(path);
        }
    }
}
