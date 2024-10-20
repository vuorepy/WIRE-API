using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Wire.Models;
using Wire.Settings;

namespace Wire.Services;

public class ProjectService : IProjectService
{
    private readonly Container _container;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IConfiguration configuration, CosmosClient cosmosClient, ILogger<ProjectService> logger)
    {
        var cosmosDBSettings = new CosmosDBSettings();
        configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);

        _container = cosmosClient.GetContainer(cosmosDBSettings.DatabaseName, cosmosDBSettings.ProjectsContainerName);

        _logger = logger;
    }

    public async Task<Project> CreateProjectAsync(Project project)
    {
        try
        {
            return await _container.CreateItemAsync(project);
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error creating new project");
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
            _logger.LogError("Error getting project with id {id}", id);
            return null;
        }
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        try
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
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error getting all projects");
            return null;
        }
    }

    public async Task<bool> RemoveProjectAsync(string id)
    {
        try
        {
            var response = await _container.DeleteItemAsync<Project>(id, new PartitionKey(id));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error deleting project with id {id}", id);
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
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error updating project with id {id}", id);
            return null;
        }
    }
}