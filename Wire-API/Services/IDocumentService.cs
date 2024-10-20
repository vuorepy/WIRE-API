using Wire.Models;

namespace Wire.Services;

public interface IDocumentService
{
  public Task<Document> GetDocumentAsync(string id);
  public Task<List<Document>> GetAllDocumentsAsync();
  public Task<List<Document>> GetDocumentsByProjectIdAsync(string projectId);
  public Task<Document> CreateDocumentAsync(Document document);
  public Task<Document> UpdateDocumentAsync(string id, Document document);
  public Task<bool> RemoveDocumentAsync(string id);
}