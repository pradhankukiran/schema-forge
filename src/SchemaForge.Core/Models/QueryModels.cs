namespace SchemaForge.Core.Models;

public record QueryTab
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Title { get; init; } = "Query 1";
    public string SqlContent { get; init; } = "";
    public QueryResult? LastResult { get; init; }
}

public record QueryHistoryEntry
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Sql { get; init; } = "";
    public DateTime ExecutedAt { get; init; } = DateTime.UtcNow;
    public double ExecutionTimeMs { get; init; }
    public int RowCount { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record QueryResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public string[] Columns { get; init; } = [];
    public object?[][] Rows { get; init; } = [];
    public int RowsAffected { get; init; }
    public double ExecutionTimeMs { get; init; }
}
