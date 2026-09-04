namespace FlowForge.Application.Workflows;

public sealed class WorkflowOrchestrator : IWorkflowOrchestrator
{
    public async Task<WorkflowExecution> ExecuteAsync(
        string workflowId,
        IReadOnlyList<IWorkflowStep> steps,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowId);
        ArgumentNullException.ThrowIfNull(steps);

        var execution = new WorkflowExecution
        {
            WorkflowId = workflowId,
            CorrelationId = string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString("N") : correlationId
        };
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        execution.Status = WorkflowStatus.Running;
        execution.StartedAt = DateTimeOffset.UtcNow;

        try
        {
            var context = new WorkflowContext(execution);
            foreach (var step in steps)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await step.ExecuteAsync(context, cancellationToken);
            }

            execution.Status = WorkflowStatus.Completed;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            execution.Status = WorkflowStatus.Cancelled;
            throw;
        }
        catch (Exception exception)
        {
            execution.Status = WorkflowStatus.Failed;
            execution.Error = exception.Message;
        }
        finally
        {
            stopwatch.Stop();
            execution.CompletedAt = DateTimeOffset.UtcNow;
            execution.Duration = stopwatch.Elapsed;
        }

        return execution;
    }
}
