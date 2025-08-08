namespace AktWeb.Functions.Model;

public static class Mappers
{
    public static AircraftData ToAircraftData(this AircraftRawData rawData)
    {
        if (rawData == null)
        {
            throw new ArgumentNullException(nameof(rawData));
        }

        var total = GetHoursAndMinutes(rawData.Total);
        var fromReconstruction = GetHoursAndMinutes(rawData.FromReconstruction);
        var fromAnnual = GetHoursAndMinutes(rawData.FromAnnual);
        var nextServiceIn = GetHoursAndMinutes(rawData.NextServiceIn);

        return new AircraftData
        {
            Aircraft = rawData.Aircraft,
            TotalHours = total.Hours,
            TotalMinutes = total.Minutes,
            FromReconstructionHours = fromReconstruction.Hours,
            FromReconstructionMinutes = fromReconstruction.Minutes,
            FromAnnualHours = fromAnnual.Hours,
            FromAnnualMinutes = fromAnnual.Minutes,
            NextServiceInHours = nextServiceIn.Hours,
            NextServiceInMinutes = nextServiceIn.Minutes
        };
    }

    private static (int Hours, int Minutes) GetHoursAndMinutes(double excelTimeDouble)
    {
        // Convert Excel time (fraction of a day) to TimeSpan
        var time = TimeSpan.FromDays(excelTimeDouble);

        // Round total minutes to nearest 5
        int totalMinutes = (int)Math.Round(time.TotalMinutes / 5.0) * 5;

        // Convert back to TimeSpan
        var roundedTime = TimeSpan.FromMinutes(totalMinutes);

        return ((int)roundedTime.TotalHours, roundedTime.Minutes);
    }
}
