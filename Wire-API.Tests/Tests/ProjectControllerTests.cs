using Wire.Tests;
using Wire.Routes;
using Wire.Models;
using System.Net.Http.Json;
using Wire.DTO.Project;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentAssertions;
using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Wire.Settings;
using Wire.Tests.Helpers;
using Wire.Tests.Fakers;

namespace Wire.Tests;

public class ProjectControllerTests : IClassFixture<WireApiWebApplicationFactory>, IClassFixture<DatabaseHelper>
{
    private readonly HttpClient _client;

    private readonly DatabaseHelper _databaseHelper;

    public ProjectControllerTests(WireApiWebApplicationFactory factory, DatabaseHelper databaseHelper)
    {
        _client = factory.CreateClient();

        _databaseHelper = databaseHelper;
    }

    [Fact]
    public async Task GetAllProjects_ReturnsAllProjects()
    {
        // Arrange
        await _databaseHelper.AddProjectToDatabase();

        // Act
        var response = await _client.GetAsync(ApiRoutes.Project.GetProjects);
        response.EnsureSuccessStatusCode();
        var projects = await response.Content.ReadFromJsonAsync<IEnumerable<Project>>();

        // Assert
        projects.Should().NotBeEmpty();
        projects.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetProject_ValidProjectId_ReturnsProject()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();

        // Act
        var response = await _client.GetAsync(ApiRoutes.Project.GetProject.Replace("{id}", project.Id.ToString()));
        response.EnsureSuccessStatusCode();
        var getProjectDto = await response.Content.ReadFromJsonAsync<GetProjectDto>();

        // Assert
        getProjectDto.Should().NotBeNull();
        getProjectDto.Id.Should().Be(project.Id.ToString());
        getProjectDto.Name.Should().Be(project.Name);
    }

    [Fact]
    public async Task GetProject_NonExistentProjectId_ReturnsNotFound()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.GetAsync(ApiRoutes.Project.GetProject.Replace("{id}", wrongId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProject_WithValidData_ReturnsProject()
    {
        // Arrange
        CreateProjectDto createProjectDto = new CreateProjectDtoFakes().Generate();
        var json = JsonConvert.SerializeObject(createProjectDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.Project.CreateProject, content);
        response.EnsureSuccessStatusCode();
        var getProjectDto = await response.Content.ReadFromJsonAsync<GetProjectDto>();

        // Assert
        getProjectDto.Should().NotBeNull();
        getProjectDto.Id.Should().NotBeEmpty();
        getProjectDto.Name.Should().Be(createProjectDto.Name);
    }

    [Fact]
    public async Task CreateProject_WithoutName_ReturnBadRequest()
    {
        // Arrange
        CreateProjectDto createProjectDto = new CreateProjectDtoFakes().Generate();
        createProjectDto.Name = null;
        var json = JsonConvert.SerializeObject(createProjectDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.Project.CreateProject, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProject_WithValidData_ReturnsProject()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var updateProjectDto = new UpdateProjectDtoFakes().Generate();
        var json = JsonConvert.SerializeObject(updateProjectDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Project.UpdateProject.Replace("{id}", project.Id.ToString()), content);
        var getProjectDto = await response.Content.ReadFromJsonAsync<GetProjectDto>();

        // Assert
        getProjectDto.Should().NotBeNull();
        getProjectDto.Id.Should().Be(project.Id.ToString());
        getProjectDto.Name.Should().Be(updateProjectDto.Name);
    }

    [Fact]
    public async Task UpdateProject_NonExistentProjectId_ReturnsBadRequest()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();
        var project = await _databaseHelper.AddProjectToDatabase();
        var updateProjectDto = new UpdateProjectDtoFakes().Generate();
        var json = JsonConvert.SerializeObject(updateProjectDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Project.UpdateProject.Replace("{id}", wrongId), content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateProject_WithoutName_ReturnsBadRequest()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();
        var updateProjectDto = new UpdateProjectDtoFakes().Generate();
        updateProjectDto.Name = null;
        var json = JsonConvert.SerializeObject(updateProjectDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync(ApiRoutes.Project.UpdateProject.Replace("{id}", project.Id.ToString()), content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProject_ValidProjectId_ReturnsNoContent()
    {
        // Arrange
        var project = await _databaseHelper.AddProjectToDatabase();

        // Act
        var response = await _client.DeleteAsync(ApiRoutes.Project.DeleteProject.Replace("{id}", project.Id.ToString()));
        response.EnsureSuccessStatusCode();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteProject_NonExistentProjectId_ReturnsNotFound()
    {
        // Arrange
        string wrongId = Guid.NewGuid().ToString();

        // Act
        var response = await _client.DeleteAsync(ApiRoutes.Project.DeleteProject.Replace("{id}", wrongId));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
