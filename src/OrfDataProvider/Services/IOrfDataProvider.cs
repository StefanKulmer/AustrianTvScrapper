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
}
