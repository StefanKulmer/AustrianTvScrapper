namespace OrfDataProvider.Model
{
    public class EpisodeDetail
    {
        public required string JsonData { get; init; }
        public required IReadOnlyCollection<ImageDetail> Images { get; init; }
        public required IReadOnlyCollection<ImageDetail> ProfileImages { get; init; }
        public required IReadOnlyCollection<SegmentDetail> Segments { get; init; }
        public required IReadOnlyCollection<SubtitleDetail> Subtitles{ get; init; }
    }
}
