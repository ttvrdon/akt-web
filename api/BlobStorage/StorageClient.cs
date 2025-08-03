using AktWeb.Functions.Model;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace AktWeb.Functions.BlobStorage;

public class StorageClient
{
    private const string AircraftDataPropertyName = "data";
    private const string LastUpdatedPropertyName = "lastUpdated";

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
    }

    public async Task<AircraftData> GetAircraftData()
    {
        var data = await GetMetadata(AircraftDataPropertyName);

        return JsonSerializer.Deserialize<AircraftData>(data, _jsonSerializerOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize '{AircraftDataPropertyName}' metadata to AircraftData.");
    }

    public async Task<DateTimeOffset> GetLastUpdated()
    {
        var data = await GetMetadata(LastUpdatedPropertyName);
        return DateTimeOffset.Parse(data);
    }

    public async Task SetAircraftData(DateTimeOffset lastUpdated, AircraftData data)
    {
        var containerClient = _blobService.GetBlobContainerClient(_configuration.DataContainerName);
        var blobClient = containerClient.GetBlobClient(_configuration.DataBlobName);
        var metadata = new Dictionary<string, string>
        {
            { AircraftDataPropertyName, JsonSerializer.Serialize(data, _jsonSerializerOptions) },
            { LastUpdatedPropertyName, lastUpdated.ToString("o") }
        };
        await blobClient.SetMetadataAsync(metadata);
    }

    private async Task<string> GetMetadata(string key)
    {
        var containerClient = _blobService.GetBlobContainerClient(_configuration.DataContainerName);
        var blobClient = containerClient.GetBlobClient(_configuration.DataBlobName);

        var properties = await blobClient.GetPropertiesAsync();

        if (!properties.Value.Metadata.TryGetValue(key, out string? data) || string.IsNullOrEmpty(data))
        {
            throw new InvalidOperationException($"Blob metadata '{key}' is missing or empty.");
        }

        return data;
    }
}
