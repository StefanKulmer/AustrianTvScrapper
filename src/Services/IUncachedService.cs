namespace AustrianTvScrapper.Services
{
    public interface IUncachedService<T> where T:class
    {
        T Instance { get; }
    }
}
