using Downloader.Model;
using System.IO.Abstractions;
using System.Text.Json;

namespace Downloader.Services
{
    public interface IDownloadsProvider
    {
        IEnumerable<Model.Download> GetDownloaded();
        IEnumerable<Model.Download> GetScheduled();
    }
    public class DownloadsProvider : IDownloadsProvider
    {
        private readonly IDirectoryProvider directoryProvider;

        public DownloadsProvider(IDirectoryProvider directoryProvider)
        {
            this.directoryProvider = directoryProvider;
        }

        public IEnumerable<Download> GetDownloaded()
        {
            return _GetFromDirectory(directoryProvider.Succeeded);
        }

        public IEnumerable<Download> GetScheduled()
        {
            return _GetFromDirectory(directoryProvider.Queue);
        }

        private IEnumerable<Download> _GetFromDirectory(IDirectoryInfo directory)
        {
            if (!directory.Exists)
                yield break;

            var files = directory.GetFiles("*.json");
            if (files.Length == 0)
                yield break;

            foreach (var file in files)
            {
                Download? download;
                using (var stream = file.OpenRead())
                {
                    var serializeOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                        WriteIndented = true
                    };
                    download = JsonSerializer.Deserialize<Model.Download>(stream, serializeOptions);
                }

                if (download != null)
                    yield return download;
            }
        }
    }
}
