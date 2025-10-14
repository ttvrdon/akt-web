using AktWeb.Functions.BlobStorage;
using AktWeb.Functions.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AktWeb.Functions.Functions;

public class GetFuelData
{
    private readonly ILogger<GetFuelData> _logger;
    private readonly DataCache _dataCache;
    private readonly StorageClient _storageClient;

    public GetFuelData(
        ILogger<GetFuelData> logger,
        DataCache cache,
        StorageClient storageClient)
    {
        _logger = logger;
        _dataCache = cache;
        _storageClient = storageClient;
    }

    [Function(nameof(GetFuelData))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request in GetFuelData Function.");

        try
        {
            var fuelData = await _dataCache.GetCachedFuelData(async () =>
            {
                return await _storageClient.GetFuelData();
            });

            return new OkObjectResult(fuelData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data from storage");
            throw;
        }
    }
}