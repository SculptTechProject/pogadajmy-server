using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pogadajmy_server.Migrations
{
    /// <inheritdoc />
    public partial class add_dm_rooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "room_members",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "dm_rooms",
                columns: table => new
                {
                    RoomId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserA = table.Column<Guid>(type: "uuid", nullable: false),
                    UserB = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dm_rooms", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_dm_rooms_rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dm_rooms_UserA_UserB",
                table: "dm_rooms",
                columns: new[] { "UserA", "UserB" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dm_rooms");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "room_members",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");
        }
    }
}
