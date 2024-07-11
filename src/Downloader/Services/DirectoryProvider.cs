using Microsoft.Extensions.Options;
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
        private readonly DirectoryOptions directoryOptions;

        public DirectoryProvider(IFileSystem fileSystem, IOptions<DirectoryOptions> directoryOptions)
        {
            if (fileSystem is null)
            {
                throw new ArgumentNullException(nameof(fileSystem));
            }

            if (directoryOptions is null)
            {
                throw new ArgumentNullException(nameof(directoryOptions));
            }

            this.fileSystem = fileSystem;
            this.directoryOptions = directoryOptions.Value;
        }

        public IDirectoryInfo Queue => fileSystem.DirectoryInfo.New(Path.Combine(directoryOptions.DownloadListDirectory, "Queue"));

        public IDirectoryInfo Succeeded => fileSystem.DirectoryInfo.New(Path.Combine(directoryOptions.DownloadListDirectory, "Succeeded"));

        public IDirectoryInfo Failed => fileSystem.DirectoryInfo.New(Path.Combine(directoryOptions.DownloadListDirectory, "Failed"));

        public IDirectoryInfo DownloadDirectory => fileSystem.DirectoryInfo.New(directoryOptions.DownloadDirectory);
    }
}
