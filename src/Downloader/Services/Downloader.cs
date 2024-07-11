using Downloader.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Downloader.Services
{
    public interface IDownloader
    {
        void Start();
    }

    public class Downloader : IDownloader
    {
        private readonly IFileSystem fileSystem;
        private readonly IDirectoryProvider directoryProvider;

        public Downloader(IFileSystem fileSystem, IDirectoryProvider directoryProvider)
        {
            this.fileSystem = fileSystem;
            this.directoryProvider = directoryProvider;
        }

        public void Start()
        {
            var queueDirectory = directoryProvider.Queue;
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
                    download = JsonSerializer.Deserialize<Model.Download>(openStream, serializeOptions);
                }
                    
                if (download == null)
                {
                    firstFile.MoveTo(directoryProvider.Failed.FullName);
                    continue;
                }

                var rootedDirectory = Path.Combine(directoryProvider.DownloadDirectory.FullName, download.Directory);
                var downloadDirectory = fileSystem.DirectoryInfo.New(rootedDirectory);
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
                    JsonSerializer.Serialize<Model.Download>(createStream, download, serializeOptions);
                }
                    
                if (p.ExitCode == 0)
                {
                    firstFile.CopyTo(fileSystem.Path.Combine(downloadDirectory.FullName, firstFile.Name));
                    firstFile.MoveTo(fileSystem.Path.Combine(directoryProvider.Succeeded.FullName, firstFile.Name));
                }
                else
                {
                    firstFile.MoveTo(fileSystem.Path.Combine(directoryProvider.Failed.FullName, firstFile.Name), true);
                    downloadDirectory.Delete(true);
                }
            }
        }
    }


}
