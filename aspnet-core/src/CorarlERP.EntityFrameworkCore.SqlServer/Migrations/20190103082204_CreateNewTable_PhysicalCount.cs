using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class CreateNewTable_PhysicalCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<bool>(
            //    name: "ConvertToItemReceipt",
            //    table: "CarlErpBills",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "ItemReceiptDate",
            //    table: "CarlErpBills",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PhysicalCountNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    PhysicalCountDate = table.Column<DateTime>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    ClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCounts_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCounts_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPhysicalCountItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PhysicalCountId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    QtyChange = table.Column<decimal>(nullable: false),
                    QtyOnHand = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPhysicalCountItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPhysicalCountItems_CarlErpPhysicalCounts_PhysicalCountId",
                        column: x => x.PhysicalCountId,
                        principalTable: "CarlErpPhysicalCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_ItemId",
                table: "CarlErpPhysicalCountItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_PhysicalCountId",
                table: "CarlErpPhysicalCountItems",
                column: "PhysicalCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCountItems_TenantId_CreatorUserId",
                table: "CarlErpPhysicalCountItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_ClassId",
                table: "CarlErpPhysicalCounts",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_LocationId",
                table: "CarlErpPhysicalCounts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPhysicalCounts_TenantId_CreatorUserId_PhysicalCountNo",
                table: "CarlErpPhysicalCounts",
                columns: new[] { "TenantId", "CreatorUserId", "PhysicalCountNo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCountItems");

            migrationBuilder.DropTable(
                name: "CarlErpPhysicalCounts");

            //migrationBuilder.DropColumn(
            //    name: "ConvertToItemReceipt",
            //    table: "CarlErpBills");

            //migrationBuilder.DropColumn(
            //    name: "ItemReceiptDate",
            //    table: "CarlErpBills");
        }
    }
}
