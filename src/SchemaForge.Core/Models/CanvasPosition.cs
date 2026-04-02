namespace SchemaForge.Core.Models;

public record CanvasPosition
{
    public double X { get; init; }
    public double Y { get; init; }
    public double Width { get; init; } = 240;
    public double Height { get; init; }
}

public record CanvasSettings
{
    public double ViewportX { get; init; }
    public double ViewportY { get; init; }
    public double Zoom { get; init; } = 1.0;
    public int GridSize { get; init; } = 16;
    public bool ShowGrid { get; init; } = true;
}
