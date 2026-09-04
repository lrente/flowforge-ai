using FlowForge.Application.Interfaces;
using FlowForge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FlowForge.Infrastructure.Services;

public sealed class TokenCostCalculator(IOptions<LLMOptions> options) : ITokenCostCalculator
{
    public decimal Calculate(string provider, string model, int inputTokens, int outputTokens)
    {
        var key = $"{provider}:{model}";
        if (!options.Value.Pricing.TryGetValue(key, out var pricing))
        {
            return 0m;
        }

        return inputTokens / 1_000_000m * pricing.InputPerMillionTokens
            + outputTokens / 1_000_000m * pricing.OutputPerMillionTokens;
    }
}
