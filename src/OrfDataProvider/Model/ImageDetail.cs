namespace OrfDataProvider.Model
{
    public class ImageDetail
    {
        public required string Name { get; init; }
        public required string Url { get; init; }
        public required byte[] Content { get; init; }
    }
}
