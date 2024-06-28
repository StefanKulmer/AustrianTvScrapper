namespace Subscription.Model
{
    public class Subscription
    {
        public int Id { get; set; }
        public int ProfileId { get;set; }
        public string? Name { get; set; }
        public string? DownloadSubDirectory { get; set; }
        public DateTime Created { get; set; }
    }
}
