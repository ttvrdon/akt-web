namespace AktWeb.Functions.Model;

public class AircraftData
{
    public required string Aircraft { get; set; }

    public required int TotalHours { get; set; }
    public required int TotalMinutes { get; set; }

    public required int FromReconstructionHours { get; set; }
    public required int FromReconstructionMinutes { get; set; }

    public required int FromAnnualHours { get; set; }
    public required int FromAnnualMinutes { get; set; }

    public required int NextServiceInHours { get; set; }
    public required int NextServiceInMinutes { get; set; }
}
