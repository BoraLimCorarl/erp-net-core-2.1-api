using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdatePaymentMethodLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update CarlErpPaymentMethods set Member = l.Member  
                from CarlErpPaymentMethods p
                join CarlErpLocations l on p.LocationId = l.Id;
                insert into CarlErpPaymentMethodUserGroups (Id, CreationTime, CreatorUserId, TenantId, PaymentMethodId, UserGroupId)
                select NEWID(), pm.CreationTime, pm.CreatorUserId, pm.TenantId, pm.Id, g.Id
                from CarlErpPaymentMethods pm
                join CarlErpUserGroups g on pm.LocationId = g.LocationId;
            ");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
