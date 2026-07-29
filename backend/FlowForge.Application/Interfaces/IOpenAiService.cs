namespace FlowForge.Application.Interfaces;

public interface IOpenAiService
{
    Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default);
}
