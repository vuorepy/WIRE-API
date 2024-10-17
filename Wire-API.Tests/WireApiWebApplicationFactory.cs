
using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wire.Models;
using Wire.Settings;

namespace Wire.Tests;

public class WireApiWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("./appsettings.test.json");
            config.AddUserSecrets<WireApiWebApplicationFactory>();
            config.AddEnvironmentVariables();
        });

        builder.ConfigureTestServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();

            // Get the CosmosDBSettings from appsettings.json
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            CosmosDBSettings cosmosDBSettings = new CosmosDBSettings();
            configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);

            services.RemoveAll<CosmosClient>();
            services.AddSingleton<CosmosClient>(serviceProvider =>
            {
                CosmosClientOptions options = new()
                {
                    HttpClientFactory = () => new HttpClient(new HttpClientHandler()
                    {
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    }),
                    ConnectionMode = ConnectionMode.Gateway,
                };

                return new CosmosClient(cosmosDBSettings.Account, cosmosDBSettings.Key, options);
            });
        });
    }
}