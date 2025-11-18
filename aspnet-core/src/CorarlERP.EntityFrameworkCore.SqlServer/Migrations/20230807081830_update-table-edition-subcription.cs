using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class updatetableeditionsubcription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrail",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowWarning",
                table: "CarlErpSubscriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DailyPrice",
                table: "AbpEditions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "AbpEditions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrail",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "ShowWarning",
                table: "CarlErpSubscriptions");

            migrationBuilder.DropColumn(
                name: "DailyPrice",
                table: "AbpEditions");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "AbpEditions");
        }
    }
}
