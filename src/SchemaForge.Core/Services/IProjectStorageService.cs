using SchemaForge.Core.Models;

namespace SchemaForge.Core.Services;

public interface IProjectStorageService
{
    Task<IReadOnlyList<ProjectSummary>> ListProjectsAsync();
    Task<Project> LoadProjectAsync(string projectId);
    Task SaveProjectAsync(Project project);
    Task DeleteProjectAsync(string projectId);
}
