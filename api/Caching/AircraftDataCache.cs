using AktWeb.Functions.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AktWeb.Functions.Caching;

public class AircraftDataCache
{
    private const string CacheKey = "aircraft";
    private readonly IMemoryCache _memoryCache;
    private readonly AppConfiguration _configuration;

    public AircraftDataCache(IMemoryCache memoryCache, IOptions<AppConfiguration> configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration.Value;
    }

    public async Task<AircraftData> GetCachedAircraftData(Func<Task<AircraftData>> dataGetter)
    {
        // Try to get from cache
        if (!_memoryCache.TryGetValue(CacheKey, out AircraftData? data))
        {
            data = await dataGetter();

            // Set cache with expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_configuration.CacheExpiry);

            _memoryCache.Set(CacheKey, data, cacheEntryOptions);
        }

        return data!;
    }
}