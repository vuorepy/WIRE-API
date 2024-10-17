using Microsoft.Azure.Cosmos;
using Wire.Services;
using Wire.Settings;

var builder = WebApplication.CreateBuilder(args);

// CosmodDB Settings
builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var cosmosDBSettings = new CosmosDBSettings();
    configuration.GetSection(nameof(CosmosDBSettings)).Bind(cosmosDBSettings);

    return new CosmosClient(cosmosDBSettings.Account, cosmosDBSettings.Key);
});

// Services 
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IContentGenerationService, ContentGenerationService>();

// CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
                      });
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// App Settings
var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();


// Middleware
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

// Routes
app.MapControllers();
app.Run();

public partial class Program { }