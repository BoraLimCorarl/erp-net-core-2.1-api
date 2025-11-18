using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class FixProductionReportTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update ""CarlErpReportTemplate"" set ""ReportCategory"" = 7 where ""ReportType"" in (27, 29)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
