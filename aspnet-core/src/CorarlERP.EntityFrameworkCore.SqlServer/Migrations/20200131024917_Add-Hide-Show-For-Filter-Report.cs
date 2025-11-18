using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddHideShowForFilterReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowShowHideFilter",
                table: "CarlErpReportFilterTemplate",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowHideFilter",
                table: "CarlErpReportFilterTemplate",
                nullable: false,
                defaultValue: true);


            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @" 
                    UPDATE CarlErpReportFilterTemplate
                    SET CarlErpReportFilterTemplate.AllowShowHideFilter = 0                    
                    WHERE CarlErpReportFilterTemplate.FilterName = N'DateRange' 
	                OR CarlErpReportFilterTemplate.FilterName = N'Search'

                ");
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowShowHideFilter",
                table: "CarlErpReportFilterTemplate");

            migrationBuilder.DropColumn(
                name: "ShowHideFilter",
                table: "CarlErpReportFilterTemplate");
        }
    }
}
