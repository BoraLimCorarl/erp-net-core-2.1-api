using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Template_ToRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);


            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpReportTemplate
                    SET PermissionReadWrite = 2
                    WHERE TemplateType = 1;
                ");
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ReportTemplateId",
                table: "CarlErpGroupMemberItemTamplates",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
