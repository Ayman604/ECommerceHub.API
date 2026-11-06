using ECommerce.Domain.Contracts;
using StackExchange.Redis;

namespace ECommerce.Presistence.Repositories
{
    public class CacheRepository : ICacheRepositiory
    {
        private readonly IDatabase _database;

        public CacheRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<string?> GetAsync(string cacheKey)
        {
            var cacheValue = await _database.StringGetAsync(cacheKey);
            return cacheValue.IsNullOrEmpty ? null : cacheValue.ToString();
        }

        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive)
        {
            await _database.StringSetAsync(cacheKey, cacheValue, timeToLive);
        }
    }
}
