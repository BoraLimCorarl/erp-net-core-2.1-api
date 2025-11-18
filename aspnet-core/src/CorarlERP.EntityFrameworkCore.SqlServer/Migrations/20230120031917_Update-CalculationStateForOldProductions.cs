using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateCalculationStateForOldProductions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update CarlErpTransProductions set CalculationState = 2 where Year(Date) < 2023;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
