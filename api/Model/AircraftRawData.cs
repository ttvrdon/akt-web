using AktWeb.Functions.Model.Converters;
using System.Text.Json.Serialization;

namespace AktWeb.Functions.Model;

public class AircraftRawData
{
    public required string Aircraft { get; set; }

    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public required double Total { get; set; }

    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public required double FromReconstruction { get; set; }

    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public required double FromAnnual { get; set; }

    [JsonConverter(typeof(FlexibleDoubleConverter))]
    public required double NextServiceIn { get; set; }
}
