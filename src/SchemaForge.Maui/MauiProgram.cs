using Microsoft.Extensions.Logging;
using SchemaForge.Core.DdlGeneration;
using SchemaForge.Core.Services;
using SchemaForge.Core.State;
using SchemaForge.Shared.Interop;

namespace SchemaForge.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

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
        builder.Services.AddSingleton<SchemaForge.Shared.Services.ProjectContext>();
        builder.Services.AddSingleton<SchemaForge.Shared.Services.AutoSaveService>();

        // JS Interop Services (via BlazorWebView)
        builder.Services.AddSingleton<MonacoEditorInterop>();
        builder.Services.AddSingleton<KeyboardInterop>();
        builder.Services.AddSingleton<FileInterop>();
        builder.Services.AddSingleton<CanvasExportInterop>();
        builder.Services.AddSingleton<ISqlExecutionService, SqlJsInterop>();
        builder.Services.AddSingleton<IProjectStorageService, IndexedDbStorageService>();

        return builder.Build();
    }
}
