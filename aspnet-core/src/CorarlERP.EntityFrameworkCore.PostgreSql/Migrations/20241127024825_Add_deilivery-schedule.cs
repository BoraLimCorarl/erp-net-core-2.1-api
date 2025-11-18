using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_deiliveryschedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliverySchedulItemId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliverySchedulItemId",
                table: "CarlErpInvoiceItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpDeliverySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DeliveryNo = table.Column<string>(maxLength: 100, nullable: true),
                    LocationId = table.Column<long>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    InitialDeliveryDate = table.Column<DateTime>(nullable: false),
                    FinalDeliveryDate = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 520, nullable: true),
                    Memo = table.Column<string>(maxLength: 1020, nullable: true),
                    SaleOrderId = table.Column<Guid>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ReceiveStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDeliverySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySchedules_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySchedules_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySchedules_CarlErpSaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "CarlErpSaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpDeliverySheduleItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DeliveryScheduleId = table.Column<Guid>(nullable: false),
                    SaleOrderItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpDeliverySheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySheduleItems_CarlErpDeliverySchedules_Delive~",
                        column: x => x.DeliveryScheduleId,
                        principalTable: "CarlErpDeliverySchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySheduleItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpDeliverySheduleItems_CarlErpSaleOrderItems_SaleOrder~",
                        column: x => x.SaleOrderItemId,
                        principalTable: "CarlErpSaleOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_DeliverySchedulItemId",
                table: "CarlErpItemIssueItems",
                column: "DeliverySchedulItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_DeliverySchedulItemId",
                table: "CarlErpInvoiceItems",
                column: "DeliverySchedulItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_CustomerId",
                table: "CarlErpDeliverySchedules",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_LocationId",
                table: "CarlErpDeliverySchedules",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_SaleOrderId",
                table: "CarlErpDeliverySchedules",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_TenantId_CreatorUserId",
                table: "CarlErpDeliverySchedules",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_TenantId_DeliveryNo",
                table: "CarlErpDeliverySchedules",
                columns: new[] { "TenantId", "DeliveryNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySchedules_TenantId_Reference",
                table: "CarlErpDeliverySchedules",
                columns: new[] { "TenantId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySheduleItems_DeliveryScheduleId",
                table: "CarlErpDeliverySheduleItems",
                column: "DeliveryScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySheduleItems_ItemId",
                table: "CarlErpDeliverySheduleItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySheduleItems_SaleOrderItemId",
                table: "CarlErpDeliverySheduleItems",
                column: "SaleOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeliverySheduleItems_TenantId_CreatorUserId",
                table: "CarlErpDeliverySheduleItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpDeliverySheduleItems_DeliverySch~",
                table: "CarlErpInvoiceItems",
                column: "DeliverySchedulItemId",
                principalTable: "CarlErpDeliverySheduleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpDeliverySheduleItems_DeliveryS~",
                table: "CarlErpItemIssueItems",
                column: "DeliverySchedulItemId",
                principalTable: "CarlErpDeliverySheduleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInvoiceItems_CarlErpDeliverySheduleItems_DeliverySch~",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpDeliverySheduleItems_DeliveryS~",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropTable(
                name: "CarlErpDeliverySheduleItems");

            migrationBuilder.DropTable(
                name: "CarlErpDeliverySchedules");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_DeliverySchedulItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoiceItems_DeliverySchedulItemId",
                table: "CarlErpInvoiceItems");

            migrationBuilder.DropColumn(
                name: "DeliverySchedulItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "DeliverySchedulItemId",
                table: "CarlErpInvoiceItems");
        }
    }
}
