using FlowForge.Application.Interfaces;

namespace FlowForge.Infrastructure.Services;

public sealed class TextChunker : ITextChunker
{
    public IReadOnlyList<string> Chunk(string text, int maxChunkSize = 800, int overlap = 150)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<string>();
        }

        var normalized = Regex.Replace(text, "\\s+", " ").Trim();
        var sentences = SplitIntoSentences(normalized);
        if (sentences.Count == 0)
        {
            return new[] { normalized };
        }

        var chunks = new List<string>();
        var builder = new StringBuilder();
        var currentLength = 0;
        var currentSentences = new List<string>();

        foreach (var sentence in sentences)
        {
            var sentenceLength = sentence.Length;
            if (currentLength > 0 && currentLength + sentenceLength + 1 > maxChunkSize)
            {
                chunks.Add(BuildChunk(currentSentences, overlap));
                currentSentences = TrimForOverlap(currentSentences, overlap);
                currentLength = string.Join(" ", currentSentences).Length;
                builder.Clear();
            }

            currentSentences.Add(sentence);
            currentLength = string.Join(" ", currentSentences).Length;
        }

        if (currentSentences.Count > 0)
        {
            chunks.Add(BuildChunk(currentSentences, overlap));
        }

        return chunks;
    }

    private static List<string> SplitIntoSentences(string text)
    {
        var sentences = new List<string>();
        var current = new StringBuilder();

        foreach (var ch in text)
        {
            current.Append(ch);
            if (ch is '.' or '!' or '?')
            {
                var sentence = current.ToString().Trim();
                if (sentence.Length > 0)
                {
                    sentences.Add(sentence);
                    current.Clear();
                }
            }
        }

        if (current.Length > 0)
        {
            var tail = current.ToString().Trim();
            if (tail.Length > 0)
            {
                sentences.Add(tail);
            }
        }

        return sentences;
    }

    private static string BuildChunk(List<string> sentences, int overlap)
    {
        var text = string.Join(" ", sentences).Trim();
        if (text.Length <= 800)
        {
            return text;
        }

        return text.Length <= 800 ? text : text[..800];
    }

    private static List<string> TrimForOverlap(List<string> sentences, int overlap)
    {
        var joined = string.Join(" ", sentences);
        if (joined.Length <= overlap)
        {
            return new List<string>();
        }

        var trimmed = joined[^Math.Min(overlap, joined.Length)..].Trim();
        return trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
