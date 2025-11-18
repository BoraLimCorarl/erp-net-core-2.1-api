using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class updateaccountCycleintableaccounttrasactionimplemment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountCycleId",
                principalTable: "CarlErpAccountCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpAccountTransactionCloses_CarlErpAccountCycles_AccountCycleId",
                table: "CarlErpAccountTransactionCloses",
                column: "AccountCycleId",
                principalTable: "CarlErpAccountCycles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
