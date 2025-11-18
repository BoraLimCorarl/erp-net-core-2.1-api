using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateDatabase_ChangeCollumnName_N_FilterName_ReportInventory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @" 
                    
                    UPDATE CarlErpReportColumnTemplate
                    SET CarlErpReportColumnTemplate.ColumnName = N'ItemName'
                    FROM CarlErpReportTemplate rp
                    INNER JOIN CarlErpReportColumnTemplate rpc
                        ON rp.Id = rpc.ReportTemplateId
                    WHERE rpc.ColumnName = N'Item' 
	                    AND rp.ReportCategory = 3


                    UPDATE CarlErpReportFilterTemplate
                    SET CarlErpReportFilterTemplate.FilterName = N'Item'
                    FROM CarlErpReportTemplate rp
                    INNER JOIN CarlErpReportFilterTemplate rpf
                        ON rp.Id = rpf.ReportTemplateId
                    WHERE rpf.FilterName = N'ItemName' 
	                    AND rp.ReportCategory = 3

                ");
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
