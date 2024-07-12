namespace DownloadListCreator.Model
{
    public class DownloadListOptions
    {
        public string? RootDirectory { get; set; }

        public static DownloadListOptions Default = new DownloadListOptions
        {
            RootDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AustrianTvScrapper")
        };
    }
}
