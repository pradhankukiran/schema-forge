namespace SchemaForge.Core.Models;

public record IndexDefinition
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "";
    public List<string> ColumnIds { get; init; } = [];
    public bool IsUnique { get; init; }
    public string? WhereClause { get; init; }
}

public record CheckConstraint
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "";
    public string Expression { get; init; } = "";
}
