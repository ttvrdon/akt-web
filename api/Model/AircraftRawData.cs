using System.Runtime.Serialization;

namespace AktWeb.Functions.Model;

[DataContract]
public class AircraftRawData : TableEntityBase
{
    [DataMember(Name = "total")]
    public required double Total { get; set; }

    [DataMember(Name = "fromReconstruction")]
    public required double FromReconstruction { get; set; }

    [DataMember(Name = "fromAnnual")]
    public required double FromAnnual { get; set; }

    [DataMember(Name = "nextServiceIn")]
    public required double NextServiceIn { get; set; }
}
