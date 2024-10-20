
using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Wire.Settings;

namespace Wire.Services;

public class ContentGenerationService : IContentGenerationService
{
    private readonly ChatClient _chatClient;

    private readonly ILogger<ContentGenerationService> _logger;

    public ContentGenerationService(IConfiguration configuration, ILogger<ContentGenerationService> logger)
    {
        var azureOpenAISettings = new AzureOpenAISettings();
        configuration.GetSection(nameof(AzureOpenAISettings)).Bind(azureOpenAISettings);

        AzureOpenAIClient azureClient = new(new Uri(azureOpenAISettings.Endpoint), new ApiKeyCredential(azureOpenAISettings.Key));
        _chatClient = azureClient.GetChatClient(azureOpenAISettings.DeploymentName);

        _logger = logger;
    }

    public async Task<string> GenerateText(string prompt, string context)
    {
        try
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(
                [
                    new AssistantChatMessage(context),
                new UserChatMessage(prompt)
                ]);

            return completion.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text");
            return null;
        }
    }
}