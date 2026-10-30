using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UserDetailsChange2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SexByBirth",
                schema: "auth",
                table: "UserDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SexByBirth",
                schema: "auth",
                table: "UserDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
