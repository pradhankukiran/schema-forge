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

// DDL Generation
builder.Services.AddSingleton<IDdlGeneratorService, SqliteDdlGenerator>();
builder.Services.AddSingleton<DdlGeneratorFactory>(sp =>
    new DdlGeneratorFactory(sp.GetServices<IDdlGeneratorService>()));

// JS Interop Services
builder.Services.AddSingleton<MonacoEditorInterop>();
builder.Services.AddSingleton<ISqlExecutionService, SqlJsInterop>();

await builder.Build().RunAsync();
