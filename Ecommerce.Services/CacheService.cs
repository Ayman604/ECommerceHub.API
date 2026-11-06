using ECommerce.Domain.Contracts;
using ECommerce.Services.Abstraction;
using System.Text.Json;

namespace ECommerce.Services
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepositiory _cacheRepositiory;

        public CacheService(ICacheRepositiory cacheRepositiory)
        {
            _cacheRepositiory = cacheRepositiory;
        }
        public async Task<string?> GetAsync(string cacheKey)
        {
            return await _cacheRepositiory.GetAsync(cacheKey);
        }

        public async Task SetAsync(string cacheKey, object cacheValue, TimeSpan timeToLive)
        {
            var value = JsonSerializer.Serialize(
                cacheValue,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                );
            await _cacheRepositiory.SetAsync(cacheKey, value, timeToLive);
        }
    }
}
