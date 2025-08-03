using Azure.Core.Diagnostics;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

namespace AktWeb.Functions.Extensions;
internal static class StartupExtensions
{
    public static void AddGraphServiceClient(this IServiceCollection services, AppConfiguration configuration)
    {
        var graphClient = new GraphServiceClient(GetCredential(configuration), ["https://graph.microsoft.com/.default"]);

        services.AddSingleton(graphClient);
    }

    public static void AddStorageClient(this IServiceCollection services, AppConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration.StorageAccountConnectionsString);
        });


        AzureEventSourceListener.CreateConsoleLogger();
    }

    private static ClientSecretCredential GetCredential(AppConfiguration configuration)
    {
        return new ClientSecretCredential(
            configuration.AzureTenantId,
            configuration.AzureClientId,
            configuration.AzureClientSecret,
            new ClientSecretCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            });
    }
}
