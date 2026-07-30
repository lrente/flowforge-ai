namespace FlowForge.Application.Interfaces;

public interface IOpenAiService
{
    Task<string> SendAsync(string message, CancellationToken cancellationToken = default);
    Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default);
}
