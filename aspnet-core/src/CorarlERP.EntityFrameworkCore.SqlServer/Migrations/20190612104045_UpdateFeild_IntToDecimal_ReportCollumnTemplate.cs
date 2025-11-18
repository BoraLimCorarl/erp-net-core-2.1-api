using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateFeild_IntToDecimal_ReportCollumnTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ColumnLength",
                table: "CarlErpReportColumnTemplate",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ColumnLength",
                table: "CarlErpReportColumnTemplate",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
