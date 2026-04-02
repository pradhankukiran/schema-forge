using System.Text.Json;
using Microsoft.JSInterop;
using SchemaForge.Core.Models;
using SchemaForge.Core.Services;

namespace SchemaForge.Shared.Interop;

public class SqlJsInterop : ISqlExecutionService, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public bool IsInitialized { get; private set; }

    public SqlJsInterop(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/sqlJsInterop.js").AsTask());
    }

    public async Task InitializeAsync()
    {
        var module = await _moduleTask.Value;
        await module.InvokeAsync<bool>("initialize");
        IsInitialized = true;
    }

    public async Task<QueryResult> ExecuteAsync(string sql)
    {
        if (!IsInitialized)
            await InitializeAsync();

        var module = await _moduleTask.Value;
        var json = await module.InvokeAsync<JsonElement>("execute", sql);
        return ParseResult(json);
    }

    public async Task<List<QueryResult>> ExecuteBatchAsync(string sql)
    {
        if (!IsInitialized)
            await InitializeAsync();

        var module = await _moduleTask.Value;
        var json = await module.InvokeAsync<JsonElement>("executeBatch", sql);
        var results = new List<QueryResult>();

        foreach (var item in json.EnumerateArray())
        {
            results.Add(ParseResult(item));
        }

        return results;
    }

    public async Task ResetAsync()
    {
        var module = await _moduleTask.Value;
        await module.InvokeAsync<bool>("reset");
    }

    private static QueryResult ParseResult(JsonElement json)
    {
        var success = json.GetProperty("success").GetBoolean();
        var executionTimeMs = json.GetProperty("executionTimeMs").GetDouble();
        var rowsAffected = json.GetProperty("rowsAffected").GetInt32();

        string? errorMessage = null;
        if (json.TryGetProperty("errorMessage", out var errProp) && errProp.ValueKind != JsonValueKind.Null)
            errorMessage = errProp.GetString();

        var columns = Array.Empty<string>();
        if (json.TryGetProperty("columns", out var colsProp))
            columns = colsProp.EnumerateArray().Select(c => c.GetString() ?? "").ToArray();

        var rows = Array.Empty<object?[]>();
        if (json.TryGetProperty("rows", out var rowsProp))
        {
            rows = rowsProp.EnumerateArray().Select(row =>
                row.EnumerateArray().Select(cell => cell.ValueKind switch
                {
                    JsonValueKind.Null => null,
                    JsonValueKind.Number => (object?)cell.GetDouble(),
                    JsonValueKind.True => (object?)true,
                    JsonValueKind.False => (object?)false,
                    _ => (object?)cell.GetString()
                }).ToArray()
            ).ToArray();
        }

        return new QueryResult
        {
            Success = success,
            ErrorMessage = errorMessage,
            Columns = columns,
            Rows = rows,
            RowsAffected = rowsAffected,
            ExecutionTimeMs = executionTimeMs
        };
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
