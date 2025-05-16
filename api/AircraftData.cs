namespace AktWeb.Functions;
public class AircraftData
{
    public required string Aircraft { get; set; }

    public required int TotalHours { get; set; }
    public required int TotalMinutes { get; set; }

    public required int FromGOHours { get; set; }
    public required int FromGOMinutes { get; set; }

    public required int NextServiceInHours { get; set; }
    public required int NextServiceInMinutes { get; set; }
}
