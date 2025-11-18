using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateField_In_ReportFilterTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultValueId",
                table: "CarlErpReportFilterTemplate",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FilterType",
                table: "CarlErpReportFilterTemplate",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValueId",
                table: "CarlErpReportFilterTemplate");

            migrationBuilder.DropColumn(
                name: "FilterType",
                table: "CarlErpReportFilterTemplate");
        }
    }
}
