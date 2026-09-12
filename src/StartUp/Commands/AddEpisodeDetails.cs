using AustrianTvScrapper.Services;
using Downloader.Services;
using DownloadListCreator.Model;
using OrfDataProvider.Model;
using OrfDataProvider.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class AddEpisodeDetails : Command
    {
        private readonly IDataDownloader _dataDownloader;
        private readonly IOrfDataProvider _orfDataProvider;
        private readonly IFileSystem _fileSystem;

        public AddEpisodeDetails(
            IDataDownloader dataDownloader,
            IOrfDataProvider orfDataProvider,
            IFileSystem fileSystem 
            )
            : base("add-episode-details", "adds episode details in a directory")
        {
            _dataDownloader = dataDownloader ?? throw new ArgumentNullException(nameof(dataDownloader));
            _orfDataProvider = orfDataProvider ?? throw new ArgumentNullException(nameof(orfDataProvider));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

            var dirOption = new Option<string>("--directory", "-d") { Description = "directory" };
            var recurseOption = new Option<bool>("--recurse", "-r") { Description = "search in all subdirectories" };
            Add(dirOption);
            Add(recurseOption);

            this.SetAction(parseResult => _HandleCommand(
                parseResult.GetValue(dirOption),
                parseResult.GetValue(recurseOption)));
        }

        private async Task _HandleCommand(string directory, bool recurse)
        {
            var directoryInfo = _fileSystem.DirectoryInfo.New(directory);

            if (!directoryInfo.Exists)
            {
                Console.WriteLine($"directory {directory} doesn't exist.");
            }

            SearchOption searchOption = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var jsonFiles = directoryInfo.GetFiles("*.info.json", searchOption);

            Console.WriteLine($"{jsonFiles.Length} json files found in directory {directory}.");

            foreach (var jsonFile in jsonFiles)
            {
                await _ProcessDirectory(jsonFile).ConfigureAwait(false);
            }
        }

        private async Task _ProcessDirectory(IFileInfo ytdlInfoFileInfo)
        {
            var fileContent = await _fileSystem.File.ReadAllTextAsync(ytdlInfoFileInfo.FullName);
            var ytdlJsonDocument = JsonDocument.Parse(fileContent);
            var id = Convert.ToInt32(ytdlJsonDocument.RootElement.GetProperty("id").GetString());

            var episode = await _orfDataProvider.GetEpisodeDetail(id).ConfigureAwait(false);
            if (episode == null)
            {
                Console.WriteLine($"episode with {id} doesn't exist.");
                return;
            }

            var directoryInfo = ytdlInfoFileInfo.Directory;
            Console.WriteLine($"processing directory {directoryInfo.FullName} ...");
            var mainFileName = _GetMainFileName(directoryInfo);
            var mainFileNameWithoutExtension = _fileSystem.Path.GetFileNameWithoutExtension(mainFileName.Name);
            var downloadFileName = $"{mainFileNameWithoutExtension}.download.json";
            var downloadFilePath = Path.Combine(directoryInfo.FullName, downloadFileName);
            var dataFileName = $"{mainFileNameWithoutExtension}.data.zip";
            var dataFilePath = Path.Combine(directoryInfo.FullName, dataFileName);

            if (_fileSystem.File.Exists(dataFilePath))
            {
                _fileSystem.File.Delete(dataFilePath);
            }
            await _dataDownloader.DownloadData(id, dataFilePath);

            var regex = new System.Text.RegularExpressions.Regex(@"^(\d{1,10})_(\d{1,10})_(\d{1,10})_.*\.json$");
            var downloaderFiles = directoryInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly).Where(f => regex.IsMatch(f.Name)).ToList();
            if (downloaderFiles.Count == 0)
            {
                Console.WriteLine($"no downloader json file found in directory {directoryInfo.FullName}.");
                return;
            }

            if (downloaderFiles.Count == 1)
            {
                downloaderFiles[0].MoveTo(downloadFilePath);
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

