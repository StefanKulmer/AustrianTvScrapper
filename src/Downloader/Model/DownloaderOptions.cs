namespace Downloader.Model
{
    public class DownloaderOptions
    {
        public string DownloadDirectory { get; set; }
        public string YtDlpPath { get; set; }

        public static DownloaderOptions Default = new DownloaderOptions
        {
            DownloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AustrianTvScrapper"),
            YtDlpPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) , "Downloads", "yt-dlp.exe")
        };
    }
}
