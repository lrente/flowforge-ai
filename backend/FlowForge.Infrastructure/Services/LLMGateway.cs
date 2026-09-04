using FlowForge.Application.AI;
using FlowForge.Application.Interfaces;
using FlowForge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FlowForge.Infrastructure.Services;

public sealed class LLMGateway(
    IEnumerable<ILLMProvider> providers,
    IOptions<LLMOptions> options) : ILLMGateway
{
    public async Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        var providerName = string.IsNullOrWhiteSpace(request.Provider) ? "openai" : request.Provider;
        var provider = providers.FirstOrDefault(item => string.Equals(item.Name, providerName, StringComparison.OrdinalIgnoreCase));
        if (provider is null)
        {
            throw new InvalidOperationException($"LLM provider '{providerName}' is not configured.");
        }

        var attempts = Math.Clamp(options.Value.MaxAttempts, 1, 3);
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                return await provider.CompleteAsync(request, cancellationToken);
            }
            catch (HttpRequestException) when (attempt < attempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
            }
            catch (TimeoutException) when (attempt < attempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt), cancellationToken);
            }
        }
    }
}
