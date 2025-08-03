using AktWeb.Functions.Model;

namespace AktWeb.Functions.Caching;

public class AircraftDataCache
{
    private DateTimeOffset? _cachedLastUpdated;
    private AircraftData? _cachedData;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DateTimeOffset LastCacheReloaded { get; set; } = DateTimeOffset.MinValue;

    public async Task<DateTimeOffset> GetCachedLastUpdated(Func<Task<DateTimeOffset>> getter)
    {
        if (_cachedLastUpdated == null)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_cachedLastUpdated == null)
                {
                    _cachedLastUpdated = await getter();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return _cachedLastUpdated!.Value;
    }

    public async Task<AircraftData> GetCachedAircraftData(Func<Task<AircraftData>> getter)
    {
        if (_cachedData == null)
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_cachedData == null)
                {
                    _cachedData = await getter();
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        return _cachedData!;
    }

    public async Task RefreshCache(DateTimeOffset lastUpdated, AircraftData data)
    {
        await _semaphore.WaitAsync();
        try
        {
            _cachedLastUpdated = lastUpdated;
            _cachedData = data;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
