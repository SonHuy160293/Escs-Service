using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCS.Infrastructure.Migrations
{
    public partial class updateEmailConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<string>(
                name: "SmtpServer",
                table: "UserEmailServiceConfigurations",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "smtp.gmail.com");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnableSsl",
                table: "UserEmailServiceConfigurations");

            migrationBuilder.DropColumn(
                name: "SmtpServer",
                table: "UserEmailServiceConfigurations");
        }
    }
}
