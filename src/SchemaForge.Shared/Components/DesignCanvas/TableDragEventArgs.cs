namespace SchemaForge.Shared.Components.DesignCanvas;

public record TableDragEventArgs(string TableId, double ClientX, double ClientY);

public record RelationshipDrawEventArgs(string TableId, string ColumnId, double StartX, double StartY);

public record RelationshipDropEventArgs(string TableId, string ColumnId);
