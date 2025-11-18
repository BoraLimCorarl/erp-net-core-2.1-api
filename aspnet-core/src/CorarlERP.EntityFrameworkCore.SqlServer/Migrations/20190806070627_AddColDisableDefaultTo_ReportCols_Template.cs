using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColDisableDefaultTo_ReportCols_Template : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisableDefault",
                table: "CarlErpReportColumnTemplate",
                nullable: false,
                defaultValue: false);
            //if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            //{
            //    migrationBuilder.Sql(
            //    @"
            //        UPDATE CarlErpReportColumnTemplate
            //        SET DisableDefault = true
            //    ");
            //}
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisableDefault",
                table: "CarlErpReportColumnTemplate");
        }
    }
}
