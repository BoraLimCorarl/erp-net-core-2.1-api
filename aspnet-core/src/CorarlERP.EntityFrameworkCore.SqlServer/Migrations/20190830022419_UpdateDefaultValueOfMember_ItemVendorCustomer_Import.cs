using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateDefaultValueOfMember_ItemVendorCustomer_Import : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Member",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Member",
                table: "CarlErpCustomers",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Member",
                table: "CarlErpVendors",
                nullable: false,
                defaultValue: 1);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
