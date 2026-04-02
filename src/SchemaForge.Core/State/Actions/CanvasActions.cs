using SchemaForge.Core.Models;

namespace SchemaForge.Core.State.Actions;

public record UpdateCanvasSettingsAction(CanvasSettings Settings) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with { Canvas = Settings };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new UpdateCanvasSettingsAction(doc.Canvas);
}

public record UpdateNotationAction(ErdNotation Notation) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with { Notation = Notation, ModifiedAt = DateTime.UtcNow };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new UpdateNotationAction(doc.Notation);
}

public record UpdateDialectAction(SqlDialect Dialect) : ISchemaAction
{
    public SchemaDocument Apply(SchemaDocument doc) =>
        doc with { TargetDialect = Dialect, ModifiedAt = DateTime.UtcNow };

    public ISchemaAction CreateInverse(SchemaDocument doc) =>
        new UpdateDialectAction(doc.TargetDialect);
}
