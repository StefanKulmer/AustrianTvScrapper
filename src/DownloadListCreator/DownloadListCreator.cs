using Downloader.Services;
using OrfDataProvider.Model;
using OrfDataProvider.Services;
using Subscription.Services;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace DownloadListCreator;


public interface IDownloadListCreator
{
    void Create();
}

public class DownloadListCreator : IDownloadListCreator
{
    private readonly ISubscriptionManager subscriptionManager;
    private readonly IDownloadsProvider downloadsProvider;
    private readonly IOrfDataProvider orfDataProvider;
    private readonly IDirectoryProvider downloaderDirectoryProvider;

    public DownloadListCreator(ISubscriptionManager subscriptionManager, IDownloadsProvider downloadsProvider, IOrfDataProvider orfDataProvider, Downloader.Services.IDirectoryProvider downloaderDirectoryProvider)
    {
        this.subscriptionManager = subscriptionManager;
        this.downloadsProvider = downloadsProvider;
        this.orfDataProvider = orfDataProvider;
        this.downloaderDirectoryProvider = downloaderDirectoryProvider;
    }

    public void Create()
    {
        var existingDownloads = downloadsProvider.GetDownloaded().ToList();
        var alreadyScheduled = downloadsProvider.GetScheduled().ToList();
        existingDownloads.AddRange(alreadyScheduled);
        int i = 0;
        var subscriptions = subscriptionManager.GetSubscriptions();
        foreach (var subscription in subscriptions)
        {
            var episodes = orfDataProvider.GetEpisodesOfProfileAsync(subscription.ProfileId).Result;
            foreach (var  episode in episodes)
            {
                if (existingDownloads.Any(ex => ex.EpisodeId == episode.Id && ex.ProfileId == episode.ProfileId))
                    continue;

                var episodeFileName = ReplaceInvalidFileNameChars(episode.Name);

                var directory = subscription.DownloadSubDirectory;
                if (directory != null)
                {
                    var dlParentDirectory = Path.GetDirectoryName(directory);
                    var dlFileName = Path.GetFileName(directory);

                    if (dlParentDirectory == null)
                    {
                        directory = ReplaceInvalidFileNameChars(dlFileName);
                    }
                    else
                    {
                        directory = Path.Combine(dlParentDirectory, ReplaceInvalidFileNameChars(dlFileName));
                    }
                }
                else
                {
                    directory = ReplaceInvalidFileNameChars(subscription.Name);
                }
                directory = directory.Replace("#year", episode.ReleaseDate.Year.ToString(), StringComparison.OrdinalIgnoreCase);

                var dl = new Downloader.Model.Download()
                {
                    EpisodeId = episode.Id,
                    ProfileId = episode.ProfileId,
                    CreationDate = DateTime.Now,
                    Directory = $"{directory}\\{episode.ReleaseDate:yyyy-MM-dd} {episodeFileName}",
                    Url = episode.Url
                };
                var serializeOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    WriteIndented = true
                };

                var fileName = $"{i++:00000000}_{episode.ProfileId}_{episode.Id}_{episodeFileName}.json";
                var filePath = Path.Combine(downloaderDirectoryProvider.Queue.FullName, fileName);

                // Create a file stream for writing
                using FileStream createStream = System.IO.File.Create(filePath);

                // Serialize the object to the file stream
                JsonSerializer.Serialize(createStream, dl, serializeOptions);

                // Ensure all data is written to the file
                createStream.Flush();
            }
            
        }
    }

    static string ReplaceInvalidFileNameChars(string filename, char replacementChar = '_')
    {
        // Get the array of invalid characters
        char[] invalidChars = Path.GetInvalidFileNameChars();

        // Create a regex pattern that matches any invalid character
        string invalidCharsPattern = $"[{Regex.Escape(new string(invalidChars))}]";

        // Replace each invalid character with the specified replacement character
        filename = Regex.Replace(filename, invalidCharsPattern, replacementChar.ToString());

        // Trim any leading or trailing replacement characters
        filename = filename.Trim(replacementChar);

        return filename;
    }
    // fetch abos
    // for each abo
    //   
}
