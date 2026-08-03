using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FlowForge.Application.Interfaces;
using FlowForge.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FlowForge.Infrastructure.Services;

public sealed class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public EmbeddingService(HttpClient httpClient, IOptions<OpenAIOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<float>> CreateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        var embeddings = await CreateEmbeddingsAsync(new[] { text }, cancellationToken);
        return embeddings.FirstOrDefault() ?? Array.Empty<float>();
    }

    public async Task<IReadOnlyList<IReadOnlyList<float>>> CreateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        var input = texts.Where(item => !string.IsNullOrWhiteSpace(item)).Select(item => item.Trim()).ToList();
        if (input.Count == 0)
        {
            return Array.Empty<IReadOnlyList<float>>();
        }

        var requestBody = new
        {
            model = "text-embedding-3-small",
            input
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings")
        {
            Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(json);

        var results = new List<IReadOnlyList<float>>();
        foreach (var item in document.RootElement.GetProperty("data").EnumerateArray())
        {
            var embedding = item.GetProperty("embedding");
            var values = new List<float>();
            foreach (var value in embedding.EnumerateArray())
            {
                values.Add(value.GetSingle());
            }

            results.Add(values);
        }

        return results;
    }
}
