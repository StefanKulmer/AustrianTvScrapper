using OrfDataProvider.Model;
using OrfDataProvider.Services;
using System.IO.Compression;
using System.Text.Json;

namespace Downloader.Services
{
    public interface IDataDownloader
    {
        Task DownloadData(int episodeId, string targetPath);
    }

    internal class DataDownloader : IDataDownloader
    {
        private readonly IOrfDataProvider _orfDataProvider;

        public DataDownloader(IOrfDataProvider orfDataProvider)
        {
            _orfDataProvider = orfDataProvider ?? throw new ArgumentNullException(nameof(orfDataProvider));
        }

        public async Task DownloadData(int episodeId, string targetPath)
        {
            var directory = Path.GetDirectoryName(targetPath);
            var targetDirectory = Path.Combine(directory, "orf-data");

            if (Directory.Exists(targetDirectory))
            {
                Directory.Delete(targetDirectory, true);
            }
            Directory.CreateDirectory(targetDirectory);

            await _Download(episodeId, targetDirectory);

            ZipFile.CreateFromDirectory(targetDirectory, targetPath, CompressionLevel.SmallestSize, false);

            try
            {
                Directory.Delete(targetDirectory, true);
            }
            catch
            {
                // ignore
            }
        }

        private async Task _Download(int episodeId, string targetDirectory)
        {
            var episode = _orfDataProvider.GetEpisodeDetail(episodeId).Result;
            if (episode == null)
            {
                Console.WriteLine($"episode with {episodeId} doesn't exist.");
                return;
            }

            _WriteJsonToFile(episode.JsonData, Path.Combine(targetDirectory, "episode.json"));
            await _ExtractImages(episode.Images, targetDirectory);
            await _ExtractImages(episode.ProfileImages, targetDirectory);
            await _ExtractSubtitles(episode.Subtitles, targetDirectory);

            foreach (var segment in episode.Segments)
            {
                await _ExtractImages(segment.Images, targetDirectory);
                await _ExtractSubtitles(segment.Subtitles, targetDirectory);
            }
        }

        private async Task _ExtractSubtitles(IReadOnlyCollection<SubtitleDetail> subtitles, string targetDirectory)
        {
            foreach (var subtitle in subtitles)
            {
                var subtitleFileName = Path.GetFileName(new Uri(subtitle.Url).Segments[^1]);
                await File.WriteAllBytesAsync(Path.Combine(targetDirectory, subtitleFileName), subtitle.Data);
            }
        }
        
        private async Task _ExtractImages(IReadOnlyCollection<ImageDetail> imageDetails, string targetDirectory)
        {
            foreach (var imageDetail in imageDetails)
            {
                var imageFileName = Path.GetFileName(new Uri(imageDetail.Url).Segments[^1]);
                await File.WriteAllBytesAsync(Path.Combine(targetDirectory, imageFileName), imageDetail.Content);
            }
        }

        private static void _WriteJsonToFile(string jsonString, string filePath)
        {
            // Parse the JSON string
            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                // Serialize back to JSON string with indentation
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Enables indentation
                };

                string formattedJson = JsonSerializer.Serialize(document.RootElement, options);

                // Write the formatted JSON to the file
                File.WriteAllText(filePath, formattedJson);
            }
        }
    }
}
