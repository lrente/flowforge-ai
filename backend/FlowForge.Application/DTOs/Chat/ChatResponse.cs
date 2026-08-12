namespace FlowForge.Application.DTOs.Chat;

public sealed class ChatResponse
{
    public Guid ConversationId { get; set; }
    public string Response { get; set; } = string.Empty;
}