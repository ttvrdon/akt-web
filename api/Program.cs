using AktWeb.Functions;
using AktWeb.Functions.Caching;
using AktWeb.Functions.Extensions;
using AktWeb.Functions.TableStorage;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

var appConfig = builder.Configuration.Get<AppConfiguration>() ?? throw new InvalidOperationException($"Config is required");
builder.Services.AddSingleton(appConfig);

builder.Services.AddTableStorageClient(appConfig);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<DataCache>();
builder.Services.AddSingleton<TableStorageClient>();

builder.Services.Configure<WorkerOptions>(options =>
{
    options.Serializer = new JsonObjectSerializer(
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
});

builder.Build().Run();
