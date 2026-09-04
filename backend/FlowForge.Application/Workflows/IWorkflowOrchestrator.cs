namespace FlowForge.Application.Workflows;

public interface IWorkflowOrchestrator
{
    Task<WorkflowExecution> ExecuteAsync(
        string workflowId,
        IReadOnlyList<IWorkflowStep> steps,
        string? correlationId = null,
        CancellationToken cancellationToken = default);
}
