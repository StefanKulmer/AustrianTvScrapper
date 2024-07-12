using DownloadListCreator.Model;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;

namespace DownloadListCreator.Services;

public interface IDirectoryProvider
{
    IDirectoryInfo Queue { get; }
    IDirectoryInfo Succeeded { get; }
    IDirectoryInfo Failed { get; }
}

public class DirectoryProvider : IDirectoryProvider
{
    private readonly IFileSystem _fileSystem;
    private readonly DownloadListOptions _downloadListOptions;

    public DirectoryProvider(IFileSystem fileSystem, IOptions<DownloadListOptions> downloadListOptions)
    {
        if (fileSystem is null)
        {
            throw new ArgumentNullException(nameof(fileSystem));
        }

        if (downloadListOptions is null)
        {
            throw new ArgumentNullException(nameof(DirectoryProvider._downloadListOptions));
        }

        _fileSystem = fileSystem;

        if (downloadListOptions.Value != null)
        {
            _downloadListOptions = downloadListOptions.Value;
        }
        else
        {
            _downloadListOptions = DownloadListOptions.Default;
        }
    }

    public IDirectoryInfo Queue => _fileSystem.DirectoryInfo.New(Path.Combine(_downloadListOptions.RootDirectory, "Queue"));

    public IDirectoryInfo Succeeded => _fileSystem.DirectoryInfo.New(Path.Combine(_downloadListOptions.RootDirectory, "Succeeded"));

    public IDirectoryInfo Failed => _fileSystem.DirectoryInfo.New(Path.Combine(_downloadListOptions.RootDirectory, "Failed"));
}
