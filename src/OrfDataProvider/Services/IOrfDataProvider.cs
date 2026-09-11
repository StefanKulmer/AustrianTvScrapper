using OrfDataProvider.Model;
using System;
using System.Buffers;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OrfDataProvider.Services;

public interface IOrfDataProvider
{
    Task<IReadOnlyCollection<Episode>> GetEpisodesOfProfileAsync(int profileId);
    Task<IReadOnlyCollection<Genre>> GetGenres();
    Task<IReadOnlyCollection<Profile>> GetProfiles();
    Task<Profile> GetProfile(int profileId);

    Task<EpisodeDetail> GetEpisodeDetail(int episodeId);
}

public class OrfDataProvider : IOrfDataProvider
{
    const string UserName = "orf_on_v43";
    const string Password = "jFRsbNPFiPSxuwnLbYDfCLUN5YNZ28mt";

    public async Task<IReadOnlyCollection<Episode>> GetEpisodesOfProfileAsync(int profileId)
    {
        var url = $"https://api-tvthek.orf.at/api/v4.3/profile/{profileId}/episodes";

        var client = GetHttpClient();

        var episodesRaw = await client.GetStringAsync(url + "?page=1&limit=500");
        var episodesJsonDocument = JsonDocument.Parse(episodesRaw);
        var result = new List<Episode>();
        var eps = episodesJsonDocument.RootElement.GetProperty("_embedded").GetProperty("items").EnumerateArray();
        foreach (var ep in eps)
        {
            var releaseDate = ep.GetProperty("release_date").GetDateTime();
            var epName = ep.GetProperty("title").GetString();
            var episodeId = ep.GetProperty("id").GetInt32();
            var episode = new Episode
            {
                Id = episodeId,
                ProfileId = profileId,
                ReleaseDate = releaseDate,
                Name = epName,
                Url = ep.GetProperty("share_body").GetString(),
            };

            result.Add(episode);
        }

        return result;
    }

    public async Task<IReadOnlyCollection<Profile>> GetProfiles()
    {
        var url = "https://api-tvthek.orf.at/api/v4.3/profiles?page=1&limit=2000";
        var response = await _LoadFromWebOrCacheAsync(url, "profiles_snapshot.json");
        var jsonDocument = JsonDocument.Parse(response);
        var result = new List<Profile>();
        var items = jsonDocument.RootElement.GetProperty("_embedded").GetProperty("items").EnumerateArray();
        foreach (var item in items)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
            var profile = JsonSerializer.Deserialize<Model.Profile>(item, serializeOptions);
            result.Add(profile);
        }

        return result;
    }

    private async Task<string> _LoadFromWebOrCacheAsync(string url, string snapshotFileName)
    {
        var fileInfo = new FileInfo(snapshotFileName);
        if (fileInfo.Exists && fileInfo.CreationTime > (DateTime.Now - TimeSpan.FromHours(1)))
        {
            var allLines = await File.ReadAllLinesAsync(fileInfo.FullName);
            return string.Join(Environment.NewLine, allLines);
        }

        var client = GetHttpClient();

        var response = await client.GetStringAsync(url);
        
        // create cache
        if (fileInfo.Exists)
            fileInfo.Delete();
        File.WriteAllText(fileInfo.FullName, response);

        return response;
    }

    private HttpClient GetHttpClient()
    {
        var client = new HttpClient();
        client.BaseAddress = new Uri("https://api-tvthek.orf.at/api/v4.3/profiles");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var authenticationBytes = Encoding.ASCII.GetBytes($"{UserName}:{Password}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(authenticationBytes));

        return client;
    }

    public async Task<IReadOnlyCollection<Genre>> GetGenres()
    {
        var url = "https://api-tvthek.orf.at/api/v4.3/genres?page=1&limit=100";
        var response = await _LoadFromWebOrCacheAsync(url, "genres_snapshot.json");
        var jsonDocument = JsonDocument.Parse(response);
        var result = new List<Genre>();
        var items = jsonDocument.RootElement.GetProperty("_embedded").GetProperty("items").EnumerateArray();
        foreach (var item in items)
        {
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
            var profile = JsonSerializer.Deserialize<Model.Genre>(item, serializeOptions);
            result.Add(profile);
        }

        return result;
    }

    public async Task<Profile?> GetProfile(int profileId)
    {
        var url = $"https://api-tvthek.orf.at/api/v4.3/profile/{profileId}";
        try
        {
            var response = await _LoadFromWebOrCacheAsync(url, $"profile_{profileId}_snapshot.json");
            var jsonDocument = JsonDocument.Parse(response);
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
            var profile = JsonSerializer.Deserialize<Model.Profile>(jsonDocument, serializeOptions);

            return profile;
        }
        catch
        {
            return null;
        }
    }

    public async Task<EpisodeDetail> GetEpisodeDetail(int episodeId)
    {
        var url = $"https://api-tvthek.orf.at/api/v4.3/episode/{episodeId}";

        var client = GetHttpClient();

        string episodeRaw;
        try
        {
            episodeRaw = await client.GetStringAsync(url);
        }
        catch(HttpRequestException ex)
        {
            Console.WriteLine($"request exception for url '{url}' with code {ex.StatusCode} {ex.Message}");
            return null;
        }

        var episodeJsonDocument = JsonDocument.Parse(episodeRaw);
        
        var episodeImages = await _GetImages(client, episodeJsonDocument.RootElement);
        var subtitles = await _GetSubtitles(client, episodeJsonDocument.RootElement);

        var profileElement = episodeJsonDocument.RootElement.GetProperty("_embedded")
            .GetProperty("profile");
        var profileImages = await _GetImages(client, profileElement);

        var segments = episodeJsonDocument.RootElement.GetProperty("_embedded")
            .GetProperty("segments")
            .EnumerateArray();
        var segmentDetails = new List<SegmentDetail>();
        foreach (var segment in segments)
        {
            var segmentImages = await _GetImages(client, segment);
            var segmentSubtitles = await _GetSubtitles(client, segment);
            var segmentDetail = new SegmentDetail()
            {
                JsonData = segment.ToString(),
                Images = segmentImages,
                Subtitles = subtitles,
            };

            segmentDetails.Add(segmentDetail);
        }

        return new EpisodeDetail()
        {
            JsonData = episodeRaw,
            Images = episodeImages,
            ProfileImages = profileImages,
            Segments = segmentDetails,
            Subtitles = subtitles,
        };
    }

    private async Task<IReadOnlyCollection<ImageDetail>> _GetImages(HttpClient client, JsonElement itemElement)
    {
        if (!itemElement.TryGetProperty("_embedded", out var embeddedElement) || embeddedElement.ValueKind == JsonValueKind.Null)
            return Array.Empty<ImageDetail>();

        if (!embeddedElement.TryGetProperty("image", out var imageElement) || imageElement.ValueKind == JsonValueKind.Null)
            return Array.Empty<ImageDetail>();

        if (!imageElement.TryGetProperty("public_urls", out var publicUrlsElement) || publicUrlsElement.ValueKind == JsonValueKind.Null)
            return Array.Empty<ImageDetail>();

        if (publicUrlsElement.ValueKind != JsonValueKind.Object)
            return Array.Empty<ImageDetail>();

        var imageDetails = new List<ImageDetail>();
        foreach (var childElement in publicUrlsElement.EnumerateObject())
        {
            var imageUrl = childElement.Value.GetProperty("url").GetString();
            if (string.IsNullOrEmpty(imageUrl))
                continue;

            var imageData = await client.GetByteArrayAsync(imageUrl);
            var imageDetail = new ImageDetail()
            {
                Name = childElement.Name,
                Url = imageUrl,
                Content = imageData,
            };

            imageDetails.Add(imageDetail);
        }

        return imageDetails;
    }

    private async Task<IReadOnlyCollection<SubtitleDetail>> _GetSubtitles(HttpClient client, JsonElement parentElement)
    {
        if (!parentElement.TryGetProperty("_embedded", out var embeddedElement))
            return Array.Empty<SubtitleDetail>();

        if (!embeddedElement.TryGetProperty("subtitle", out var subtitleElement))
            return Array.Empty<SubtitleDetail>();

        if (subtitleElement.ValueKind == JsonValueKind.Null)
            return Array.Empty<SubtitleDetail>();

        var subtitleDetails = new List<SubtitleDetail>();
        foreach (var type in SubtitleType.All)
        {
            if (!subtitleElement.TryGetProperty($"{type.Prefix}_url", out var subtitleUrl))
            {
                continue;
            }

            var subtitleUrlString = subtitleUrl.GetString();
            if (string.IsNullOrEmpty(subtitleUrlString))
            {
                continue;
            }

            var subtitleData = await client.GetByteArrayAsync(subtitleUrlString);
            var subtitleDetail =
                new SubtitleDetail
                {
                    Type = type,
                    Url = subtitleUrlString,
                    Data = subtitleData,
                };
            subtitleDetails.Add(subtitleDetail);
        }

        return subtitleDetails;
    }
}
