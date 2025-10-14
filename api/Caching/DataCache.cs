using AktWeb.Functions.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AktWeb.Functions.Caching;

public class DataCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly AppConfiguration _configuration;

    public DataCache(IMemoryCache memoryCache, IOptions<AppConfiguration> configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration.Value;
    }

    public Task<AircraftData> GetCachedAircraftData(Func<Task<AircraftData>> dataGetter)
    {
        return GetCachedData(dataGetter, nameof(GetCachedAircraftData));
    }

    public Task<FuelData> GetCachedFuelData(Func<Task<FuelData>> dataGetter)
    {
        return GetCachedData(dataGetter, nameof(GetCachedFuelData));
    }

    //AircraftDataCacheKey
    private async Task<T> GetCachedData<T>(Func<Task<T>> dataGetter, string cacheKey)
    {
        // Try to get from cache
        if (!_memoryCache.TryGetValue(cacheKey, out T? data))
        {
            data = await dataGetter();

            // Set cache with expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_configuration.CacheExpiry);

            _memoryCache.Set(cacheKey, data, cacheEntryOptions);
        }

        return data!;
    }
}