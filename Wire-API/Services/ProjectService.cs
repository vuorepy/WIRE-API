using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Wire.Models;
using Wire.Settings;

namespace Wire.Services;
public interface IProjectService
{
    public Task<Project> GetProjectAsync(string id);
    public Task<List<Project>> GetAllProjectsAsync();
    public Task<Project> CreateProjectAsync(Project project);
    public Task<Project> UpdateProjectAsync(string id, Project project);
    public Task<bool> RemoveProjectAsync(string id);
}

public class ProjectService : IProjectService
{
    private readonly Container _container;

    public ProjectService(IConfiguration configuration, CosmosClient cosmosClient)
    {
        var cosmosDBSettings = new CosmosDBSettings();
        configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);

        _container = cosmosClient.GetContainer(cosmosDBSettings.DatabaseName, cosmosDBSettings.ProjectsContainerName);
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        try
        {
            return await _container.CreateItemAsync(project);
        }
        catch (CosmosException)
        {
            return null;
        }
    }

    public async Task<Project> GetProjectAsync(string id)
    {
        try
        {
            return await _container.ReadItemAsync<Project>(id, new PartitionKey(id));
        }
        catch (CosmosException)
        {
            return null;
        }
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        var query = _container.GetItemLinqQueryable<Project>().ToFeedIterator();
        var projects = new List<Project>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            projects.AddRange(response);
        }

        return projects;
    }

    public async Task<bool> RemoveProjectAsync(string id)
    {
        try
        {
            var response = await _container.DeleteItemAsync<Project>(id, new PartitionKey(id));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (CosmosException)
        {
            return false;
        }
    }

    public async Task<Project> UpdateProjectAsync(string id, Project project)
    {
        try
        {
            project.Id = Guid.Parse(id);
            var response = await _container.ReplaceItemAsync(project, id, new PartitionKey(id));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return project;
        }
        catch (CosmosException)
        {
            return null;
        }
    }
}