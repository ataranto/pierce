namespace Pierce.Net
{
    public interface ICache
    {
        CacheEntry this[object key] { get; set; }
    }
}
