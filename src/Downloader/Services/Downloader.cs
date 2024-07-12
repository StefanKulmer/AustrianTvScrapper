using DownloadListCreator.Model;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Json;

namespace Downloader.Services
{
    public interface IDownloader
    {
        void Start();
    }

    public class Downloader : IDownloader
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryProvider _downloaderDirectoryProvider;
        private readonly DownloadListCreator.Services.IDirectoryProvider _downloadListDirectoryProvider;

        public Downloader(IFileSystem fileSystem, IDirectoryProvider downloaderDirectoryProvider, DownloadListCreator.Services.IDirectoryProvider downloadListDirectoryProvider)
        {
            _fileSystem = fileSystem;
            _downloaderDirectoryProvider = downloaderDirectoryProvider;
            _downloadListDirectoryProvider = downloadListDirectoryProvider;
        }

        public void Start()
        {
            var queueDirectory = _downloadListDirectoryProvider.Queue;
            while (true)
            {
                var files = queueDirectory.GetFiles("*.json");
                if (files.Length == 0)
                    break;

                var firstFile = files.OrderBy(f => f.Name).FirstOrDefault();

                Download? download;
                using (FileStream openStream = File.OpenRead(firstFile.FullName))
                {
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        WriteIndented = true
                    };
                    download = JsonSerializer.Deserialize<Download>(openStream, serializeOptions);
                }
                    
                if (download == null)
                {
                    firstFile.MoveTo(_downloadListDirectoryProvider.Failed.FullName);
                    continue;
                }

                var rootedDirectory = Path.Combine(_downloaderDirectoryProvider.DownloadDirectory.FullName, download.Directory);
                var downloadDirectory = _fileSystem.DirectoryInfo.New(rootedDirectory);
                if (downloadDirectory.Exists)
                {
                    downloadDirectory.Delete(true);
                }
                downloadDirectory.Create();

                var processStartInfo = new ProcessStartInfo()
                {
                    //Arguments = $"--write-description --write-annotations --write-all-thumbnails --write-info-json --limit-rate 500K {download.Url}",
                    Arguments = $"--write-description --write-annotations --write-all-thumbnails --write-info-json {download.Url}",
                    FileName = @"C:\Users\Stefan\Downloads\yt-dlp.exe",
                    WorkingDirectory = downloadDirectory.FullName
                };
                processStartInfo.UseShellExecute = false;
                var p = new Process()
                { StartInfo = processStartInfo };
                download.DownloadStartedDate = DateTime.Now;
                p.Start();
                p.WaitForExit();
                download.DownloadFinishedDate = DateTime.Now;

                // Create a file stream for writing
                using (var createStream = firstFile.OpenWrite())
                {
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        WriteIndented = true
                    };
                    JsonSerializer.Serialize<Download>(createStream, download, serializeOptions);
                }
                    
                if (p.ExitCode == 0)
                {
                    firstFile.CopyTo(_fileSystem.Path.Combine(downloadDirectory.FullName, firstFile.Name));
                    firstFile.MoveTo(_fileSystem.Path.Combine(_downloadListDirectoryProvider.Succeeded.FullName, firstFile.Name));
                }
                else
                {
                    firstFile.MoveTo(_fileSystem.Path.Combine(_downloadListDirectoryProvider.Failed.FullName, firstFile.Name), true);
                    downloadDirectory.Delete(true);
                }
            }
        }
    }


}
