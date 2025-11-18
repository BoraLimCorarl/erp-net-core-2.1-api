using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateValueOldDataFieldCreationTimeIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                 @"
                    Update CarlErpJournals set CreationTimeIndex = 
                    convert(bigint, REPLACE(REPLACE(Right(CONVERT(VARCHAR(10), j.CreationTime, 102) + convert(VARCHAR(8), j.CreationTime, 108), 16), '.',''), ':','') + (case when ir.TransferOrderId is null then '00' else '01' end))
                    from CarlErpJournals j
                    left join CarlErpItemReceipts ir on j.ItemReceiptId = ir.Id
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
