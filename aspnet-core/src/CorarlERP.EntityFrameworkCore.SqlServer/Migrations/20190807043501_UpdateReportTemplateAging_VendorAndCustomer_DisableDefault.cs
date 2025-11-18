using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateReportTemplateAging_VendorAndCustomer_DisableDefault : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"
                    UPDATE CarlErpReportColumnTemplate
                    SET DisableDefault = 1
                    FROM CarlErpReportTemplate rp
                    WHERE (ColumnName in ('VendorName', 'CustomerName', 'Total')                        
                        AND rp.Id = CarlErpReportColumnTemplate.ReportTemplateId                    
                        AND (rp.ReportType = 11 OR rp.ReportType = 9))                    
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
