using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Report_Tamplate_optional_permission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermissionReadWrite",
                table: "CarlErpReportTemplate",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PermissionReadWrite",
                table: "CarlErpReportTemplate",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
