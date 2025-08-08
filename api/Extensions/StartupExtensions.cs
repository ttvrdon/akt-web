using Azure.Core.Diagnostics;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace AktWeb.Functions.Extensions;
internal static class StartupExtensions
{
    public static void AddStorageClient(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration.StorageAccountConnectionsString);
        });

        AzureEventSourceListener.CreateConsoleLogger();
    }
}
