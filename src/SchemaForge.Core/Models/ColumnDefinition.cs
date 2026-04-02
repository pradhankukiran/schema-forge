namespace SchemaForge.Core.Models;

public record ColumnDefinition
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Name { get; init; } = "column";
    public LogicalDataType DataType { get; init; } = LogicalDataType.Integer;
    public int? Length { get; init; }
    public int? Precision { get; init; }
    public int? Scale { get; init; }
    public bool IsPrimaryKey { get; init; }
    public bool IsAutoIncrement { get; init; }
    public bool IsNullable { get; init; } = true;
    public bool IsUnique { get; init; }
    public string? DefaultValue { get; init; }
    public string? CheckExpression { get; init; }
    public string Comment { get; init; } = "";
    public int OrdinalPosition { get; init; }
}
