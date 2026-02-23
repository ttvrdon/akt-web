using Azure.Core.Diagnostics;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace AktWeb.Functions.Extensions;

internal static class StartupExtensions
{
    public static void AddTableStorageClient(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddTableServiceClient(configuration.StorageAccountConnectionsString);
        });

        AzureEventSourceListener.CreateConsoleLogger();
    }
}
