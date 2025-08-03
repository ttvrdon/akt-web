namespace AktWeb.Functions;
public class AppConfiguration
{
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(15);

    public required string StorageAccountConnectionsString { get; set; }
    public required string DataContainerName { get; set; }
    public required string DataBlobName { get; set; }

    public required string AzureTenantId { get; set; }
    public required string AzureClientId { get; set; }
    public required string AzureClientSecret { get; set; }

    public required string SharepointSiteId { get; set; }
    public required string SharepointFilePath { get; set; }
}
