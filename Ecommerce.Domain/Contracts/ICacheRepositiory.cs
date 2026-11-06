namespace ECommerce.Domain.Contracts
{
    public interface ICacheRepositiory
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive);
    }
}
