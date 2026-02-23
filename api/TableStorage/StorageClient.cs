using AktWeb.Functions.Model;
using Azure.Data.Tables;

namespace AktWeb.Functions.TableStorage;

public class TableStorageClient
{
    private readonly TableClient _aircraftDataTableClient;
    private readonly TableClient _fuelDataTableClient;

    public TableStorageClient(TableServiceClient tableService, AppConfiguration configuration)
    {
        _aircraftDataTableClient = tableService.GetTableClient(configuration.AircraftDataTableName);
        _fuelDataTableClient = tableService.GetTableClient(configuration.FuelDataTableName);
    }

    public async Task<AircraftRawData> GetAircraftData(string aircraft, CancellationToken ct)
    {
        return await _aircraftDataTableClient.GetEntityAsync<AircraftRawData>(
            partitionKey: aircraft,
            rowKey: "current",
            cancellationToken: ct
        );
    }

    public async Task<FuelData> GetFuelData(CancellationToken ct)
    {
        var tableData = await _fuelDataTableClient.GetEntityAsync<TableEntity>(
            partitionKey: "fuel",
            rowKey: "current",
            cancellationToken: ct
        );

        return new FuelData
        {
            FuelRemaining = Convert.ToInt32(tableData.Value["RemainingFuel"])
        };
    }
}
