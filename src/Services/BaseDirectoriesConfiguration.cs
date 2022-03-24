namespace AustrianTvScrapper.Services
{
    public class BaseDirectoriesConfiguration
    {
        public string BasePath { get; set; } 
        public string DefaultEpisodeNameFormat { get; set; } = "#Date #Title";
        public string DefaultSubDirectory { get; set; } = "#Year\\#Name";
    }
}
