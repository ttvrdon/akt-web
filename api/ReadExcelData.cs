using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AktWeb.Api;

public static class ReadExcelData
{
    [FunctionName("ReadExcelData")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "readExcelData")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Processing request in ReadExcelData Function.");

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
