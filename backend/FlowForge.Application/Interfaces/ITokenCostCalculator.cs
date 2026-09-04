namespace FlowForge.Application.Interfaces;

public interface ITokenCostCalculator
{
    decimal Calculate(string provider, string model, int inputTokens, int outputTokens);
}
