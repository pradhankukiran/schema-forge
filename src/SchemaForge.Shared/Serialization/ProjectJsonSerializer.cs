using System.Text.Json;
using System.Text.Json.Serialization;
using SchemaForge.Core.Models;

namespace SchemaForge.Shared.Serialization;

internal static class ProjectJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static JsonElement SerializeToElement(Project project) =>
        JsonSerializer.SerializeToElement(project, Options);

    public static Project DeserializeProject(JsonElement json) =>
        JsonSerializer.Deserialize<Project>(json.GetRawText(), Options)
        ?? throw new InvalidOperationException("Failed to deserialize project");
}
