using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using SchemaForge.Core.Models;
using SchemaForge.Core.Services;

namespace SchemaForge.Shared.Interop;

public class IndexedDbStorageService : IProjectStorageService, IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public IndexedDbStorageService(IJSRuntime jsRuntime)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/storageInterop.js").AsTask());
    }

    public async Task<IReadOnlyList<ProjectSummary>> ListProjectsAsync()
    {
        var module = await _moduleTask.Value;
        var json = await module.InvokeAsync<JsonElement>("listProjects");
        var summaries = new List<ProjectSummary>();

        foreach (var item in json.EnumerateArray())
        {
            summaries.Add(new ProjectSummary
            {
                Id = item.GetProperty("id").GetString() ?? "",
                Name = item.GetProperty("name").GetString() ?? "",
                TableCount = item.GetProperty("tableCount").GetInt32(),
                ModifiedAt = item.TryGetProperty("modifiedAt", out var dt)
                    ? DateTime.Parse(dt.GetString() ?? DateTime.UtcNow.ToString("o"))
                    : DateTime.UtcNow
            });
        }

        return summaries;
    }

    public async Task<Project> LoadProjectAsync(string projectId)
    {
        var module = await _moduleTask.Value;
        var json = await module.InvokeAsync<JsonElement>("loadProject", projectId);

        if (json.ValueKind == JsonValueKind.Null)
            throw new InvalidOperationException($"Project {projectId} not found");

        return JsonSerializer.Deserialize<Project>(json.GetRawText(), JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize project");
    }

    public async Task SaveProjectAsync(Project project)
    {
        var module = await _moduleTask.Value;
        var json = JsonSerializer.SerializeToElement(project, JsonOptions);
        await module.InvokeAsync<bool>("saveProject", json);
    }

    public async Task DeleteProjectAsync(string projectId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeAsync<bool>("deleteProject", projectId);
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
