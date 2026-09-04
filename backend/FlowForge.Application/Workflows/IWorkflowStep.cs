namespace FlowForge.Application.Workflows;

public interface IWorkflowStep
{
    string Name { get; }

    Task ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default);
}
