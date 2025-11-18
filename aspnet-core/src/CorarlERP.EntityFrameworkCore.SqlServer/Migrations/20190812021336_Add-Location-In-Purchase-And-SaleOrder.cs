using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddLocationInPurchaseAndSaleOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpSaleOrders",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpPurchaseOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_LocationId",
                table: "CarlErpSaleOrders",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_LocationId",
                table: "CarlErpPurchaseOrders",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPurchaseOrders_CarlErpLocations_LocationId",
                table: "CarlErpPurchaseOrders",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpLocations_LocationId",
                table: "CarlErpSaleOrders",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                 
                    Update po Set po.LocationId = t.LocationId
                    From CarlErpPurchaseOrders AS po
                    inner join AbpTenants as t on po.TenantId = t.Id
                    go
                    Update so Set so.LocationId = t.LocationId
                    From CarlErpSaleOrders As so
                    inner join AbpTenants as t on so.TenantId = t.Id            
                ");
            }
        }



        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPurchaseOrders_CarlErpLocations_LocationId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpSaleOrders_CarlErpLocations_LocationId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpSaleOrders_LocationId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPurchaseOrders_LocationId",
                table: "CarlErpPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpSaleOrders");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpPurchaseOrders");
        }
    }
}
