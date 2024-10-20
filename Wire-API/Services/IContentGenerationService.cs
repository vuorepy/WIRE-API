namespace Wire.Services;

public interface IContentGenerationService
{
  public Task<string> GenerateText(string prompt, string context);
}