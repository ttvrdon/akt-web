using AktWeb.Functions;
using AktWeb.Functions.Caching;
using AktWeb.Functions.Extensions;
using AktWeb.Functions.TableStorage;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

var appConfig = builder.Configuration.Get<AppConfiguration>() ?? throw new InvalidOperationException($"Config is required");
builder.Services.AddSingleton(appConfig);

builder.Services.AddTableStorageClient(appConfig);

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<DataCache>();
builder.Services.AddSingleton<TableStorageClient>();

builder.Build().Run();
