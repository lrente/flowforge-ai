using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FlowForge.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FlowForge.Infrastructure.Services;

public sealed class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GenerateResponseAsync(string systemPrompt, string userMessage, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return "OpenAI API key is not configured.";
        }

        var requestBody = new
        {
            model = _configuration["OpenAI:Model"] ?? "gpt-4.1-mini",
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
