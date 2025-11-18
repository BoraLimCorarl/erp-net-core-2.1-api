using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateItemPriceAddCustomerType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CustomerTypeId",
                table: "CarlErpItemPrices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemPrices_CustomerTypeId",
                table: "CarlErpItemPrices",
                column: "CustomerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemPrices_CarlErpCustomerTypes_CustomerTypeId",
                table: "CarlErpItemPrices",
                column: "CustomerTypeId",
                principalTable: "CarlErpCustomerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    update CarlErpItemPrices set CustomerTypeId = c.CustomerTypeId
                    from CarlErpCustomers c 
                    where c.IsWalkIn = 1 and c.TenantId = CarlErpItemPrices.TenantId                    
                ");
            }


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemPrices_CarlErpCustomerTypes_CustomerTypeId",
                table: "CarlErpItemPrices");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemPrices_CustomerTypeId",
                table: "CarlErpItemPrices");

            migrationBuilder.DropColumn(
                name: "CustomerTypeId",
                table: "CarlErpItemPrices");
        }
    }
}
