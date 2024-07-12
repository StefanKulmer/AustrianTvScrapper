using Downloader.Model;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;

namespace Downloader.Services;

public interface IDirectoryProvider
{
    IDirectoryInfo DownloadDirectory { get; }
    IFileInfo YtDlpFile { get; }
}

public class DirectoryProvider : IDirectoryProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly DownloaderOptions _downloaderOptions;

    public DirectoryProvider(IFileSystem fileSystem, IOptions<DownloaderOptions> downloaderOptions)
    {
        if (fileSystem is null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        if (downloaderOptions is null)
        {
            throw new ArgumentNullException(nameof(downloaderOptions));
        }

        _fileSystem = fileSystem;

        if (downloaderOptions.Value != null)
        {
            _downloaderOptions = downloaderOptions.Value;
        }
        else
        {
            _downloaderOptions = DownloaderOptions.Default;
        }
    }

    public IDirectoryInfo DownloadDirectory => _fileSystem.DirectoryInfo.New(_downloaderOptions.DownloadDirectory);

    public IFileInfo YtDlpFile => _fileSystem.FileInfo.New(_downloaderOptions.YtDlpPath);
}
