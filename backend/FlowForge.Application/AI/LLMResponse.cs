namespace FlowForge.Application.AI;

public sealed record LLMUsage(int InputTokens, int OutputTokens)
{
    public int TotalTokens => InputTokens + OutputTokens;
}

public sealed record LLMResponse(
    string Content,
    string Provider,
    string Model,
    LLMUsage Usage,
    TimeSpan Latency,
    string CorrelationId);
