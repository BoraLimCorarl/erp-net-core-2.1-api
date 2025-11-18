using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class TrackSerialAndExpiration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrackExpiration",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "CarlErpBatchNos",
                type: "Date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSerial",
                table: "CarlErpBatchNos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBatchNos_ExpirationDate",
                table: "CarlErpBatchNos",
                column: "ExpirationDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpBatchNos_ExpirationDate",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "TrackExpiration",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "CarlErpBatchNos");

            migrationBuilder.DropColumn(
                name: "IsSerial",
                table: "CarlErpBatchNos");
        }
    }
}
