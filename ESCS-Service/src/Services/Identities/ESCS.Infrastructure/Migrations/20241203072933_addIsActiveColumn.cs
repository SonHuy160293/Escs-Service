using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCS.Infrastructure.Migrations
{
    public partial class addIsActiveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserEmailServiceConfigurations",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserApiKeys",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserEmailServiceConfigurations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserApiKeys");
        }
    }
}
