using Wire.Models;

namespace Wire.Services;
public interface IProjectService
{
    public Task<Project> GetProjectAsync(string id);
    public Task<List<Project>> GetAllProjectsAsync();
    public Task<Project> CreateProjectAsync(Project project);
    public Task<Project> UpdateProjectAsync(string id, Project project);
    public Task<bool> RemoveProjectAsync(string id);
}