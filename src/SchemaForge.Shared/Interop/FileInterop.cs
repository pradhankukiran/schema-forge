using Microsoft.JSInterop;

namespace SchemaForge.Shared.Interop;

public class FileInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public FileInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/fileInterop.js").AsTask());
    }

    public async Task DownloadTextAsync(string filename, string content, string mimeType = "text/plain")
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("downloadText", filename, content, mimeType);
    }

    public async Task DownloadSqlAsync(string filename, string sql)
    {
        await DownloadTextAsync(filename, sql, "application/sql");
    }

    public async Task CopyToClipboardAsync(string text)
    {
        var module = await _moduleTask.Value;
        await module.InvokeAsync<bool>("copyToClipboard", text);
    }

    public async Task DownloadCsvAsync(string filename, string csvContent)
    {
        await DownloadTextAsync(filename, csvContent, "text/csv");
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
