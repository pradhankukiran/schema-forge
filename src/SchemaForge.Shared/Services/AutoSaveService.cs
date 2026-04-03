using System.Threading;
using Microsoft.JSInterop;
using SchemaForge.Core.Models;
using SchemaForge.Core.Services;
using SchemaForge.Core.State;
using SchemaForge.Shared.Serialization;

namespace SchemaForge.Shared.Services;

public class AutoSaveService : IDisposable
{
    private readonly SchemaState _schemaState;
    private readonly QueryState _queryState;
    private readonly IProjectStorageService _storage;
    private readonly Lazy<Task<IJSObjectReference>> _storageModuleTask;
    private readonly SemaphoreSlim _saveGate = new(1, 1);
    private Timer? _timer;
    private Timer? _pendingProjectTimer;
    private string _currentProjectId = "";
    private bool _isDirty;
    private long _changeVersion;
    private bool _lifecyclePersistenceInitialized;

    public event Action<string>? OnSaveStatusChanged;
    public string SaveStatus { get; private set; } = "Not saved";

    public AutoSaveService(
        SchemaState schemaState,
        QueryState queryState,
        IProjectStorageService storage,
        IJSRuntime jsRuntime)
    {
        _schemaState = schemaState;
        _queryState = queryState;
        _storage = storage;
        _storageModuleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/SchemaForge.Shared/js/storageInterop.js").AsTask());

        _schemaState.OnDirty += MarkDirty;
        _queryState.OnDirty += MarkDirty;
    }

    public void Start(string projectId, bool hasStoredSnapshot = false, int intervalSeconds = 30)
    {
        _currentProjectId = projectId;
        _isDirty = false;
        Interlocked.Exchange(ref _changeVersion, 0);
        UpdateSaveStatus(hasStoredSnapshot ? "Saved" : "Not saved");
        _timer?.Dispose();
        _timer = new Timer(_ => _ = SaveIfDirtyAsync(), null,
            TimeSpan.FromSeconds(intervalSeconds),
            TimeSpan.FromSeconds(intervalSeconds));

        _ = EnsureLifecyclePersistenceAsync();
        if (hasStoredSnapshot)
        {
            _ = SyncPendingProjectAsync();
        }
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
        _pendingProjectTimer?.Dispose();
        _pendingProjectTimer = null;
    }

    public async Task SaveNowAsync()
    {
        await SaveProjectAsync();
    }

    private void MarkDirty()
    {
        Interlocked.Increment(ref _changeVersion);
        _isDirty = true;
        UpdateSaveStatus("Unsaved changes");
        SchedulePendingProjectSync();
    }

    private void SchedulePendingProjectSync()
    {
        if (string.IsNullOrEmpty(_currentProjectId)) return;

        _pendingProjectTimer?.Dispose();
        _pendingProjectTimer = new Timer(_ => _ = SyncPendingProjectAsync(), null,
            TimeSpan.FromMilliseconds(750),
            Timeout.InfiniteTimeSpan);
    }

    private async Task SaveIfDirtyAsync()
    {
        if (!_isDirty || string.IsNullOrEmpty(_currentProjectId)) return;
        await SaveProjectAsync();
    }

    private Project BuildProject() =>
        new()
        {
            Id = _currentProjectId,
            Name = _schemaState.Document.Name,
            Schema = _schemaState.Document,
            QueryTabs = _queryState.Tabs,
            ActiveQueryTabIndex = _queryState.ActiveTabIndex,
            QueryHistory = _queryState.History,
            ModifiedAt = DateTime.UtcNow
        };

    private async Task EnsureLifecyclePersistenceAsync()
    {
        try
        {
            if (_lifecyclePersistenceInitialized)
                return;

            var module = await _storageModuleTask.Value;
            await module.InvokeVoidAsync("initializeLifecyclePersistence");
            _lifecyclePersistenceInitialized = true;
        }
        catch
        {
            // Lifecycle persistence is best-effort.
        }
    }

    private async Task SyncPendingProjectAsync()
    {
        if (string.IsNullOrEmpty(_currentProjectId))
            return;

        try
        {
            await EnsureLifecyclePersistenceAsync();
            var module = await _storageModuleTask.Value;
            var json = ProjectJsonSerializer.SerializeToElement(BuildProject());
            await module.InvokeVoidAsync("setPendingProject", json);
        }
        catch
        {
            // Keep autosave functional even if the unload-sync path fails.
        }
    }

    private async Task SaveProjectAsync()
    {
        if (string.IsNullOrEmpty(_currentProjectId))
            return;

        await _saveGate.WaitAsync();
        try
        {
            var versionAtSaveStart = Interlocked.Read(ref _changeVersion);
            var project = BuildProject();

            await _storage.SaveProjectAsync(project);

            if (Interlocked.Read(ref _changeVersion) == versionAtSaveStart)
            {
                _isDirty = false;
                UpdateSaveStatus("Saved");
            }
            else
            {
                _isDirty = true;
                UpdateSaveStatus("Unsaved changes");
            }

            _ = SyncPendingProjectAsync();
        }
        catch
        {
            UpdateSaveStatus("Save failed");
        }
        finally
        {
            _saveGate.Release();
        }
    }

    private void UpdateSaveStatus(string status)
    {
        if (SaveStatus == status)
            return;

        SaveStatus = status;
        OnSaveStatusChanged?.Invoke(SaveStatus);
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _pendingProjectTimer?.Dispose();
        _schemaState.OnDirty -= MarkDirty;
        _queryState.OnDirty -= MarkDirty;
        _saveGate.Dispose();
    }
}
