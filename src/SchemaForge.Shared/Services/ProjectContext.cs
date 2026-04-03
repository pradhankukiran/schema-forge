namespace SchemaForge.Shared.Services;

public class ProjectContext
{
    public string ProjectId { get; set; } = "";
    public bool HasStoredSnapshot { get; set; }
}
