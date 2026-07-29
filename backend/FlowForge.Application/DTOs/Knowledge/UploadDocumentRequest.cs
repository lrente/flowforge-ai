namespace FlowForge.Application.DTOs.Knowledge;

public sealed class UploadDocumentRequest
{
    public Guid AgentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public Stream? Content { get; set; }
    public long Length { get; set; }
}
