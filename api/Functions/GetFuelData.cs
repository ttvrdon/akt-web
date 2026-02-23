using AktWeb.Functions.Caching;
using AktWeb.Functions.TableStorage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AktWeb.Functions.Functions;

public class GetFuelData
{
    private readonly ILogger<GetFuelData> _logger;
    private readonly DataCache _dataCache;
    private readonly TableStorageClient _storageClient;

    public GetFuelData(
        ILogger<GetFuelData> logger,
        DataCache cache,
        TableStorageClient storageClient)
    {
        _logger = logger;
        _dataCache = cache;
        _storageClient = storageClient;
    }

    [Function(nameof(GetFuelData))]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, CancellationToken ct)
    {
        _logger.LogInformation("Processing request in GetFuelData Function.");

        try
        {
            var fuelData = await _dataCache.GetCachedFuelData(async () =>
            {
                return await _storageClient.GetFuelData(ct);
            }, ct);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(fuelData, ct);
            return ok;
        }
        catch (Exception ex)
        {
            // Build an error payload but STILL return 200
            var error = ErrorEnvelope.FromException(ex);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(error, ct); // Response is 200 with exception detail in body
            return ok;

            //_logger.LogError(ex, "Error getting data from storage");
            //throw;
        }
    }
}