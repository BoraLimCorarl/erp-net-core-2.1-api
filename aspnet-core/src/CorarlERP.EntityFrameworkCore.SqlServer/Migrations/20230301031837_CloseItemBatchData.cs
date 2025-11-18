using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CloseItemBatchData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"insert into CarlErpInventoryCostCloseItemBatchNos(Id, CreationTime, CreatorUserId, TenantId, LotId, Qty, InventoryCostCloseId)
                select NEWID(), CreationTime, CreatorUserId, TenantId, LotId, Qty, InventoryCostCloseId from CarlErpInventoryCostCloseItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
