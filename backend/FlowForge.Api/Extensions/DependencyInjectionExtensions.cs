using FlowForge.Application.Interfaces;
using FlowForge.Application.Workflows;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Configuration;
using FlowForge.Infrastructure.Persistence;
using FlowForge.Infrastructure.Repositories;
using FlowForge.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FlowForge.Api.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string? connectionString, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(string.IsNullOrWhiteSpace(connectionString)
                ? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.")
                : connectionString));
        services.AddHttpContextAccessor();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IKnowledgeRepository, KnowledgeRepository>();
        services.AddScoped<IKnowledgeDocumentRepository, KnowledgeDocumentRepository>();
        services.AddScoped<IKnowledgeChunkRepository, KnowledgeChunkRepository>();
        services.AddScoped<IKnowledgeSearchService, KnowledgeSearchService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAgentService, AgentService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IDocumentParser, DocumentParser>();
        services.AddScoped<ITextChunker, TextChunker>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IKnowledgeProcessingService, KnowledgeProcessingService>();
        services.AddScoped<KnowledgeService>();
        services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.SectionName));
        services.Configure<LLMOptions>(configuration.GetSection(LLMOptions.SectionName));
        services.AddHttpClient<IOpenAiService, OpenAiService>();
        services.AddHttpClient<OpenAiLlmProvider>();
        services.AddScoped<ILLMProvider>(serviceProvider => serviceProvider.GetRequiredService<OpenAiLlmProvider>());
        services.AddScoped<ILLMGateway, LLMGateway>();
        services.AddSingleton<ITokenCostCalculator, TokenCostCalculator>();
        services.AddSingleton<IWorkflowOrchestrator, WorkflowOrchestrator>();

        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:Key must be configured with at least 32 characters.");
        }
        var issuer = configuration["Jwt:Issuer"] ?? "FlowForge";
        var audience = configuration["Jwt:Audience"] ?? "FlowForgeClients";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        services.AddAuthorization();

        return services;
    }
}
