namespace FlowForge.Application.Interfaces;

public interface ITextChunker
{
    IReadOnlyList<string> Chunk(string text, int maxChunkSize = 800, int overlap = 150);
}
