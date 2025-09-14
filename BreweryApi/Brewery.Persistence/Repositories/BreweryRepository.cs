using Brewery.Core.Entities;
using Brewery.Persistence.Clients;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Brewery.Persistence.Repositories
{
    public class BreweryRepository : IBreweryRepository
    {
        private readonly IOpenBreweryApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BreweryRepository> _logger;
        private const string CacheKey = "breweries_cache";
        private const int CacheExpirationMinutes = 10;

        public BreweryRepository(
            IOpenBreweryApiClient apiClient, 
            IMemoryCache cache, 
            ILogger<BreweryRepository> logger)
        {
            _apiClient = apiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<BreweryEntity>> GetAllAsync()
        {
            if (_cache.TryGetValue(CacheKey, out IEnumerable<BreweryEntity>? cachedBreweries))
            {
                _logger.LogInformation("Retrieved breweries from cache");
                return cachedBreweries!;
            }

            return await RefreshAndGetCacheAsync();
        }

        public async Task<BreweryEntity?> GetByIdAsync(string id)
        {
            var breweries = await GetAllAsync();
            return breweries.FirstOrDefault(b => b.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public async Task RefreshCacheAsync()
        {
            await RefreshAndGetCacheAsync();
        }

        private async Task<IEnumerable<BreweryEntity>> RefreshAndGetCacheAsync()
        {
            try
            {
                var breweries = await _apiClient.GetBreweriesAsync();
                
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes)
                };

                _cache.Set(CacheKey, breweries, cacheOptions);
                _logger.LogInformation("Cached {Count} breweries for {Minutes} minutes", 
                    breweries.Count(), CacheExpirationMinutes);

                return breweries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing brewery cache");
                throw;
            }
        }
    }
}