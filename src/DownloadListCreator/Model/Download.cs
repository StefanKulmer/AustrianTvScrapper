namespace DownloadListCreator.Model;

public class Download
{
    public string? Directory { get; set; }
    public string? Url { get; set; }
    public int ProfileId { get; set; }
    public int EpisodeId { get; set; }
    public DateTime CreationDate { get; set; }
    public string? State { get; set; }
    public DateTime? DownloadStartedDate { get; set; }
    public DateTime? DownloadFinishedDate { get; set; }
}
