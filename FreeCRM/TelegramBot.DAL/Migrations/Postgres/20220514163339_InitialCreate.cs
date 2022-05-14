using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TelegramBot.DAL.Migrations.Postgres
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Chats",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    InviteLink = table.Column<string>(type: "text", nullable: true),
                    SlowModeDelay = table.Column<int>(type: "integer", nullable: true),
                    StickerSetName = table.Column<string>(type: "text", nullable: true),
                    CanSetStickerSet = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsBot = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    LanguageCode = table.Column<string>(type: "text", nullable: true),
                    CanJoinGroups = table.Column<bool>(type: "boolean", nullable: true),
                    CanReadAllGroupMessages = table.Column<bool>(type: "boolean", nullable: true),
                    SupportsInlineQueries = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatPermissions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CanSendMessages = table.Column<bool>(type: "boolean", nullable: true),
                    CanSendMediaMessages = table.Column<bool>(type: "boolean", nullable: true),
                    CanSendPolls = table.Column<bool>(type: "boolean", nullable: true),
                    CanSendOtherMessages = table.Column<bool>(type: "boolean", nullable: true),
                    CanAddWebPagePreviews = table.Column<bool>(type: "boolean", nullable: true),
                    CanChangeInfo = table.Column<bool>(type: "boolean", nullable: true),
                    CanInviteUsers = table.Column<bool>(type: "boolean", nullable: true),
                    CanPinMessages = table.Column<bool>(type: "boolean", nullable: true),
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatPermissions_Chats_ChatId",
                        column: x => x.ChatId,
                        principalSchema: "public",
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Vcard = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PreviousMessageId = table.Column<long>(type: "bigint", nullable: true),
                    Caption = table.Column<string>(type: "text", nullable: true),
                    NewChatTitle = table.Column<string>(type: "text", nullable: true),
                    DeleteChatPhoto = table.Column<bool>(type: "boolean", nullable: true),
                    GroupChatCreated = table.Column<bool>(type: "boolean", nullable: true),
                    SupergroupChatCreated = table.Column<bool>(type: "boolean", nullable: true),
                    ChannelChatCreated = table.Column<bool>(type: "boolean", nullable: true),
                    MigrateToChatId = table.Column<long>(type: "bigint", nullable: true),
                    MigrateFromChatId = table.Column<long>(type: "bigint", nullable: true),
                    ConnectedWebsite = table.Column<string>(type: "text", nullable: true),
                    FromUserId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ForwardFromMessageId = table.Column<int>(type: "integer", nullable: true),
                    ForwardSignature = table.Column<string>(type: "text", nullable: true),
                    ForwardSenderName = table.Column<string>(type: "text", nullable: true),
                    ForwardDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EditDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MediaGroupId = table.Column<string>(type: "text", nullable: true),
                    AuthorSignature = table.Column<string>(type: "text", nullable: true),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatId",
                        column: x => x.ChatId,
                        principalSchema: "public",
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Updates",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MessageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Updates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Updates_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "public",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatPermissions_ChatId",
                schema: "public",
                table: "ChatPermissions",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPermissions_Id_ChatId",
                schema: "public",
                table: "ChatPermissions",
                columns: new[] { "Id", "ChatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_Id",
                schema: "public",
                table: "Chats",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Id",
                schema: "public",
                table: "Contacts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Id_UserId",
                schema: "public",
                table: "Contacts",
                columns: new[] { "Id", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_UserId",
                schema: "public",
                table: "Contacts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                schema: "public",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Id",
                schema: "public",
                table: "Messages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                schema: "public",
                table: "Messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Updates_Id",
                schema: "public",
                table: "Updates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Updates_MessageId",
                schema: "public",
                table: "Updates",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                schema: "public",
                table: "Users",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatPermissions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Contacts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Updates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Chats",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "public");
        }
    }
}
