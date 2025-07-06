using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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