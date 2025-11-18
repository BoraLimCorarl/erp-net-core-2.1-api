using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddField_RoundingDigitUnitCost_AcccountCycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoundingDigitUnitCost",
                table: "CarlErpAccountCycles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
               @"
                    UPDATE CarlErpAccountCycles
                    SET RoundingDigitUnitCost = RoundingDigit;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoundingDigitUnitCost",
                table: "CarlErpAccountCycles");
        }
    }
}
