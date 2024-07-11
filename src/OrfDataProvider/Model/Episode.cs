namespace OrfDataProvider.Model
{
    public class Episode
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? Name { get; set; }
        public string Url { get; set; }
    }
}
