using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace AktWeb.Functions.Functions;

public class KeepAlive
{
    static readonly object ImAlive = new { Message = "I'm awake" };

    [Function("keepAlive")]
    public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, CancellationToken ct)
    {
        var ok = req.CreateResponse(HttpStatusCode.OK);
        await ok.WriteAsJsonAsync(ImAlive, ct);
        return ok;
    }
}
