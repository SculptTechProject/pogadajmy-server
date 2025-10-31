using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pogadajmy_server.Migrations
{
    /// <inheritdoc />
    public partial class ChatRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomMembers_Rooms_RoomId",
                table: "RoomMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomMembers",
                table: "RoomMembers");

            migrationBuilder.RenameTable(
                name: "Rooms",
                newName: "rooms");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "messages");

            migrationBuilder.RenameTable(
                name: "RoomMembers",
                newName: "room_members");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_CreatedAt",
                table: "rooms",
                newName: "IX_rooms_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_RoomId_CreatedAt",
                table: "messages",
                newName: "IX_messages_RoomId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_RoomMembers_JoinedAt",
                table: "room_members",
                newName: "IX_room_members_JoinedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "rooms",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rooms",
                table: "rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_room_members",
                table: "room_members",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_messages_UserId",
                table: "messages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_room_members_UserId",
                table: "room_members",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_rooms_RoomId",
                table: "messages",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_users_UserId",
                table: "messages",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_room_members_rooms_RoomId",
                table: "room_members",
                column: "RoomId",
                principalTable: "rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_room_members_users_UserId",
                table: "room_members",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_rooms_RoomId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_users_UserId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_room_members_rooms_RoomId",
                table: "room_members");

            migrationBuilder.DropForeignKey(
                name: "FK_room_members_users_UserId",
                table: "room_members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rooms",
                table: "rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_UserId",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_room_members",
                table: "room_members");

            migrationBuilder.DropIndex(
                name: "IX_room_members_UserId",
                table: "room_members");

            migrationBuilder.RenameTable(
                name: "rooms",
                newName: "Rooms");

            migrationBuilder.RenameTable(
                name: "messages",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "room_members",
                newName: "RoomMembers");

            migrationBuilder.RenameIndex(
                name: "IX_rooms_CreatedAt",
                table: "Rooms",
                newName: "IX_Rooms_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_messages_RoomId_CreatedAt",
                table: "Messages",
                newName: "IX_Messages_RoomId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_room_members_JoinedAt",
                table: "RoomMembers",
                newName: "IX_RoomMembers_JoinedAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Rooms",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rooms",
                table: "Rooms",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomMembers",
                table: "RoomMembers",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomMembers_Rooms_RoomId",
                table: "RoomMembers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
