using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Wire.Models;
using Wire.Settings;
using Wire.Tests.Fakers;

namespace Wire.Tests.Helpers;

public class DatabaseHelper
{
    private readonly CosmosDBSettings _cosmosDbSettings;
    private readonly CosmosClient _cosmosClient;
    private Container _projectsContainer;
    private Container _documentsContainer;
    public List<Project> Projects;
    public List<Document> Documents;

    public DatabaseHelper()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("./appsettings.test.json")
            .AddUserSecrets<DatabaseHelper>()
            .AddEnvironmentVariables();

        IConfigurationRoot configurationRoot = builder.Build();
        _cosmosDbSettings = new CosmosDBSettings();
        configurationRoot.GetSection(nameof(CosmosDBSettings)).Bind(_cosmosDbSettings);

        CosmosClientOptions options = new()
        {
            HttpClientFactory = () => new HttpClient(new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }),
            ConnectionMode = ConnectionMode.Gateway,
        };

        _cosmosClient = new CosmosClient(_cosmosDbSettings.Account, _cosmosDbSettings.Key, options);

        Projects = new List<Project>();
        Documents = new List<Document>();

        InitializeDatabase().Wait();
    }

    /// <summary>
    /// Initialize the CosmosDB database and create the containers. 
    /// After each container creation, a delay of 2 seconds is added to allow the container to be created before the next one is created. Otherwise, the next container creation willsometimes fail.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeDatabase()
    {
        Console.WriteLine($"Initializing database '{_cosmosDbSettings.DatabaseName}' on account '{_cosmosDbSettings.Account}'");
        var dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_cosmosDbSettings.DatabaseName);

        await Task.Delay(2000);

        Console.WriteLine($"Creating container '{_cosmosDbSettings.ProjectsContainerName}' on account '{_cosmosDbSettings.Account}'");
        _projectsContainer = await dbResponse.Database.CreateContainerIfNotExistsAsync(
            id: _cosmosDbSettings.ProjectsContainerName,
            partitionKeyPath: "/id"
        );

        await Task.Delay(2000);

        Console.WriteLine($"Creating container '{_cosmosDbSettings.DocumentsContainerName}' on account '{_cosmosDbSettings.Account}'");
        _documentsContainer = await dbResponse.Database.CreateContainerIfNotExistsAsync(
            id: _cosmosDbSettings.DocumentsContainerName,
            partitionKeyPath: "/id"
        );

        await Task.Delay(2000);

        Console.WriteLine("Database initialization completed.");
    }

    public async Task<Project> AddProjectToDatabase()
    {
        Project project = new ProjectFakes().Generate();
        await _projectsContainer.CreateItemAsync(project);
        Projects.Add(project);
        return project;
    }

    public async Task<Document> AddDocumentToDatabase(Guid projectId)
    {
        Document document = new DocumentFakes().Generate();
        document.ProjectId = projectId;
        await _documentsContainer.CreateItemAsync(document);
        Documents.Add(document);
        return document;
    }
}