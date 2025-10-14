using AktWeb.Functions.Model;
using AktWeb.Functions.Model.Converters;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace AktWeb.Functions.BlobStorage;

public class StorageClient
{
    private const string DataPropertyName = "data";

    private readonly BlobServiceClient _blobService;
    private readonly AppConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public StorageClient(BlobServiceClient blobService, AppConfiguration configuration)
    {
        _blobService = blobService;
        _configuration = configuration;

        _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        _jsonSerializerOptions.Converters.Add(new StringToIntConverter());
    }

    public async Task<AircraftRawData> GetAircraftData()
    {
        var data = await GetMetadata(_configuration.AircraftDataBlobName, DataPropertyName);

        return JsonSerializer.Deserialize<AircraftRawData>(data, _jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize '{DataPropertyName}' metadata to AircraftRawData.");
    }

    public async Task<FuelData> GetFuelData()
    {
        var data = await GetMetadata(_configuration.FuelDataBlobName, DataPropertyName);

        return JsonSerializer.Deserialize<FuelData>(data, _jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize '{DataPropertyName}' metadata to FuelData.");
    }


    private async Task<string> GetMetadata(string blobName, string key)
    {
        var containerClient = _blobService.GetBlobContainerClient(_configuration.DataContainerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var properties = await blobClient.GetPropertiesAsync();

        if (!properties.Value.Metadata.TryGetValue(key, out string? data) || string.IsNullOrEmpty(data))
        {
            throw new InvalidOperationException($"Blob metadata '{key}' is missing or empty.");
        }

        return data;
    }
}
