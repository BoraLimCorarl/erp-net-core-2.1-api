using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePaymentMethodAddLocationColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpPaymentMethods",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPaymentMethods_LocationId",
                table: "CarlErpPaymentMethods",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPaymentMethods_CarlErpLocations_LocationId",
                table: "CarlErpPaymentMethods",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    update CarlErpPaymentMethods set LocationId = l.Id
                    from CarlErpLocations l
                    where l.TenantId = CarlErpPaymentMethods.TenantId and CarlErpPaymentMethods.LocationId is null                    
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPaymentMethods_CarlErpLocations_LocationId",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPaymentMethods_LocationId",
                table: "CarlErpPaymentMethods");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpPaymentMethods");
        }
    }
}
