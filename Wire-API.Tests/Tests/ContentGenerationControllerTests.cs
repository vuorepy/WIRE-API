using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Newtonsoft.Json;
using Wire.DTO.ContentGeneration;
using Wire.Routes;
using Wire.Tests.Helpers;

namespace Wire.Tests;

public class ContentGenerationControllerTests : IClassFixture<WireApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContentGenerationControllerTests(WireApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GenerateText_ValidProjectId_ReturnsAllDocuments()
    {
        // Arrange
        var generateContentDto = new GenerateContentDto() { Prompt = "Tell me a joke" };
        var json = JsonConvert.SerializeObject(generateContentDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiRoutes.ContentGeneration.GenerateText, content);
        response.EnsureSuccessStatusCode();
        var getGenerateText = await response.Content.ReadFromJsonAsync<GetGenerateText>();

        // Assert
        getGenerateText.Should().NotBeNull();
        getGenerateText.Content.Should().NotBeNullOrEmpty();
    }

}