using SchemaForge.Core.Models;
using SchemaForge.Core.Services;
using SchemaForge.Core.State;

namespace SchemaForge.Shared.Services;

public class AutoSaveService : IDisposable
{
    private readonly SchemaState _schemaState;
    private readonly QueryState _queryState;
    private readonly IProjectStorageService _storage;
    private Timer? _timer;
    private string _currentProjectId = "";
    private bool _isDirty;

    public event Action<string>? OnSaveStatusChanged;
    public string SaveStatus { get; private set; } = "Saved";

    public AutoSaveService(SchemaState schemaState, QueryState queryState, IProjectStorageService storage)
    {
        _schemaState = schemaState;
        _queryState = queryState;
        _storage = storage;

        _schemaState.OnChange += MarkDirty;
        _queryState.OnChange += MarkDirty;
    }

    public void Start(string projectId, int intervalSeconds = 30)
    {
        _currentProjectId = projectId;
        _timer?.Dispose();
        _timer = new Timer(async _ => await SaveIfDirty(), null,
            TimeSpan.FromSeconds(intervalSeconds),
            TimeSpan.FromSeconds(intervalSeconds));
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public async Task SaveNowAsync()
    {
        await SaveProject();
    }

    private void MarkDirty()
    {
        _isDirty = true;
        SaveStatus = "Unsaved changes";
        OnSaveStatusChanged?.Invoke(SaveStatus);
    }

    private async Task SaveIfDirty()
    {
        if (!_isDirty || string.IsNullOrEmpty(_currentProjectId)) return;
        await SaveProject();
    }

    private async Task SaveProject()
    {
        try
        {
            var project = new Project
            {
                Id = _currentProjectId,
                Name = _schemaState.Document.Name,
                Schema = _schemaState.Document,
                QueryTabs = _queryState.Tabs,
                ActiveQueryTabIndex = _queryState.ActiveTabIndex,
                QueryHistory = _queryState.History,
                ModifiedAt = DateTime.UtcNow
            };

            await _storage.SaveProjectAsync(project);
            _isDirty = false;
            SaveStatus = "Saved";
            OnSaveStatusChanged?.Invoke(SaveStatus);
        }
        catch
        {
            SaveStatus = "Save failed";
            OnSaveStatusChanged?.Invoke(SaveStatus);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _schemaState.OnChange -= MarkDirty;
        _queryState.OnChange -= MarkDirty;
    }
}
