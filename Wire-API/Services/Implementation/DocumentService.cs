using Microsoft.Azure.Cosmos;
using Wire.Models;
using Wire.Settings;
using Microsoft.Azure.Cosmos.Linq;

namespace Wire.Services;

public class DocumentService : IDocumentService
{
    private readonly Container _container;
    private readonly IProjectService _projectService;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(IConfiguration configuration, CosmosClient cosmosClient, IProjectService projectService, ILogger<DocumentService> logger)
    {
        var cosmosDBSettings = new CosmosDBSettings();
        configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);
        _container = cosmosClient.GetContainer(cosmosDBSettings.DatabaseName, cosmosDBSettings.DocumentsContainerName);

        _projectService = projectService;

        _logger = logger;
    }

    public async Task<Document> CreateDocumentAsync(Document document)
    {
        try
        {
            var project = await _projectService.GetProjectAsync(document.ProjectId.ToString());

            if (project == null)
            {
                return null;
            }

            return await _container.CreateItemAsync(document);
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error creating new document");
            return null;
        }
    }

    public async Task<Document> GetDocumentAsync(string id)
    {
        try
        {
            return await _container.ReadItemAsync<Document>(id, new PartitionKey(id));
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error getting document with id {id}", id);
            return null;
        }
    }

    public async Task<List<Document>> GetAllDocumentsAsync()
    {
        try
        {
            var query = _container.GetItemLinqQueryable<Document>().ToFeedIterator();
            var documents = new List<Document>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                documents.AddRange(response);
            }

            return documents;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error getting all documents");
            return null;
        }
    }

    public async Task<bool> RemoveDocumentAsync(string id)
    {
        try
        {
            var response = await _container.DeleteItemAsync<Document>(id, new PartitionKey(id));
            return response.StatusCode == System.Net.HttpStatusCode.NoContent;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error removing document with id {id}", id);
            return false;
        }
    }

    public async Task<Document> UpdateDocumentAsync(string id, Document document)
    {
        try
        {
            var project = await _projectService.GetProjectAsync(document.ProjectId.ToString());

            if (project == null)
            {
                return null;
            }
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error getting project with id {id} while updating document", document.ProjectId);
            return null;
        }

        document.Id = Guid.Parse(id);

        try
        {
            var response = await _container.ReplaceItemAsync(document, id, new PartitionKey(id));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return document;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error updating document with id {id}", id);
            return null;
        }
    }

    public async Task<List<Document>> GetDocumentsByProjectIdAsync(string projectId)
    {
        try
        {
            var query = _container.GetItemLinqQueryable<Document>()
                .Where(d => d.ProjectId == Guid.Parse(projectId))
                .ToFeedIterator();

            var documents = new List<Document>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                documents.AddRange(response);
            }

            return documents;
        }
        catch (CosmosException ex)
        {
            _logger.LogError(ex, "Error getting documents by project id {projectId}", projectId);
            return null;
        }
    }
}
