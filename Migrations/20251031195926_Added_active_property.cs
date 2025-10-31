using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pogadajmy_server.Migrations
{
    /// <inheritdoc />
    public partial class Added_active_property : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "users");
        }
    }
}
