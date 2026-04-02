namespace SchemaForge.Core.Models;

public record RelationshipDefinition
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "";
    public string SourceTableId { get; init; } = "";
    public string SourceColumnId { get; init; } = "";
    public string TargetTableId { get; init; } = "";
    public string TargetColumnId { get; init; } = "";
    public Cardinality Cardinality { get; init; } = Cardinality.ManyToOne;
    public ReferentialAction OnDelete { get; init; } = ReferentialAction.NoAction;
    public ReferentialAction OnUpdate { get; init; } = ReferentialAction.NoAction;
}
