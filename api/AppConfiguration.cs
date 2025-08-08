namespace AktWeb.Functions;
public class AppConfiguration
{
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(10);

    public required string StorageAccountConnectionsString { get; set; }
    public required string DataContainerName { get; set; }
    public required string DataBlobName { get; set; }
}
