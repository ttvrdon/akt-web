using AktWeb.Functions.Caching;
using AktWeb.Functions.Model;
using AktWeb.Functions.TableStorage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AktWeb.Functions.Functions;

public class GetAircraftData
{
    private readonly ILogger<GetAircraftData> _logger;
    private readonly DataCache _dataCache;
    private readonly TableStorageClient _storageClient;

    public GetAircraftData(
        ILogger<GetAircraftData> logger,
        DataCache cache,
        TableStorageClient storageClient)
    {
        _logger = logger;
        _dataCache = cache;
        _storageClient = storageClient;
    }

    [Function(nameof(GetAircraftData))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "aircraft/{aircraftId}")]
        HttpRequestData req,
        string aircraftId,
        CancellationToken ct)
    {
        _logger.LogInformation("Processing request in GetAircraftData Function.");

        try
        {
            var aircraftData = await _dataCache.GetCachedAircraftData(aircraftId, async () =>
            {
                var rawData = await _storageClient.GetAircraftData(aircraftId, ct);
                return rawData.ToAircraftData();
            }, ct);

            var ok = req.CreateResponse(HttpStatusCode.OK);
            await ok.WriteAsJsonAsync(aircraftData, ct);
            return ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data from storage");
            throw;
        }
    }
}