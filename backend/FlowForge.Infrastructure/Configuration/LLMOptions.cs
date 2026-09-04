namespace FlowForge.Infrastructure.Configuration;

public sealed class LLMOptions
{
    public const string SectionName = "LLM";

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);
    public int MaxAttempts { get; set; } = 2;
    public Dictionary<string, ModelPricing> Pricing { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class ModelPricing
{
    public decimal InputPerMillionTokens { get; set; }
    public decimal OutputPerMillionTokens { get; set; }
}
