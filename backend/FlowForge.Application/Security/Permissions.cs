using FlowForge.Domain.Entities;

namespace FlowForge.Application.Security;

public static class Permissions
{
    public const string UsersView = "Users.View"; public const string UsersCreate = "Users.Create"; public const string UsersCreateMember = "Users.CreateMember"; public const string UsersCreateGuest = "Users.CreateGuest"; public const string UsersUpdate = "Users.Update"; public const string UsersDelete = "Users.Delete"; public const string UsersAssignRole = "Users.AssignRole";
    public const string AgentsView = "Agents.View"; public const string AgentsCreate = "Agents.Create"; public const string AgentsUpdate = "Agents.Update"; public const string AgentsDelete = "Agents.Delete"; public const string AgentsUse = "Agents.Use";
    public const string KnowledgeView = "Knowledge.View"; public const string KnowledgeCreate = "Knowledge.Create"; public const string KnowledgeUpdate = "Knowledge.Update"; public const string KnowledgeDelete = "Knowledge.Delete";
    public const string ConversationsView = "Conversations.View"; public const string ConversationsUse = "Conversations.Use";
    public const string ClientSettingsView = "Client.Settings.View"; public const string ClientSettingsUpdate = "Client.Settings.Update"; public const string AuditLogsView = "AuditLogs.View";

    private static readonly string[] All = typeof(Permissions).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
        .Where(f => f.FieldType == typeof(string)).Select(f => (string)f.GetValue(null)!).ToArray();

    public static IReadOnlyCollection<string> For(ClientRole role) => role switch
    {
        ClientRole.Admin => All,
        ClientRole.Editor => new[] { UsersView, UsersCreate, UsersCreateMember, UsersCreateGuest, AgentsView, AgentsCreate, AgentsUpdate, AgentsDelete, AgentsUse, KnowledgeView, KnowledgeCreate, KnowledgeUpdate, KnowledgeDelete, ConversationsView, ConversationsUse },
        ClientRole.Member => new[] { AgentsView, AgentsUse, KnowledgeView, ConversationsView, ConversationsUse },
        _ => new[] { AgentsView }
    };

    public static bool Has(ClientRole role, string permission) => For(role).Contains(permission);
}
