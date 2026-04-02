namespace SchemaForge.Core.Models;

public record TableDefinition
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "new_table";
    public string Schema { get; init; } = "public";
    public string Comment { get; init; } = "";
    public string AccentColor { get; init; } = "#1B5E9E";
    public List<ColumnDefinition> Columns { get; init; } = [];
    public List<IndexDefinition> Indexes { get; init; } = [];
    public List<CheckConstraint> CheckConstraints { get; init; } = [];
    public CanvasPosition Position { get; init; } = new();
    public bool IsCollapsed { get; init; }
}
