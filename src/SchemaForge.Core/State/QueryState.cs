using SchemaForge.Core.Models;

namespace SchemaForge.Core.State;

public class QueryState
{
    private const int MaxHistoryEntries = 500;

    public event Action? OnChange;

    public List<QueryTab> Tabs { get; private set; } = [new()];
    public int ActiveTabIndex { get; private set; }
    public List<QueryHistoryEntry> History { get; private set; } = [];

    public QueryTab ActiveTab => Tabs[ActiveTabIndex];

    public void AddTab()
    {
        if (Tabs.Count >= 20) return;
        var tab = new QueryTab { Title = $"Query {Tabs.Count + 1}" };
        Tabs.Add(tab);
        ActiveTabIndex = Tabs.Count - 1;
        OnChange?.Invoke();
    }

    public void CloseTab(int index)
    {
        if (Tabs.Count <= 1) return;
        Tabs.RemoveAt(index);
        if (ActiveTabIndex >= Tabs.Count)
            ActiveTabIndex = Tabs.Count - 1;
        OnChange?.Invoke();
    }

    public void SetActiveTab(int index)
    {
        if (index < 0 || index >= Tabs.Count) return;
        ActiveTabIndex = index;
        OnChange?.Invoke();
    }

    public void UpdateTabContent(int index, string sql)
    {
        if (index < 0 || index >= Tabs.Count) return;
        Tabs[index] = Tabs[index] with { SqlContent = sql };
    }

    public void UpdateTabResult(int index, QueryResult result)
    {
        if (index < 0 || index >= Tabs.Count) return;
        Tabs[index] = Tabs[index] with { LastResult = result };
        OnChange?.Invoke();
    }

    public void RenameTab(int index, string title)
    {
        if (index < 0 || index >= Tabs.Count) return;
        Tabs[index] = Tabs[index] with { Title = title };
        OnChange?.Invoke();
    }

    public void AddHistoryEntry(QueryHistoryEntry entry)
    {
        History.Insert(0, entry);
        if (History.Count > MaxHistoryEntries)
            History.RemoveAt(History.Count - 1);
        OnChange?.Invoke();
    }

    public void LoadState(List<QueryTab> tabs, int activeIndex, List<QueryHistoryEntry> history)
    {
        Tabs = tabs.Count > 0 ? tabs : [new()];
        ActiveTabIndex = Math.Clamp(activeIndex, 0, Tabs.Count - 1);
        History = history;
        OnChange?.Invoke();
    }
}
