
using System.ClientModel;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Wire.Settings;

namespace Wire.Services;

public interface IContentGenerationService
{
    public Task<string> GenerateText(string propmt);
}
public class ContentGenerationService : IContentGenerationService
{

    private readonly ChatClient _chatClient;

    public ContentGenerationService(IConfiguration configuration)
    {
        var azureOpenAISettings = new AzureOpenAISettings();
        configuration.GetSection(nameof(AzureOpenAISettings)).Bind(azureOpenAISettings);

        AzureOpenAIClient azureClient = new(new Uri(azureOpenAISettings.Endpoint), new ApiKeyCredential(azureOpenAISettings.Key));
        _chatClient = azureClient.GetChatClient(azureOpenAISettings.DeploymentName);
    }

    public async Task<string> GenerateText(string prompt)
    {
        ChatCompletion completion = await _chatClient.CompleteChatAsync(
            [
                new UserChatMessage(prompt)
            ]);

        return completion.Content[0].Text;
    }
}