using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SchemaForge.Core.DdlGeneration;
using SchemaForge.Core.Services;
using SchemaForge.Core.State;
using SchemaForge.Shared.Interop;
using SchemaForge.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// State
builder.Services.AddSingleton<SchemaState>();
builder.Services.AddSingleton<QueryState>();

// DDL Generation (all 4 dialects)
builder.Services.AddSingleton<IDdlGeneratorService, SqliteDdlGenerator>();
builder.Services.AddSingleton<IDdlGeneratorService, PostgresDdlGenerator>();
builder.Services.AddSingleton<IDdlGeneratorService, MySqlDdlGenerator>();
builder.Services.AddSingleton<IDdlGeneratorService, SqlServerDdlGenerator>();
builder.Services.AddSingleton<DdlGeneratorFactory>(sp =>
    new DdlGeneratorFactory(sp.GetServices<IDdlGeneratorService>()));

// App Services
builder.Services.AddSingleton<SchemaForge.Shared.Services.AutoSaveService>();

// JS Interop Services
builder.Services.AddSingleton<MonacoEditorInterop>();
builder.Services.AddSingleton<KeyboardInterop>();
builder.Services.AddSingleton<FileInterop>();
builder.Services.AddSingleton<CanvasExportInterop>();
builder.Services.AddSingleton<ISqlExecutionService, SqlJsInterop>();
builder.Services.AddSingleton<IProjectStorageService, IndexedDbStorageService>();

await builder.Build().RunAsync();
