//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using System.Net;

//namespace AktWeb.Functions.Functions;

//public class KeepAlive
//{
//    [Function("keepAlive")]
//    public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
//    {
//        var response = req.CreateResponse(HttpStatusCode.OK);
//        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
//        response.WriteString("I'm awake!");

//        return response;
//    }
//}
