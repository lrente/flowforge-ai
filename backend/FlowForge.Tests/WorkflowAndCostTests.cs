using FlowForge.Application.Workflows;
using FlowForge.Infrastructure.Configuration;
using FlowForge.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace FlowForge.Tests;

public sealed class WorkflowAndCostTests
{
    [Fact]
    public async Task Workflow_marks_execution_completed_after_all_steps()
    {
        var executed = new List<string>();
        var steps = new IWorkflowStep[]
        {
            new TestStep("first", executed),
            new TestStep("second", executed)
        };

        var execution = await new WorkflowOrchestrator().ExecuteAsync("chat", steps, "correlation-1");

        Assert.Equal(WorkflowStatus.Completed, execution.Status);
        Assert.Equal("correlation-1", execution.CorrelationId);
        Assert.Equal(new[] { "first", "second" }, executed);
        Assert.NotNull(execution.Duration);
        Assert.NotNull(execution.CompletedAt);
    }

    [Fact]
    public async Task Workflow_captures_failure_and_stops_following_steps()
    {
        var executed = new List<string>();
        var steps = new IWorkflowStep[]
        {
            new TestStep("first", executed),
            new FailingStep(),
            new TestStep("never", executed)
        };

        var execution = await new WorkflowOrchestrator().ExecuteAsync("chat", steps);

        Assert.Equal(WorkflowStatus.Failed, execution.Status);
        Assert.Contains("expected failure", execution.Error);
        Assert.Equal(new[] { "first" }, executed);
    }

    [Fact]
    public void Token_cost_calculator_uses_configured_rates()
    {
        var options = Options.Create(new LLMOptions
        {
            Pricing = new Dictionary<string, ModelPricing>(StringComparer.OrdinalIgnoreCase)
            {
                ["openai:test"] = new() { InputPerMillionTokens = 2m, OutputPerMillionTokens = 4m }
            }
        });

        var cost = new TokenCostCalculator(options).Calculate("openai", "test", 500_000, 250_000);

        Assert.Equal(2m, cost);
    }

    private sealed class TestStep(string name, ICollection<string> executed) : IWorkflowStep
    {
        public string Name => name;

        public Task ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default)
        {
            executed.Add(Name);
            return Task.CompletedTask;
        }
    }

    private sealed class FailingStep : IWorkflowStep
    {
        public string Name => "failure";

        public Task ExecuteAsync(WorkflowContext context, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("expected failure");
    }
}
