using SchemaForge.Core.Models;

namespace SchemaForge.Core.State.Actions;

public record AddRelationshipAction(RelationshipDefinition Relationship) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Relationships = [.. doc.Relationships, Relationship],
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new RemoveRelationshipAction(Relationship.Id);
}

public record RemoveRelationshipAction(string RelationshipId) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Relationships = doc.Relationships.Where(r => r.Id != RelationshipId).ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var rel = doc.Relationships.First(r => r.Id == RelationshipId);
        return new AddRelationshipAction(rel);
    }
}

public record UpdateRelationshipAction(RelationshipDefinition Relationship) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Relationships = doc.Relationships
                .Select(r => r.Id == Relationship.Id ? Relationship : r)
                .ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var old = doc.Relationships.First(r => r.Id == Relationship.Id);
        return new UpdateRelationshipAction(old);
    }
}
