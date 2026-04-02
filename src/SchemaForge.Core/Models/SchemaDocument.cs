namespace SchemaForge.Core.Models;

public record SchemaDocument
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "Untitled Schema";
    public string Description { get; init; } = "";
    public SqlDialect TargetDialect { get; init; } = SqlDialect.SQLite;
    public List<TableDefinition> Tables { get; init; } = [];
    public List<RelationshipDefinition> Relationships { get; init; } = [];
    public CanvasSettings Canvas { get; init; } = new();
    public ErdNotation Notation { get; init; } = ErdNotation.CrowsFoot;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;
}
