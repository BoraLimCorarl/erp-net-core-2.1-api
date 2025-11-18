using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Report_Tamplate_optional_TramplateId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
