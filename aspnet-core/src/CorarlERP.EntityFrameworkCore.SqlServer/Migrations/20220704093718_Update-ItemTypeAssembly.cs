using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateItemTypeAssembly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"update CarlErpItemTypes set DisplayInventoryAccount = 0 where Name = N'Assembly'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
