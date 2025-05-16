using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AktWeb.Functions.Functions;

public class GetAircraftData
{
    private readonly AppConfiguration _configuration;
    private readonly ILogger<GetAircraftData> _logger;
    private readonly AircraftDataCache _cache;

    public GetAircraftData(
        ILogger<GetAircraftData> logger,
        IOptions<AppConfiguration> configuration,
        AircraftDataCache cache)
    {
        _configuration = configuration.Value;
        _logger = logger;
        _cache = cache;
    }

    [Function(nameof(GetAircraftData))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request in GetAircraftData Function.");

        try
        {
            var aircraftData = await _cache.GetCachedAircraftData(() => DownloadFromExcelAsync(_configuration.ExcelSharingLink));

            return new OkObjectResult(aircraftData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading data from Excel");
            throw;
        }
    }


    private static async Task<AircraftData> DownloadFromExcelAsync(string sharingLink)
    {
        using var client = new HttpClient();

        // Convert OneDrive sharing link to direct download link
        var fileBytes = await client.GetByteArrayAsync(sharingLink);

        using var stream = new MemoryStream(fileBytes);
        using var workbook = new XLWorkbook(stream);

        var table = workbook.Table("Table2");

        var totalStr = table.Row(2).Cell(2).GetString();
        var fromGOStr = table.Row(2).Cell(3).GetString();
        var nextServiceInStr = table.Row(2).Cell(4).GetString();

        var total = totalStr.Split(':').Select(int.Parse).ToArray();
        var fromGO = fromGOStr.Split(':').Select(int.Parse).ToArray();
        var nextServiceIn = nextServiceInStr.Split(':').Select(int.Parse).ToArray();

        return new AircraftData
        {
            Aircraft = table.Row(2).Cell(1).GetString(),
            TotalHours = total[0],
            TotalMinutes = total[1],
            FromGOHours = fromGO[0],
            FromGOMinutes = fromGO[1],
            NextServiceInHours = nextServiceIn[0],
            NextServiceInMinutes = nextServiceIn[1],
        };
    }
}