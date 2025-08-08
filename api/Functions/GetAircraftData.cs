using AktWeb.Functions.BlobStorage;
using AktWeb.Functions.Caching;
using AktWeb.Functions.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AktWeb.Functions.Functions;

public class GetAircraftData
{
    private readonly ILogger<GetAircraftData> _logger;
    private readonly AircraftDataCache _dataCache;
    private readonly StorageClient _storageClient;

    public GetAircraftData(
        ILogger<GetAircraftData> logger,
        AircraftDataCache cache,
        StorageClient storageClient)
    {
        _logger = logger;
        _dataCache = cache;
        _storageClient = storageClient;
    }

    [Function(nameof(GetAircraftData))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request in GetAircraftData Function.");

        try
        {
            var aircraftData = await _dataCache.GetCachedAircraftData(async () =>
            {
                var rawData = await _storageClient.GetAircraftData();
                return rawData.ToAircraftData();
            });

            return new OkObjectResult(aircraftData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data from storage");
            throw;
        }
    }
}