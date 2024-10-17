using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Wire.DTO.Document;
using Wire.Models;
using Wire.Routes;
using Wire.Tests.Helpers;

namespace Wire.Tests;

public class DocumentControllerTests : IClassFixture<WireApiWebApplicationFactory>, IClassFixture<DatabaseHelper>
{
    private readonly HttpClient _client;
    private readonly DatabaseHelper _databaseHelper;

    public DocumentControllerTests(WireApiWebApplicationFactory factory, DatabaseHelper databaseHelper)
    {
        _client = factory.CreateClient();

        _databaseHelper = databaseHelper;
    }

    [Fact]
    public async Task GetAllDocuments_ValidProjectId_ReturnsAllDocuments()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        await _databaseHelper.AddDocumentToDatabase(project.Id);

        // Act
        var response = await _client.GetAsync(ApiRoutes.Document.GetDocuments);
        response.EnsureSuccessStatusCode();
        var documents = await response.Content.ReadFromJsonAsync<IEnumerable<Document>>();

        // Assert
        documents.Should().NotBeEmpty();
        documents.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetDocument_ValidDocumentId_ReturnsDocument()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var document = await _databaseHelper.AddDocumentToDatabase(project.Id);

        // Act
        var response = await _client.GetAsync(ApiRoutes.Document.GetDocument.Replace("{id}", document.Id.ToString()));
        response.EnsureSuccessStatusCode();
        var getDocumentDto = await response.Content.ReadFromJsonAsync<GetDocumentDto>();

        // Assert
        getDocumentDto.Should().NotBeNull();
        getDocumentDto.Id.Should().Be(document.Id.ToString());
        getDocumentDto.Title.Should().Be(document.Title);
        getDocumentDto.Content.Should().Be(document.Content);
    }

    [Fact]
    public async Task GetDocument_NonExistentDocumentId_ReturnsDocument()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync(ApiRoutes.Document.GetDocument.Replace("{id}", wrongId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDocument_WithValidData_ReturnsDocument()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        CreateDocumentDto createDocumentDto = new CreateDocumentDtoFakes().Generate();
        createDocumentDto.ProjectId = project.Id;
        var json = JsonConvert.SerializeObject(createDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.Document.CreateDocument, content);
        response.EnsureSuccessStatusCode();
        var getDocumentDto = await response.Content.ReadFromJsonAsync<GetDocumentDto>();

        // Assert
        getDocumentDto.Should().NotBeNull();
        getDocumentDto.ProjectId.Should().Be(createDocumentDto.ProjectId.ToString());
        getDocumentDto.Title.Should().Be(createDocumentDto.Title);
        getDocumentDto.Content.Should().Be(createDocumentDto.Content);
    }

    [Fact]
    public async Task CreateDocument_NonExistentProjectId_ReturnsBadRequest()
    {
        // Arrange
        CreateDocumentDto createDocumentDto = new CreateDocumentDtoFakes().Generate();
        createDocumentDto.ProjectId = Guid.NewGuid();
        var json = JsonConvert.SerializeObject(createDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.Document.CreateDocument, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDocument_NonExistentProjectId_ReturnsDocument()
    {
        // Arrange
        CreateDocumentDto createDocumentDto = new CreateDocumentDtoFakes().Generate();
        createDocumentDto.ProjectId = Guid.NewGuid();
        var json = JsonConvert.SerializeObject(createDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.Document.CreateDocument, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDocument_WithValidData_ReturnsDocument()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var document = await _databaseHelper.AddDocumentToDatabase(project.Id);
        var updateDocumentDto = new UpdateDocumentDtoFakes().Generate();
        updateDocumentDto.ProjectId = project.Id;
        var json = JsonConvert.SerializeObject(updateDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Document.UpdateDocument.Replace("{id}", document.Id.ToString()), content);
        response.EnsureSuccessStatusCode();
        var getDocumentDto = await response.Content.ReadFromJsonAsync<GetDocumentDto>();

        // Assert
        getDocumentDto.Should().NotBeNull();
        getDocumentDto.Id.Should().Be(document.Id.ToString());
        getDocumentDto.ProjectId.Should().Be(document.ProjectId.ToString());
        getDocumentDto.Title.Should().Be(updateDocumentDto.Title);
        getDocumentDto.Content.Should().Be(updateDocumentDto.Content);
    }

    [Fact]
    public async Task UpdateDocument_NonExistentDocumentId_ReturnsDocument()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();
        var updateDocumentDto = new UpdateDocumentDtoFakes().Generate();
        var json = JsonConvert.SerializeObject(updateDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Document.UpdateDocument.Replace("{id}", wrongId), content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDocument_NonExistentProjectId_ReturnsDocument()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var document = await _databaseHelper.AddDocumentToDatabase(project.Id);
        string wrongId = Guid.NewGuid().ToString();
        var updateDocumentDto = new UpdateDocumentDtoFakes().Generate();
        var json = JsonConvert.SerializeObject(updateDocumentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Document.UpdateDocument.Replace("{id}", wrongId), content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteDocument_ValidDocumentId_ReturnsNoContent()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var document = await _databaseHelper.AddDocumentToDatabase(project.Id);

        // Act
        var response = await _client.DeleteAsync(ApiRoutes.Document.DeleteDocument.Replace("{id}", document.Id.ToString()));
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteDocument_NonExistentDocumentId_ReturnsNotFound()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(ApiRoutes.Document.DeleteDocument.Replace("{id}", wrongId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}