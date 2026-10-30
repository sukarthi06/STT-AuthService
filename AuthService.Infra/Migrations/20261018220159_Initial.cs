using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AuthService.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "AppRoles",
                schema: "auth",
                columns: table => new
                {
                    RoleID = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppRoles", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "auth",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LicenseNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.OrganizationId);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    LicenseNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AppUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "auth",
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationDepts",
                schema: "auth",
                columns: table => new
                {
                    DeptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationDepts", x => x.DeptId);
                    table.UniqueConstraint("AK_OrganizationDepts_OrganizationId_DeptId", x => new { x.OrganizationId, x.DeptId });
                    table.ForeignKey(
                        name: "FK_OrganizationDepts_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "auth",
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationRoles",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationRoles", x => x.RoleId);
                    table.UniqueConstraint("AK_OrganizationRoles_OrganizationId_RoleId", x => new { x.OrganizationId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_OrganizationRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "auth",
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserRoles",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleID = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoles", x => new { x.UserId, x.RoleID });
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppRoles_RoleID",
                        column: x => x.RoleID,
                        principalSchema: "auth",
                        principalTable: "AppRoles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppUserRoles_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrgAddresses",
                schema: "auth",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Zip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Phone1 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Phone2 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PrimaryUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecondaryUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgAddresses", x => x.OrganizationId);
                    table.ForeignKey(
                        name: "FK_OrgAddresses_AppUsers_PrimaryUserId",
                        column: x => x.PrimaryUserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrgAddresses_AppUsers_SecondaryUserId",
                        column: x => x.SecondaryUserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrgAddresses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "auth",
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "auth",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AddressLine2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Zip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_UserAddresses_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDetails",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DOB = table.Column<DateTime>(type: "date", nullable: false),
                    Phone1 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Phone2 = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Sex = table.Column<int>(type: "integer", nullable: false),
                    SexByBirth = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserDetails_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrgUserRoles",
                schema: "auth",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    AllDepartments = table.Column<bool>(type: "boolean", nullable: false),
                    AllRoles = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    RecordStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUserRoles", x => new { x.OrganizationId, x.UserId });
                    table.CheckConstraint("CK_OrgUserRoles_Dept", "NOT (\"AllDepartments\" AND \"DepartmentId\" IS NOT NULL)");
                    table.CheckConstraint("CK_OrgUserRoles_Role", "NOT (\"AllRoles\" AND \"OrgRoleId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_OrgUserRoles_AppUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "AppUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrgUserRoles_OrganizationDepts_OrganizationId_DepartmentId",
                        columns: x => new { x.OrganizationId, x.DepartmentId },
                        principalSchema: "auth",
                        principalTable: "OrganizationDepts",
                        principalColumns: new[] { "OrganizationId", "DeptId" });
                    table.ForeignKey(
                        name: "FK_OrgUserRoles_OrganizationRoles_OrganizationId_OrgRoleId",
                        columns: x => new { x.OrganizationId, x.OrgRoleId },
                        principalSchema: "auth",
                        principalTable: "OrganizationRoles",
                        principalColumns: new[] { "OrganizationId", "RoleId" });
                    table.ForeignKey(
                        name: "FK_OrgUserRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "auth",
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppRoles_RoleName",
                schema: "auth",
                table: "AppRoles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserRoles_RoleID",
                schema: "auth",
                table: "AppUserRoles",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                schema: "auth",
                table: "AppUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_OrganizationId",
                schema: "auth",
                table: "AppUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAddresses_PrimaryUserId",
                schema: "auth",
                table: "OrgAddresses",
                column: "PrimaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgAddresses_SecondaryUserId",
                schema: "auth",
                table: "OrgAddresses",
                column: "SecondaryUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationDepts_OrganizationId_Name",
                schema: "auth",
                table: "OrganizationDepts",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationRoles_OrganizationId_RoleName",
                schema: "auth",
                table: "OrganizationRoles",
                columns: new[] { "OrganizationId", "RoleName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_LicenseNo",
                schema: "auth",
                table: "Organizations",
                column: "LicenseNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgUserRoles_OrganizationId_DepartmentId",
                schema: "auth",
                table: "OrgUserRoles",
                columns: new[] { "OrganizationId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrgUserRoles_OrganizationId_OrgRoleId",
                schema: "auth",
                table: "OrgUserRoles",
                columns: new[] { "OrganizationId", "OrgRoleId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrgUserRoles_UserId",
                schema: "auth",
                table: "OrgUserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "auth",
                table: "UserAddresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "OrgAddresses",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "OrgUserRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserDetails",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "AppRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "OrganizationDepts",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "OrganizationRoles",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "AppUsers",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "auth");
        }
    }
}
