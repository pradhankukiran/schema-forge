using Microsoft.JSInterop;

namespace SchemaForge.Shared.Interop;

public class CanvasExportInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public CanvasExportInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/canvasExportInterop.js").AsTask());
    }

    public async Task<string?> ExportSvgAsync(string svgSelector = ".sf-canvas")
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string?>("exportSvg", svgSelector);
    }

    public async Task<string?> ExportPngAsync(string svgSelector = ".sf-canvas", int scale = 2)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string?>("exportPng", svgSelector, scale);
    }

    public async Task DownloadSvgAsync(string filename = "schema.svg")
    {
        var svg = await ExportSvgAsync();
        if (svg == null) return;
        var module = await _moduleTask.Value;
        var dataUrl = $"data:image/svg+xml;charset=utf-8,{Uri.EscapeDataString(svg)}";
        await module.InvokeVoidAsync("downloadDataUrl", dataUrl, filename);
    }

    public async Task DownloadPngAsync(string filename = "schema.png", int scale = 2)
    {
        var png = await ExportPngAsync(scale: scale);
        if (png == null) return;
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("downloadDataUrl", png, filename);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
