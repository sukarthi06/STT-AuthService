using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UserDetailsChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Organizations_OrganizationId",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_AppUsers_OrganizationId",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "LicenseNo",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "auth",
                table: "AppUsers");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNo",
                schema: "auth",
                table: "UserDetails",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "auth",
                table: "UserDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_OrganizationId",
                schema: "auth",
                table: "UserDetails",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDetails_Organizations_OrganizationId",
                schema: "auth",
                table: "UserDetails",
                column: "OrganizationId",
                principalSchema: "auth",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserDetails_Organizations_OrganizationId",
                schema: "auth",
                table: "UserDetails");

            migrationBuilder.DropIndex(
                name: "IX_UserDetails_OrganizationId",
                schema: "auth",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "LicenseNo",
                schema: "auth",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "auth",
                table: "UserDetails");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNo",
                schema: "auth",
                table: "AppUsers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "auth",
                table: "AppUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OrganizationId",
                schema: "auth",
                table: "AppUsers",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Organizations_OrganizationId",
                schema: "auth",
                table: "AppUsers",
                column: "OrganizationId",
                principalSchema: "auth",
                principalTable: "Organizations",
                principalColumn: "OrganizationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
