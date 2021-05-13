using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TelegramBot.DAL.Migrations.MsSqlMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHATS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InviteLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SlowModeDelay = table.Column<int>(type: "int", nullable: true),
                    StickerSetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanSetStickerSet = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsBot = table.Column<bool>(type: "bit", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanJoinGroups = table.Column<bool>(type: "bit", nullable: true),
                    CanReadAllGroupMessages = table.Column<bool>(type: "bit", nullable: true),
                    SupportsInlineQueries = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CHATPERMISSIONS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanSendMessages = table.Column<bool>(type: "bit", nullable: true),
                    CanSendMediaMessages = table.Column<bool>(type: "bit", nullable: true),
                    CanSendPolls = table.Column<bool>(type: "bit", nullable: true),
                    CanSendOtherMessages = table.Column<bool>(type: "bit", nullable: true),
                    CanAddWebPagePreviews = table.Column<bool>(type: "bit", nullable: true),
                    CanChangeInfo = table.Column<bool>(type: "bit", nullable: true),
                    CanInviteUsers = table.Column<bool>(type: "bit", nullable: true),
                    CanPinMessages = table.Column<bool>(type: "bit", nullable: true),
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATPERMISSIONS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CHATPERMISSIONS_CHATS_ChatId",
                        column: x => x.ChatId,
                        principalTable: "CHATS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CONTACTS",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vcard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONTACTS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CONTACTS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MESSAGES",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Caption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewChatTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteChatPhoto = table.Column<bool>(type: "bit", nullable: true),
                    GroupChatCreated = table.Column<bool>(type: "bit", nullable: true),
                    SupergroupChatCreated = table.Column<bool>(type: "bit", nullable: true),
                    ChannelChatCreated = table.Column<bool>(type: "bit", nullable: true),
                    MigrateToChatId = table.Column<long>(type: "bigint", nullable: true),
                    MigrateFromChatId = table.Column<long>(type: "bigint", nullable: true),
                    ConnectedWebsite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ForwardFromMessageId = table.Column<int>(type: "int", nullable: false),
                    ForwardSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForwardSenderName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ForwardDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MediaGroupId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGES", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MESSAGES_CHATS_ChatId",
                        column: x => x.ChatId,
                        principalTable: "CHATS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MESSAGES_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHATPERMISSIONS_ChatId",
                table: "CHATPERMISSIONS",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_CHATPERMISSIONS_Id_ChatId",
                table: "CHATPERMISSIONS",
                columns: new[] { "Id", "ChatId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CHATS_Id",
                table: "CHATS",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CONTACTS_Id",
                table: "CONTACTS",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CONTACTS_Id_UserId",
                table: "CONTACTS",
                columns: new[] { "Id", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CONTACTS_UserId",
                table: "CONTACTS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_ChatId",
                table: "MESSAGES",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_Id",
                table: "MESSAGES",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_UserId",
                table: "MESSAGES",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_Id",
                table: "USERS",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHATPERMISSIONS");

            migrationBuilder.DropTable(
                name: "CONTACTS");

            migrationBuilder.DropTable(
                name: "MESSAGES");

            migrationBuilder.DropTable(
                name: "CHATS");

            migrationBuilder.DropTable(
                name: "USERS");
        }
    }
}
