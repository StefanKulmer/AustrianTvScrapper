using DownloadListCreator.Model;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;

namespace Downloader.Services
{
    public interface IDownloader
    {
        Task Start();
    }

    public class Downloader : IDownloader
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryProvider _downloaderDirectoryProvider;
        private readonly DownloadListCreator.Services.IDirectoryProvider _downloadListDirectoryProvider;
        private readonly IDataDownloader _dataDownloader;

        public Downloader(IFileSystem fileSystem, IDirectoryProvider downloaderDirectoryProvider, DownloadListCreator.Services.IDirectoryProvider downloadListDirectoryProvider, IDataDownloader dataDownloader)
        {
            _fileSystem = fileSystem;
            _downloaderDirectoryProvider = downloaderDirectoryProvider;
            _downloadListDirectoryProvider = downloadListDirectoryProvider;
            _dataDownloader = dataDownloader;
        }

        public async Task Start()
        {
            var queueDirectory = _downloadListDirectoryProvider.Queue;
            while (true)
            {
                var files = queueDirectory.GetFiles("*.json");
                if (files.Length == 0)
                    break;

                var firstFile = files.OrderBy(f => f.Name).FirstOrDefault();
                if (firstFile == null)
                    continue;

                var download = _GetDownload(firstFile);

                if (download == null)
                {
                    _MoveToFailed(firstFile);
                    continue;
                }

                var rootedDirectory = Path.Combine(_downloaderDirectoryProvider.DownloadDirectory.FullName, download.Directory);
                var downloadDirectory = _fileSystem.DirectoryInfo.New(rootedDirectory);
                if (downloadDirectory.Exists)
                {
                    downloadDirectory.Delete(true);
                }
                downloadDirectory.Create();

                string workingDirectory = _fileSystem.Path.Combine(_fileSystem.Path.GetTempPath(), Guid.NewGuid().ToString());
                var workingDirectoryInfo = _fileSystem.Directory.CreateDirectory(workingDirectory);

                var processStartInfo = new ProcessStartInfo()
                {
                    //Arguments = $"--write-description --write-annotations --write-all-thumbnails --write-info-json --limit-rate 500K {download.Url}",
                    Arguments = $"--write-description --windows-filenames--restrict-filenames --write-all-thumbnails --write-info-json {download.Url}",
                    //Arguments = $"--write-description --compat-options filename-sanitization --write-all-thumbnails {download.Url}",
                    FileName = _downloaderDirectoryProvider.YtDlpFile.FullName,
                    WorkingDirectory = workingDirectory,
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
                    foreach (var fileInfo in workingDirectoryInfo.GetFiles())
                    {
                        fileInfo.MoveTo(_fileSystem.Path.Combine(downloadDirectory.FullName, fileInfo.Name));
                    }

                    var mainFileName = _GetMainFileName(downloadDirectory);
                    var mainFileNameWithoutExtension = _fileSystem.Path.GetFileNameWithoutExtension(mainFileName.Name);
                    var downloadFileName = $"{mainFileNameWithoutExtension}.download{firstFile.Extension}";

                    firstFile.CopyTo(_fileSystem.Path.Combine(downloadDirectory.FullName, downloadFileName));
                    firstFile.MoveTo(_fileSystem.Path.Combine(_downloadListDirectoryProvider.Succeeded.FullName, firstFile.Name), true);

                    var dataFileName = $"{mainFileNameWithoutExtension}.data.zip";
                    var dataFilePath = Path.Combine(downloadDirectory.FullName, dataFileName);
                    await _dataDownloader.DownloadData(download.EpisodeId, dataFilePath);
                }
                else
                {
                    firstFile.MoveTo(_fileSystem.Path.Combine(_downloadListDirectoryProvider.Failed.FullName, firstFile.Name), true);
                    workingDirectoryInfo.Delete(true);
                    downloadDirectory.Delete(true);
                }
            }
        }

        private void _MoveToFailed(IFileInfo firstFile)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var targetDirectory = _downloadListDirectoryProvider.Failed;
                    firstFile.MoveTo(_fileSystem.Path.Combine(targetDirectory.FullName, firstFile.Name), true);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error moving file {firstFile.Name} to failed directory: {ex.Message}");
                    Thread.Sleep(1000); // Wait for 1 second before retrying
                }
            }
        }

        private static Download? _GetDownload(IFileInfo? firstFile)
        {
            if (firstFile == null)
                return null;

            try
            {

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

                return download;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on file {firstFile.Name}: {ex.Message}");
                return null;
            }
        }

        private IFileInfo _GetMainFileName(IDirectoryInfo downloadDirectory)
        {
            var maxSize = 0;
            IFileInfo mainFile = downloadDirectory.GetFiles().First();
            foreach (var file in downloadDirectory.GetFiles())
            {
                if (file.Length > maxSize)
                {
                    maxSize = (int)file.Length;
                    mainFile = file;
                }
            }

            return mainFile;
        }
    }


}
