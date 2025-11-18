using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class updateaccountCycleintableaccounttrasaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTransactionCloses_AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountCycleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountCycleId",
                principalTable: "CarlErpAccountCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpAccountTransactionCloses_AccountCycleId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.DropColumn(
                name: "AccountCycleId",
                table: "CarlErpAccountTransactionCloses");
        }
    }
}
