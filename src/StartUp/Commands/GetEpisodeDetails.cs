using AustrianTvScrapper.Services;
using OrfDataProvider.Model;
using OrfDataProvider.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class GetEpisodeDetails : Command
    {
        private readonly IOrfDataProvider _orfDataProvider;

        public GetEpisodeDetails(IOrfDataProvider orfDataProvider)
            : base("get-episode-details", "ignores a series")
        {
            var idOption = new Option<int>("--id", "-id") { Description = "id of episode" };
            var dirOption = new Option<string>("--directory", "-d") { Description = "directory" };
            Add(idOption);
            Add(dirOption);

            _orfDataProvider = orfDataProvider;

            this.SetAction(parseResult =>
            {
                _HandleCommand(parseResult.GetValue(idOption), parseResult.GetValue(dirOption));
                return Task.CompletedTask;
            });
        }

        private void _HandleCommand(int id, string directory)
        {
            var episode = _orfDataProvider.GetEpisodeDetail(id).Result;
            if (episode == null)
            {
                Console.WriteLine($"episode with {id} doesn't exist.");
                return;
            }



            var targetDirectory = Path.Combine(AppContext.BaseDirectory, "EpisodeDetails");
            if (Directory.Exists(targetDirectory))
            {
                Directory.Delete(targetDirectory);
            }
            Directory.CreateDirectory(targetDirectory);


            _WriteJsonToFile(episode.JsonData, Path.Combine(targetDirectory, "episode_detail_formatted.json"));
            //if (episode.ImageData != null && episode.ImageUrl != null)
            //{
            //    var imageFileName = Path.GetFileName(new Uri(episode.ImageUrl).Segments[^1]);
            //    File.WriteAllBytes(Path.Combine(targetDirectory, imageFileName), episode.ImageData);
            //}
            //_ExtractSubtitles(episode.Subtitles, targetDirectory);


            //int index = 0;
            //foreach (var segment in episode.Segments)
            //{
            //    Console.WriteLine($"Segment: {segment.ImageUrl}");
            //    if (segment.ImageData != null && segment.ImageUrl != null)
            //    {
            //        var imageFileName = Path.GetFileName(new Uri(segment.ImageUrl).Segments[^1]);
            //        File.WriteAllBytes(Path.Combine(targetDirectory, imageFileName), segment.ImageData);
            //    }
            //    _ExtractSubtitles(episode.Subtitles, targetDirectory);
            //    index++;
            //}
        }

        private void _ExtractSubtitles(IReadOnlyCollection<SubtitleDetail> subtitles, string targetDirectory)
        {
            foreach (var subtitle in subtitles)
            {
                var subtitleFileName = Path.GetFileName(new Uri(subtitle.Url).Segments[^1]);
                File.WriteAllBytes(Path.Combine(targetDirectory, subtitleFileName), subtitle.Data);
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

