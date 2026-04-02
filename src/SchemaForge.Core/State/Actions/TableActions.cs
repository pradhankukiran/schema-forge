using SchemaForge.Core.Models;

namespace SchemaForge.Core.State.Actions;

public record AddTableAction(TableDefinition Table) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with { Tables = [.. doc.Tables, Table], ModifiedAt = DateTime.UtcNow };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new RemoveTableAction(Table.Id);
}

public record RemoveTableAction(string TableId) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Where(t => t.Id != TableId).ToList(),
            Relationships = doc.Relationships
                .Where(r => r.SourceTableId != TableId && r.TargetTableId != TableId)
                .ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var table = doc.Tables.First(t => t.Id == TableId);
        var rels = doc.Relationships
            .Where(r => r.SourceTableId == TableId || r.TargetTableId == TableId)
            .ToList();
        return new CompoundAction([
            new AddTableAction(table),
            .. rels.Select(r => (ISchemaAction)new AddRelationshipAction(r))
        ]);
    }
}

public record UpdateTableAction(string TableId, string Name, string Comment, string AccentColor) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Select(t => t.Id == TableId
                ? t with { Name = Name, Comment = Comment, AccentColor = AccentColor }
                : t).ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var table = doc.Tables.First(t => t.Id == TableId);
        return new UpdateTableAction(TableId, table.Name, table.Comment, table.AccentColor);
    }
}

public record MoveTableAction(string TableId, double X, double Y) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Select(t => t.Id == TableId
                ? t with { Position = t.Position with { X = X, Y = Y } }
                : t).ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var table = doc.Tables.First(t => t.Id == TableId);
        return new MoveTableAction(TableId, table.Position.X, table.Position.Y);
    }
}
