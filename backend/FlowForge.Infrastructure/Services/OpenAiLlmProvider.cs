using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FlowForge.Application.AI;
using FlowForge.Application.Interfaces;
using FlowForge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FlowForge.Infrastructure.Services;

public sealed class OpenAiLlmProvider(
    HttpClient httpClient,
    IOptions<OpenAIOptions> openAiOptions,
    IOptions<LLMOptions> llmOptions) : ILLMProvider
{
    public string Name => "openai";

    public async Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(openAiOptions.Value.ApiKey))
        {
            throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        }

        var correlationId = string.IsNullOrWhiteSpace(request.CorrelationId)
            ? Guid.NewGuid().ToString("N")
            : request.CorrelationId;
        var model = string.IsNullOrWhiteSpace(request.Model) ? openAiOptions.Value.Model : request.Model;
        var timeout = request.Timeout ?? llmOptions.Value.Timeout;
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(timeout);
        using var message = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses")
        {
            Content = new StringContent(JsonSerializer.Serialize(new
            {
                model,
                input = new object[]
                {
                    new { role = "system", content = request.SystemPrompt },
                    new { role = "user", content = request.UserMessage }
                }
            }), Encoding.UTF8, "application/json")
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAiOptions.Value.ApiKey);
        message.Headers.Add("X-Correlation-Id", correlationId);

        var stopwatch = Stopwatch.StartNew();
        using var response = await httpClient.SendAsync(message, timeoutSource.Token);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(timeoutSource.Token);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: timeoutSource.Token);
        stopwatch.Stop();

        var content = ExtractContent(document.RootElement);
        var usage = document.RootElement.TryGetProperty("usage", out var usageElement)
            ? new LLMUsage(
                usageElement.TryGetProperty("input_tokens", out var input) ? input.GetInt32() : 0,
                usageElement.TryGetProperty("output_tokens", out var output) ? output.GetInt32() : 0)
            : new LLMUsage(0, 0);

        return new LLMResponse(content, Name, model, usage, stopwatch.Elapsed, correlationId);
    }

    private static string ExtractContent(JsonElement root)
    {
        if (!root.TryGetProperty("output", out var output))
        {
            return string.Empty;
        }

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var content))
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
            }
        }

        return string.Empty;
    }
}
