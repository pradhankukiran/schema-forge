using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SchemaForge.Core.DdlGeneration;
using SchemaForge.Core.Services;
using SchemaForge.Core.State;
using SchemaForge.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// State
builder.Services.AddSingleton<SchemaState>();
builder.Services.AddSingleton<QueryState>();

// Services
builder.Services.AddSingleton<IDdlGeneratorService, SqliteDdlGenerator>();
builder.Services.AddSingleton<DdlGeneratorFactory>(sp =>
    new DdlGeneratorFactory(sp.GetServices<IDdlGeneratorService>()));

await builder.Build().RunAsync();
