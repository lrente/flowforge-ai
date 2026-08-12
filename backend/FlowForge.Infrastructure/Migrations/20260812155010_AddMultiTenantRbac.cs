using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowForge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantRbac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KnowledgeDocuments_AgentId_CreatedAt",
                table: "KnowledgeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_AgentId_VisitorId_CreatedAt",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Agents_UserId_CreatedAt",
                table: "Agents");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "KnowledgeDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Conversations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "Agents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    EntityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientMemberships_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Preserve existing installations: every historical user receives a workspace and
            // their existing resources are backfilled into that workspace before tenant indexes apply.
            migrationBuilder.Sql("""
                INSERT INTO "Clients" ("Id", "Name", "Email", "IsActive", "CreatedAt", "UpdatedAt")
                SELECT gen_random_uuid(), COALESCE(NULLIF("Name", ''), 'Workspace') || '''s workspace', "Email", TRUE, NOW(), NOW()
                FROM "Users";
                INSERT INTO "ClientMemberships" ("Id", "ClientId", "UserId", "Role", "CreatedAt", "UpdatedAt")
                SELECT gen_random_uuid(), c."Id", u."Id", 1, NOW(), NOW()
                FROM "Users" u JOIN "Clients" c ON c."Email" = u."Email";
                UPDATE "Agents" a SET "ClientId" = m."ClientId" FROM "ClientMemberships" m WHERE m."UserId" = a."UserId";
                UPDATE "KnowledgeDocuments" d SET "ClientId" = a."ClientId" FROM "Agents" a WHERE a."Id" = d."AgentId";
                UPDATE "Conversations" c SET "ClientId" = a."ClientId" FROM "Agents" a WHERE a."Id" = c."AgentId";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_AgentId",
                table: "KnowledgeDocuments",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_ClientId_AgentId_CreatedAt",
                table: "KnowledgeDocuments",
                columns: new[] { "ClientId", "AgentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ClientId_AgentId_VisitorId_CreatedAt",
                table: "Conversations",
                columns: new[] { "ClientId", "AgentId", "VisitorId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_ClientId_CreatedAt",
                table: "Agents",
                columns: new[] { "ClientId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ClientId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "ClientId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberships_ClientId_UserId",
                table: "ClientMemberships",
                columns: new[] { "ClientId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientMemberships_UserId_ClientId",
                table: "ClientMemberships",
                columns: new[] { "UserId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_ClientId_Email_Status",
                table: "Invitations",
                columns: new[] { "ClientId", "Email", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_TokenHash",
                table: "Invitations",
                column: "TokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ClientMemberships");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_KnowledgeDocuments_AgentId",
                table: "KnowledgeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_KnowledgeDocuments_ClientId_AgentId_CreatedAt",
                table: "KnowledgeDocuments");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ClientId_AgentId_VisitorId_CreatedAt",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Agents_ClientId_CreatedAt",
                table: "Agents");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "KnowledgeDocuments");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Agents");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_AgentId_CreatedAt",
                table: "KnowledgeDocuments",
                columns: new[] { "AgentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AgentId_VisitorId_CreatedAt",
                table: "Conversations",
                columns: new[] { "AgentId", "VisitorId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_UserId_CreatedAt",
                table: "Agents",
                columns: new[] { "UserId", "CreatedAt" });
        }
    }
}
