using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class BOMAndKitchenOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisplayItemMenu",
                table: "CarlErpItemTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "KitchenOrderId",
                table: "CarlErpItemIssues",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "KitchenOrderItemAndBOMItemId",
                table: "CarlErpItemIssueItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpBOMs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 125, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBOMs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBOMs_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpKitchenOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: true),
                    OrderNumber = table.Column<string>(maxLength: 128, nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    MultiCurrencyId = table.Column<long>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencySubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    BillingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    BillingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    BillingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    ShippingAddress_Street = table.Column<string>(maxLength: 512, nullable: true),
                    ShippingAddress_CityTown = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_Province = table.Column<string>(maxLength: 256, nullable: true),
                    ShippingAddress_PostalCode = table.Column<string>(maxLength: 64, nullable: true),
                    ShippingAddress_Country = table.Column<string>(maxLength: 3, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ClassId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpKitchenOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrders_CarlErpCurrencies_MultiCurrencyId",
                        column: x => x.MultiCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBOMItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    BomId = table.Column<Guid>(nullable: false),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBOMItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBOMItems_CarlErpBOMs_BomId",
                        column: x => x.BomId,
                        principalTable: "CarlErpBOMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBOMItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpKitchenOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    KitchenOrderId = table.Column<Guid>(nullable: false),
                    BOMId = table.Column<Guid>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyUnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    MultiCurrencyTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpKitchenOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_CarlErpBOMs_BOMId",
                        column: x => x.BOMId,
                        principalTable: "CarlErpBOMs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_CarlErpKitchenOrders_KitchenOrderId",
                        column: x => x.KitchenOrderId,
                        principalTable: "CarlErpKitchenOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_AbpUsers_LastModifierUserId",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpKitchenOrderItemAndBOMItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalQty = table.Column<decimal>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    BomItemId = table.Column<Guid>(nullable: false),
                    KitchenOrderItemId = table.Column<Guid>(nullable: false),
                    LotId = table.Column<long>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpKitchenOrderItemAndBOMItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_CarlErpBOMItems_BomItemId",
                        column: x => x.BomItemId,
                        principalTable: "CarlErpBOMItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_AbpUsers_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_CarlErpKitchenOrderItems~",
                        column: x => x.KitchenOrderItemId,
                        principalTable: "CarlErpKitchenOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_AbpUsers_LastModifierUse~",
                        column: x => x.LastModifierUserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_CarlErpLots_LotId",
                        column: x => x.LotId,
                        principalTable: "CarlErpLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpKitchenOrderItemAndBOMItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_KitchenOrderId",
                table: "CarlErpItemIssues",
                column: "KitchenOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_KitchenOrderItemAndBOMItemId",
                table: "CarlErpItemIssueItems",
                column: "KitchenOrderItemAndBOMItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBOMItems_BomId",
                table: "CarlErpBOMItems",
                column: "BomId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBOMItems_ItemId",
                table: "CarlErpBOMItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBOMs_IsDefault",
                table: "CarlErpBOMs",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBOMs_ItemId",
                table: "CarlErpBOMs",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBOMs_Name",
                table: "CarlErpBOMs",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_BomItemId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "BomItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_CreatorUserId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_ItemId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_KitchenOrderItemId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "KitchenOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_LastModifierUserId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_LotId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_TaxId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItemAndBOMItems_TenantId_CreatorUserId",
                table: "CarlErpKitchenOrderItemAndBOMItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_BOMId",
                table: "CarlErpKitchenOrderItems",
                column: "BOMId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_CreatorUserId",
                table: "CarlErpKitchenOrderItems",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_ItemId",
                table: "CarlErpKitchenOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_KitchenOrderId",
                table: "CarlErpKitchenOrderItems",
                column: "KitchenOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_LastModifierUserId",
                table: "CarlErpKitchenOrderItems",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_TaxId",
                table: "CarlErpKitchenOrderItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrderItems_TenantId_CreatorUserId",
                table: "CarlErpKitchenOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_ClassId",
                table: "CarlErpKitchenOrders",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_CreatorUserId",
                table: "CarlErpKitchenOrders",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_CurrencyId",
                table: "CarlErpKitchenOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_CustomerId",
                table: "CarlErpKitchenOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_LastModifierUserId",
                table: "CarlErpKitchenOrders",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_LocationId",
                table: "CarlErpKitchenOrders",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_MultiCurrencyId",
                table: "CarlErpKitchenOrders",
                column: "MultiCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_TenantId_OrderNumber",
                table: "CarlErpKitchenOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_TenantId_Reference",
                table: "CarlErpKitchenOrders",
                columns: new[] { "TenantId", "Reference" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpKitchenOrders_TenantId_CreatorUserId_OrderNumber",
                table: "CarlErpKitchenOrders",
                columns: new[] { "TenantId", "CreatorUserId", "OrderNumber" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpKitchenOrderItemAndBOMItems_Ki~",
                table: "CarlErpItemIssueItems",
                column: "KitchenOrderItemAndBOMItemId",
                principalTable: "CarlErpKitchenOrderItemAndBOMItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpKitchenOrders_KitchenOrderId",
                table: "CarlErpItemIssues",
                column: "KitchenOrderId",
                principalTable: "CarlErpKitchenOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"INSERT INTO  ""CarlErpJournalTransactionTypes""(""Id"",""CreationTime"" ,""CreatorUserId"" ,""Name"",""IsIssue"",""IsDefault"",""Active"",""InventoryTransactionType"",""TenantId"")
                                SELECT gen_random_uuid(), NOW(), t2.""CreatorUserId"", t1.IndxName, t1.IdxIssue, '1', '1', t1.Idx, t2.""Id"" FROM(
                                 select 17 Idx, 'Kitchen Order' IndxName, true IdxIssue
                                ) as t1
                                full join ""AbpTenants"" as t2
                                on 1 = 1
                                where t2.""IsDeleted"" = false");

            migrationBuilder.Sql(@"Insert Into ""CarlErpBOMs"" (""Id"",""CreationTime"",""CreatorUserId"",""LastModificationTime"",""LastModifierUserId"",""TenantId"",""ItemId"",""IsDefault"",""Name"",""IsActive"",""Description"",""Qty"")
                                    select gen_random_uuid(), item.""CreationTime"", item.""CreatorUserId"", item.""LastModificationTime"", item.""LastModifierUserId"", item.""TenantId"", item.""Id"", true, 'Standard', item.""IsActive"", null, 1
                                    from ""CarlErpItems"" as item
                                    join ""CarlErpItemTypes"" as itemType on item.""ItemTypeId"" = itemType.""Id""
                                    Where itemType.""Name"" = 'Assembly'");

            migrationBuilder.Sql(@"Insert Into ""CarlErpBOMItems""(""Id"", ""CreationTime"", ""CreatorUserId"", ""LastModificationTime"", ""LastModifierUserId"", ""TenantId"", ""ItemId"", ""BomId"", ""Qty"")
                                select gen_random_uuid(), item.""CreationTime"", item.""CreatorUserId"", item.""LastModificationTime"", item.""LastModifierUserId"", item.""TenantId"", item.""ItemId"", b.""Id"", item.""Quantity""
                                from ""CarlErpSubItems"" as item
                                join ""CarlErpBOMs"" as b on item.""ParentSubItemId"" = b.""ItemId""");

            migrationBuilder.Sql(@"update ""CarlErpItems"" set ""InventoryAccountId"" = null where ""Id"" in (
                                select i.""Id"" from ""CarlErpItems"" i
                                join ""CarlErpItemTypes"" t on i.""ItemTypeId"" = t.""Id""
                                where t.""Name"" = 'Assembly')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueItems_CarlErpKitchenOrderItemAndBOMItems_Ki~",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpKitchenOrders_KitchenOrderId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropTable(
                name: "CarlErpKitchenOrderItemAndBOMItems");

            migrationBuilder.DropTable(
                name: "CarlErpBOMItems");

            migrationBuilder.DropTable(
                name: "CarlErpKitchenOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpBOMs");

            migrationBuilder.DropTable(
                name: "CarlErpKitchenOrders");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_KitchenOrderId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueItems_KitchenOrderItemAndBOMItemId",
                table: "CarlErpItemIssueItems");

            migrationBuilder.DropColumn(
                name: "DisplayItemMenu",
                table: "CarlErpItemTypes");

            migrationBuilder.DropColumn(
                name: "KitchenOrderId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "KitchenOrderItemAndBOMItemId",
                table: "CarlErpItemIssueItems");
        }
    }
}
