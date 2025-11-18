using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddColumnImageIdBarcodeForItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "CarlErpItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "CarlErpItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_ImageId",
                table: "CarlErpItems",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_Barcode",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "Barcode" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItems_CarlErpGalleries_ImageId",
                table: "CarlErpItems",
                column: "ImageId",
                principalTable: "CarlErpGalleries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItems_CarlErpGalleries_ImageId",
                table: "CarlErpItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItems_ImageId",
                table: "CarlErpItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItems_TenantId_Barcode",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "CarlErpItems");
        }
    }
}
