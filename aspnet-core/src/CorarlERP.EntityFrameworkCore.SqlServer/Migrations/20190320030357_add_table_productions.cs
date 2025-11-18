using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class add_table_productions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpTransProductions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ToLocationId = table.Column<long>(nullable: false),
                    FromLocationId = table.Column<long>(nullable: false),
                    ToClassId = table.Column<long>(nullable: false),
                    FromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    ShipedStatus = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ConvertToIssueAndReceipt = table.Column<bool>(nullable: false),
                    ReceiptDate = table.Column<DateTime>(nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransProductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpClasses_FromClassId",
                        column: x => x.FromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpLocations_FromLocationId",
                        column: x => x.FromLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpClasses_ToClassId",
                        column: x => x.ToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransProductions_CarlErpLocations_ToLocationId",
                        column: x => x.ToLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpFinishItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpFinishItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpFinishItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpFinishItems_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpRawMaterialItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ProductionId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpRawMaterialItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpRawMaterialItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpRawMaterialItems_CarlErpTransProductions_ProductionId",
                        column: x => x.ProductionId,
                        principalTable: "CarlErpTransProductions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ItemId",
                table: "CarlErpFinishItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_ProductionId",
                table: "CarlErpFinishItems",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFinishItems_TenantId_CreatorUserId",
                table: "CarlErpFinishItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_ItemId",
                table: "CarlErpRawMaterialItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_ProductionId",
                table: "CarlErpRawMaterialItems",
                column: "ProductionId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpRawMaterialItems_TenantId_CreatorUserId",
                table: "CarlErpRawMaterialItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_FromClassId",
                table: "CarlErpTransProductions",
                column: "FromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_FromLocationId",
                table: "CarlErpTransProductions",
                column: "FromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ToClassId",
                table: "CarlErpTransProductions",
                column: "ToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_ToLocationId",
                table: "CarlErpTransProductions",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransProductions_TenantId_CreatorUserId_ProductionNo",
                table: "CarlErpTransProductions",
                columns: new[] { "TenantId", "CreatorUserId", "ProductionNo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpFinishItems");

            migrationBuilder.DropTable(
                name: "CarlErpRawMaterialItems");

            migrationBuilder.DropTable(
                name: "CarlErpTransProductions");
        }
    }
}
