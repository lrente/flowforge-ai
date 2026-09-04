namespace FlowForge.Application.Workflows;

public sealed class WorkflowContext
{
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    public WorkflowContext(WorkflowExecution execution)
    {
        Execution = execution;
    }

    public WorkflowExecution Execution { get; }

    public void Set(string key, object? value) => _values[key] = value;

    public T? Get<T>(string key) => _values.TryGetValue(key, out var value) ? (T?)value : default;
}
