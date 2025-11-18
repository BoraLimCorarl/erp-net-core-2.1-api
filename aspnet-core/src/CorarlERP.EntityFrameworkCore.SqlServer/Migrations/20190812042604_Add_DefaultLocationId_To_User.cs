using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_DefaultLocationId_To_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DefaultLocationId",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_DefaultLocationId",
                table: "AbpUsers",
                column: "DefaultLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_CarlErpLocations_DefaultLocationId",
                table: "AbpUsers",
                column: "DefaultLocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_CarlErpLocations_DefaultLocationId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_DefaultLocationId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "DefaultLocationId",
                table: "AbpUsers");
        }
    }
}
