using FlowForge.Application.DTOs.Chat;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using FlowForge.Domain.Interfaces;
using FlowForge.Infrastructure.Services;
using Moq;
using Xunit;

namespace FlowForge.Tests;

public sealed class ChatServiceTests
{
    [Fact]
    public async Task SendMessageAsync_CreatesConversationAndReturnsAssistantReply()
    {
        var agentRepository = new Mock<IAgentRepository>();
        var openAiService = new Mock<IOpenAiService>();
        var conversationRepository = new Mock<IConversationRepository>();
        var messageRepository = new Mock<IMessageRepository>();

        var agent = new Agent
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Assistant",
            Description = "Test agent",
            SystemPrompt = "You are helpful.",
            Model = "gpt-4.1-mini",
            Temperature = 0.2,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var visitorId = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            AgentId = agent.Id,
            VisitorId = visitorId,
            Title = string.Empty,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        agentRepository.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(agent);
        conversationRepository.Setup(r => r.GetByAgentAndVisitorAsync(agent.Id, visitorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);
        conversationRepository.Setup(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        conversationRepository.Setup(r => r.UpdateAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        conversationRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        messageRepository.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        messageRepository.Setup(r => r.GetByConversationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Message>());
        messageRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        openAiService.Setup(s => s.GenerateResponseAsync(It.IsAny<string>(), "Hello", It.IsAny<CancellationToken>()))
            .ReturnsAsync("Hi there");

        var service = new ChatService(agentRepository.Object, openAiService.Object, conversationRepository.Object, messageRepository.Object);

        var response = await service.SendMessageAsync(agent.Id, "Hello", visitorId);

        Assert.Equal("Hi there", response.Response);
        conversationRepository.Verify(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()), Times.Once);
        messageRepository.Verify(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
