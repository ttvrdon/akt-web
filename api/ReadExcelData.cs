using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AktWeb.Functions;

public class ReadExcelData
{
    private readonly ILogger<ReadExcelData> _logger;

    public ReadExcelData(ILogger<ReadExcelData> logger)
    {
        _logger = logger;
    }

    [Function("ReadExcelData")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        _logger.LogInformation("Processing request in ReadExcelData Function.");

        var excelSharingLink = Environment.GetEnvironmentVariable("ExcelSharingLink");

        var response = new
        {
            Message = "Hello from ReadExcelData Function!",
            SharingLink = excelSharingLink,
            Timestamp = DateTime.UtcNow
        };

        await Task.CompletedTask; // Simulate async work

        return new OkObjectResult(response);
    }
}