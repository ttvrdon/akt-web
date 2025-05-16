namespace AktWeb.Functions;
public class AppConfiguration
{
    public required string ExcelSharingLink { get; set; }
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(15);
}
