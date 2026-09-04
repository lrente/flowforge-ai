namespace FlowForge.Application.AI;

public sealed record LLMRequest(
    string SystemPrompt,
    string UserMessage,
    string? Provider = null,
    string? Model = null,
    string? CorrelationId = null,
    TimeSpan? Timeout = null);
