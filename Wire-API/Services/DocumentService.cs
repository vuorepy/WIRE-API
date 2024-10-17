using Microsoft.Azure.Cosmos;
using Wire.Models;
using Wire.Settings;
using Microsoft.Azure.Cosmos.Linq;

namespace Wire.Services
{
    public interface IDocumentService
    {
        public Task<Document> GetDocumentAsync(string id);
        public Task<List<Document>> GetAllDocumentsAsync();
        public Task<Document> CreateDocumentAsync(Document document);
        public Task<Document> UpdateDocumentAsync(string id, Document document);
        public Task<bool> RemoveDocumentAsync(string id);
    }

    public class DocumentService : IDocumentService
    {
        private readonly Container _container;
        private readonly IProjectService _projectService;

        public DocumentService(IConfiguration configuration, CosmosClient cosmosClient, IProjectService projectService)
        {
            var cosmosDBSettings = new CosmosDBSettings();
            configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);
            _container = cosmosClient.GetContainer(cosmosDBSettings.DatabaseName, cosmosDBSettings.DocumentsContainerName);

            _projectService = projectService;
        }

        public async Task<Document> CreateDocumentAsync(Document document)
        {
            var project = await _projectService.GetProjectAsync(document.ProjectId.ToString());

            if (project == null)
            {
                return null;
            }

            return await _container.CreateItemAsync(document);
        }

        public async Task<Document> GetDocumentAsync(string id)
        {
            try
            {
                return await _container.ReadItemAsync<Document>(id, new PartitionKey(id));
            }
            catch (CosmosException)
            {
                return null;
            }
        }

        public async Task<List<Document>> GetAllDocumentsAsync()
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

        public async Task<bool> RemoveDocumentAsync(string id)
        {
            try
            {
                var response = await _container.DeleteItemAsync<Document>(id, new PartitionKey(id));
                return response.StatusCode == System.Net.HttpStatusCode.NoContent;
            }
            catch (CosmosException)
            {
                return false;
            }
        }

        public async Task<Document> UpdateDocumentAsync(string id, Document document)
        {
            var project = await _projectService.GetProjectAsync(document.ProjectId.ToString());

            if (project == null)
            {
                return null;
            }

            document.Id = Guid.Parse(id);
            var response = await _container.ReplaceItemAsync(document, id, new PartitionKey(id));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return document;
        }
    }
}