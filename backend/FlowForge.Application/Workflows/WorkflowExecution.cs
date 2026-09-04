namespace FlowForge.Application.Workflows;

public sealed class WorkflowExecution
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string WorkflowId { get; init; }
    public WorkflowStatus Status { get; internal set; } = WorkflowStatus.Pending;
    public DateTimeOffset StartedAt { get; internal set; }
    public DateTimeOffset? CompletedAt { get; internal set; }
    public string? Error { get; internal set; }
    public required string CorrelationId { get; init; }
    public TimeSpan? Duration { get; internal set; }
    public int InputTokens { get; internal set; }
    public int OutputTokens { get; internal set; }
    public decimal EstimatedCost { get; internal set; }
}
