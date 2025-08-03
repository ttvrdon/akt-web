using AktWeb.Functions.BlobStorage;
using AktWeb.Functions.Caching;
using AktWeb.Functions.Model;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace AktWeb.Functions.Functions;

public class GetAircraftData
{
    private readonly AppConfiguration _configuration;
    private readonly ILogger<GetAircraftData> _logger;
    private readonly AircraftDataCache _dataCache;
    private readonly StorageClient _storageClient;
    private readonly GraphServiceClient _graphClient;

    public GetAircraftData(
        ILogger<GetAircraftData> logger,
        AppConfiguration configuration,
        AircraftDataCache cache,
        GraphServiceClient graphClient,
        StorageClient storageClient)
    {
        _configuration = configuration;
        _logger = logger;
        _dataCache = cache;
        _graphClient = graphClient;
        _storageClient = storageClient;
    }

    [Function(nameof(GetAircraftData))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request in GetAircraftData Function.");

        try
        {
            if (_dataCache.LastCacheReloaded + _configuration.CacheExpiry < DateTimeOffset.UtcNow)
            {
                _logger.LogInformation("Cache is stale. Triggering cache reload.");
                await ReloadCache();
            }

            var aircraftData = await _dataCache.GetCachedAircraftData(_storageClient.GetAircraftData);

            return new OkObjectResult(aircraftData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading data from Excel");
            throw;
        }
    }

    private async Task ReloadCache()
    {
        var lastUpdated = await _dataCache.GetCachedLastUpdated(_storageClient.GetLastUpdated);

        var drive = await _graphClient.Sites[_configuration.SharepointSiteId].Drive.GetAsync();
        var fileInfo = await _graphClient.Drives[drive!.Id].Root.ItemWithPath(_configuration.SharepointFilePath).GetAsync();

        var excelLastUpdated = fileInfo!.LastModifiedDateTime!.Value;

        if (excelLastUpdated > lastUpdated)
        {
            _logger.LogInformation("Excel file has been updated. Downloading new data...");

            var contentRequest = _graphClient.Drives[drive.Id].Items[fileInfo.Id].Content;
            var aircraftData = await DownloadFromExcelAsync(contentRequest);

            await _storageClient.SetAircraftData(excelLastUpdated, aircraftData);
            await _dataCache.RefreshCache(excelLastUpdated, aircraftData);
        }

        _dataCache.LastCacheReloaded = DateTimeOffset.UtcNow;
    }

    private static async Task<AircraftData> DownloadFromExcelAsync(Microsoft.Graph.Drives.Item.Items.Item.Content.ContentRequestBuilder contentRequest)
    {
        using var stream = await contentRequest.GetAsync();
        using var workbook = new XLWorkbook(stream);

        var table = workbook.Table("Table1");

        var totalDouble = table.Row(2).Cell(4).GetValue<double>();
        var fromReconstructionDouble = table.Row(2).Cell(5).GetValue<double>();
        var fromAnnualDouble = table.Row(2).Cell(6).GetValue<double>();
        var nextServiceInDouble = table.Row(2).Cell(7).GetValue<double>();

        var total = GetHoursAndMinutes(totalDouble);
        var fromReconstruction = GetHoursAndMinutes(fromReconstructionDouble);
        var fromAnnual = GetHoursAndMinutes(fromAnnualDouble);
        var nextServiceIn = GetHoursAndMinutes(nextServiceInDouble);

        return new AircraftData
        {
            Aircraft = table.Row(2).Cell(1).GetString(),
            TotalHours = total.Hours,
            TotalMinutes = total.Minutes,
            FromReconstructionHours = fromReconstruction.Hours,
            FromReconstructionMinutes = fromReconstruction.Minutes,
            FromAnnualHours = fromAnnual.Hours,
            FromAnnualMinutes = fromAnnual.Minutes,
            NextServiceInHours = nextServiceIn.Hours,
            NextServiceInMinutes = nextServiceIn.Minutes,
        };
    }

    private static (int Hours, int Minutes) GetHoursAndMinutes(double excelTimeDouble)
    {
        // Convert Excel time (fraction of a day) to TimeSpan
        var time = TimeSpan.FromDays(excelTimeDouble);

        // Round total minutes to nearest 5
        int totalMinutes = (int)Math.Round(time.TotalMinutes / 5.0) * 5;

        // Convert back to TimeSpan
        var roundedTime = TimeSpan.FromMinutes(totalMinutes);

        return ((int)roundedTime.TotalHours, roundedTime.Minutes);
    }
}