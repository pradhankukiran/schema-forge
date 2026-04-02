using Microsoft.JSInterop;

namespace SchemaForge.Shared.Interop;

public class MonacoEditorInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private DotNetObjectReference<MonacoEditorInterop>? _dotNetRef;

    public event Func<string, string, Task>? OnExecuteRequested;

    public MonacoEditorInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/monacoInterop.js").AsTask());
    }

    public async Task InitializeAsync()
    {
        var module = await _moduleTask.Value;
        _dotNetRef = DotNetObjectReference.Create(this);
        await module.InvokeAsync<bool>("initialize", _dotNetRef);
    }

    public async Task CreateEditorAsync(string elementId, string initialValue = "", string language = "sql")
    {
        var module = await _moduleTask.Value;
        await module.InvokeAsync<bool>("createEditor", elementId, initialValue, language);
    }

    public async Task<string> GetValueAsync(string elementId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getValue", elementId);
    }

    public async Task SetValueAsync(string elementId, string value)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setValue", elementId, value);
    }

    public async Task<string> GetSelectedTextAsync(string elementId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getSelectedText", elementId);
    }

    public async Task SetCompletionItemsAsync(string[] tableNames, Dictionary<string, string[]> tableColumns)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("setCompletionItems", tableNames, tableColumns);
    }

    public async Task FocusAsync(string elementId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("focus", elementId);
    }

    public async Task DisposeEditorAsync(string elementId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("dispose", elementId);
    }

    [JSInvokable]
    public async Task OnExecuteRequested_Internal(string editorId, string sql)
    {
        if (OnExecuteRequested != null)
            await OnExecuteRequested.Invoke(editorId, sql);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("disposeAll");
            await module.DisposeAsync();
        }
        _dotNetRef?.Dispose();
    }
}
