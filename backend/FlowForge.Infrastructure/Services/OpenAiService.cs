using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FlowForge.Application.Interfaces;
using FlowForge.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using FlowForge.Infrastructure.Configuration;
namespace FlowForge.Infrastructure.Services;

public sealed class OpenAiService(
    HttpClient httpClient,
    IOptions<OpenAIOptions> options) : IOpenAiService
{
        private readonly HttpClient _httpClient = httpClient;
        private readonly OpenAIOptions _options = options.Value;

    public async Task<string> SendAsync(string message, CancellationToken cancellationToken = default)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        var request = new
        {
            model = _options.Model,
            input = message
        };

        var json = JsonSerializer.Serialize(request);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            "https://api.openai.com/v1/responses",
            content,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(body);

        var output = document.RootElement.GetProperty("output");

        foreach (var item in output.EnumerateArray())
        {
            if (!item.TryGetProperty("content", out var contentArray))
                continue;

            foreach (var contentItem in contentArray.EnumerateArray())
            {
                if (contentItem.TryGetProperty("text", out var text))
                    return text.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default)
    {
        var apiKey = _options.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "OpenAI API key is not configured.";
        }

        var requestBody = new
        {
            model = _options.Model ?? "gpt-4.1-mini",
            input = new object[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage }
            },
            temperature = 0.7
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(json);

        if (document.RootElement.TryGetProperty("output", out var outputElement))
        {
            foreach (var item in outputElement.EnumerateArray())
            {
                if (item.TryGetProperty("type", out var typeElement) && typeElement.GetString() == "message")
                {
                    if (item.TryGetProperty("content", out var contentElement))
                    {
                        foreach (var contentItem in contentElement.EnumerateArray())
                        {
                            if (contentItem.TryGetProperty("text", out var textElement))
                            {
                                return textElement.GetString() ?? string.Empty;
                            }
                        }
                    }
                }
            }
        }

        return string.Empty;
    }
}
