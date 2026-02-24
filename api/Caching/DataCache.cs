using AktWeb.Functions.Extensions;
using AktWeb.Functions.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AktWeb.Functions.Caching;

public class DataCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly AppConfiguration _configuration;
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public DataCache(IMemoryCache memoryCache, IOptions<AppConfiguration> configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration.Value;
    }

    public Task<AircraftData> GetCachedAircraftData(string aircraftId, Func<Task<AircraftData>> dataGetter, CancellationToken ct)
    {
        return GetCachedData(dataGetter, aircraftId, ct);
    }

    public Task<FuelData> GetCachedFuelData(Func<Task<FuelData>> dataGetter, CancellationToken ct)
    {
        return GetCachedData(dataGetter, nameof(GetCachedFuelData), ct);
    }

    private async Task<T> GetCachedData<T>(Func<Task<T>> dataGetter, string cacheKey, CancellationToken ct)
    {
        using (await _mutex.LockAsync(ct))
        {
            if (!_memoryCache.TryGetValue(cacheKey, out T? data))
            {
                data = await dataGetter().ConfigureAwait(false);
                var options = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_configuration.CacheExpiry);
                _memoryCache.Set(cacheKey, data, options);
                return data!;
            }

            return data!;
        }
    }
}
