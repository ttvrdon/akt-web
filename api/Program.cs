using AktWeb.Functions;
using AktWeb.Functions.BlobStorage;
using AktWeb.Functions.Caching;
using AktWeb.Functions.Extensions;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var appConfig = builder.Configuration.Get<AppConfiguration>() ?? throw new InvalidOperationException($"Config is required");
builder.Services.AddSingleton(appConfig);

builder.Services.AddStorageClient(appConfig);

builder.Services.AddSingleton<DataCache>();
builder.Services.AddSingleton<StorageClient>();

builder.Build().Run();
