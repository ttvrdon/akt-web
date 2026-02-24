namespace AktWeb.Functions;

public class AppConfiguration
{
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(5);

    public required string StorageAccountConnectionsString { get; set; }
    public required string AircraftDataTableName { get; set; }
    public required string FuelDataTableName { get; set; }
}

