namespace SchemaForge.Core.Models;

public record Project
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "Untitled Project";
    public SchemaDocument Schema { get; init; } = new();
    public List<QueryTab> QueryTabs { get; init; } = [];
    public int ActiveQueryTabIndex { get; init; }
    public List<QueryHistoryEntry> QueryHistory { get; init; } = [];
    public ProjectSettings Settings { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;
    public int Version { get; init; } = 1;
}

public record ProjectSummary
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public int TableCount { get; init; }
    public DateTime ModifiedAt { get; init; }
}

public record ProjectSettings
{
    public SqlDialect DefaultDialect { get; init; } = SqlDialect.SQLite;
    public ErdNotation DefaultNotation { get; init; } = ErdNotation.CrowsFoot;
    public bool AutoSaveEnabled { get; init; } = true;
    public int AutoSaveIntervalSeconds { get; init; } = 30;
}
