namespace OrfDataProvider.Model
{
    public class SegmentDetail
    {
        public required string JsonData { get; init; }
        public required IReadOnlyCollection<ImageDetail> Images { get; init; }
        public required IReadOnlyCollection<SubtitleDetail> Subtitles { get; init; }
    }
}
