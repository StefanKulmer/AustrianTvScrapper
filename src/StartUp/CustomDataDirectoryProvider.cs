using AustrianTvScrapper.Services;

namespace AustrianTvScrapper.StartUp
{
    public class CustomDataDirectoryProvider : IDataDirectoryProvider
    {
        private readonly string directory;

        public CustomDataDirectoryProvider(string directory)
        {
            this.directory = directory;
        }

        public string GetDataDirectory()
        {
            return directory;
        }
    }
}
