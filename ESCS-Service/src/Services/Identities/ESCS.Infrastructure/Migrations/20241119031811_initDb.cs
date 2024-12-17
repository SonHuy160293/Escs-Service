using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCS.Infrastructure.Migrations
{
    public partial class initDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    RoleId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEndpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Method = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ServiceId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEndpoints_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserApiKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Key = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ServiceId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    UserId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApiKeys_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserApiKeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmailServiceConfigurations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    SmtpEmail = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SmtpPassword = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    SmtpPort = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UserId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ServiceId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmailServiceConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEmailServiceConfigurations_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEmailServiceConfigurations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KeyAllowedEndpoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    IsActive = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UserApiKeyId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    EndpointId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyAllowedEndpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeyAllowedEndpoints_ServiceEndpoints_EndpointId",
                        column: x => x.EndpointId,
                        principalTable: "ServiceEndpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeyAllowedEndpoints_UserApiKeys_UserApiKeyId",
                        column: x => x.UserApiKeyId,
                        principalTable: "UserApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllowedEndpoints_EndpointId",
                table: "KeyAllowedEndpoints",
                column: "EndpointId");

            migrationBuilder.CreateIndex(
                name: "IX_KeyAllowedEndpoints_UserApiKeyId",
                table: "KeyAllowedEndpoints",
                column: "UserApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEndpoints_ServiceId",
                table: "ServiceEndpoints",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApiKeys_ServiceId",
                table: "UserApiKeys",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApiKeys_UserId",
                table: "UserApiKeys",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailServiceConfigurations_ServiceId",
                table: "UserEmailServiceConfigurations",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailServiceConfigurations_UserId",
                table: "UserEmailServiceConfigurations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyAllowedEndpoints");

            migrationBuilder.DropTable(
                name: "UserEmailServiceConfigurations");

            migrationBuilder.DropTable(
                name: "ServiceEndpoints");

            migrationBuilder.DropTable(
                name: "UserApiKeys");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
