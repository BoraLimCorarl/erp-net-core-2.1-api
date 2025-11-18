using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateInvoiceReceiveStatusReceiveAllToShipAll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    update CarlErpInvoices set ReceivedStatus = 6 where ReceivedStatus = 3                
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
