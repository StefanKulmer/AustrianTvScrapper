using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Downloader.Services
{
    public interface  IDirectoryProvider
    {
        IDirectoryInfo Queue { get; }
        IDirectoryInfo Succeeded { get; }
        IDirectoryInfo Failed { get; }
        IDirectoryInfo DownloadDirectory { get; }
    }

    public class DirectoryProvider : IDirectoryProvider
    {
        private readonly IFileSystem fileSystem;

        public DirectoryProvider(IFileSystem fileSystem)
        {
            if (fileSystem is null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }
            
            this.fileSystem = fileSystem;
        }

        public IDirectoryInfo Queue => fileSystem.DirectoryInfo.New(@"C:\Users\Stefan\Documents\AustrianTvScrapper\Queue");

        public IDirectoryInfo Succeeded => fileSystem.DirectoryInfo.New(@"C:\Users\Stefan\Documents\AustrianTvScrapper\Succeeded");

        public IDirectoryInfo Failed => fileSystem.DirectoryInfo.New(@"C:\Users\Stefan\Documents\AustrianTvScrapper\Failed");

        public IDirectoryInfo DownloadDirectory => fileSystem.DirectoryInfo.New(@"I:\03 Movies\Tv\2024-temp\downloads");
    }
}
