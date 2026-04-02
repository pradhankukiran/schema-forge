using SchemaForge.Core.Models;

namespace SchemaForge.Core.State.Actions;

public record AddColumnAction(string TableId, ColumnDefinition Column) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Select(t => t.Id == TableId
                ? t with { Columns = [.. t.Columns, Column] }
                : t).ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new RemoveColumnAction(TableId, Column.Id);
}

public record RemoveColumnAction(string TableId, string ColumnId) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Select(t => t.Id == TableId
                ? t with { Columns = t.Columns.Where(c => c.Id != ColumnId).ToList() }
                : t).ToList(),
            Relationships = doc.Relationships
                .Where(r => r.SourceColumnId != ColumnId && r.TargetColumnId != ColumnId)
                .ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var table = doc.Tables.First(t => t.Id == TableId);
        var column = table.Columns.First(c => c.Id == ColumnId);
        var rels = doc.Relationships
            .Where(r => r.SourceColumnId == ColumnId || r.TargetColumnId == ColumnId)
            .ToList();
        return new CompoundAction([
            new AddColumnAction(TableId, column),
            .. rels.Select(r => (ISchemaAction)new AddRelationshipAction(r))
        ]);
    }
}

public record UpdateColumnAction(string TableId, ColumnDefinition Column) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with
        {
            Tables = doc.Tables.Select(t => t.Id == TableId
                ? t with { Columns = t.Columns.Select(c => c.Id == Column.Id ? Column : c).ToList() }
                : t).ToList(),
            ModifiedAt = DateTime.UtcNow
        };

    public ISchemaAction CreateInverse(SchemaDocument doc)
    {
        var table = doc.Tables.First(t => t.Id == TableId);
        var oldColumn = table.Columns.First(c => c.Id == Column.Id);
        return new UpdateColumnAction(TableId, oldColumn);
    }
}
