using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace hub.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeKeyToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_User_UsersId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_SenderId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.DropIndex(
                name: "IX_ChatUser_UsersId",
                table: "ChatUser");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Message",
                newName: "SenderSystem");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "ChatUser",
                newName: "UsersSystem");

            migrationBuilder.AddColumn<string>(
                name: "SenderExternalId",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsersExternalId",
                table: "ChatUser",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                columns: new[] { "System", "ExternalId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "ChatsId", "UsersSystem", "UsersExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderSystem_SenderExternalId",
                table: "Message",
                columns: new[] { "SenderSystem", "SenderExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UsersSystem_UsersExternalId",
                table: "ChatUser",
                columns: new[] { "UsersSystem", "UsersExternalId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_User_UsersSystem_UsersExternalId",
                table: "ChatUser",
                columns: new[] { "UsersSystem", "UsersExternalId" },
                principalTable: "User",
                principalColumns: new[] { "System", "ExternalId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_SenderSystem_SenderExternalId",
                table: "Message",
                columns: new[] { "SenderSystem", "SenderExternalId" },
                principalTable: "User",
                principalColumns: new[] { "System", "ExternalId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_User_UsersSystem_UsersExternalId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_SenderSystem_SenderExternalId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Message_SenderSystem_SenderExternalId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.DropIndex(
                name: "IX_ChatUser_UsersSystem_UsersExternalId",
                table: "ChatUser");

            migrationBuilder.DropColumn(
                name: "SenderExternalId",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UsersExternalId",
                table: "ChatUser");

            migrationBuilder.RenameColumn(
                name: "SenderSystem",
                table: "Message",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "UsersSystem",
                table: "ChatUser",
                newName: "UsersId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "User",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "ChatsId", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_UsersId",
                table: "ChatUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_User_UsersId",
                table: "ChatUser",
                column: "UsersId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
