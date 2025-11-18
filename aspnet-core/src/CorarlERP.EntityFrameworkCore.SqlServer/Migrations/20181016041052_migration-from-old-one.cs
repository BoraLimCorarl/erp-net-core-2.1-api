using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class migrationfromoldone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarlErpAccountCycles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountCycles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpAccountTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    AccountTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCache",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    KeyName = table.Column<string>(maxLength: 512, nullable: false),
                    KeyValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCache", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpClasses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ClassName = table.Column<string>(maxLength: 512, nullable: false),
                    ClassParent = table.Column<bool>(nullable: false),
                    ParentClassId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpClasses_CarlErpClasses_ParentClassId",
                        column: x => x.ParentClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCurrencies",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Code = table.Column<string>(maxLength: 16, nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    Symbol = table.Column<string>(maxLength: 16, nullable: false),
                    PluralName = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCurrencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpFormats",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Key = table.Column<string>(maxLength: 50, nullable: true),
                    Web = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpFormats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    DisplayInventoryAccount = table.Column<bool>(nullable: false),
                    DisplayPurchase = table.Column<bool>(nullable: false),
                    DisplaySale = table.Column<bool>(nullable: false),
                    DisplayReorderPoint = table.Column<bool>(nullable: false),
                    DisplayTrackSerialNumber = table.Column<bool>(nullable: false),
                    DisplaySubItem = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpLocations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    LocationName = table.Column<string>(maxLength: 512, nullable: false),
                    LocationParent = table.Column<bool>(nullable: false),
                    ParentLocationId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpLocations_CarlErpLocations_ParentLocationId",
                        column: x => x.ParentLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(maxLength: 512, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpProperties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTaxes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TaxName = table.Column<string>(maxLength: 32, nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTaxes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransferOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    TransferDate = table.Column<DateTime>(nullable: false),
                    TransferToLocationId = table.Column<long>(nullable: false),
                    TransferFromLocationId = table.Column<long>(nullable: false),
                    TransferToClassId = table.Column<long>(nullable: false),
                    TransferFromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    ShipedStatus = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransferOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpClasses_TransferFromClassId",
                        column: x => x.TransferFromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpLocations_TransferFromLocationId",
                        column: x => x.TransferFromLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpClasses_TransferToClassId",
                        column: x => x.TransferToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrders_CarlErpLocations_TransferToLocationId",
                        column: x => x.TransferToLocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPropertyValues",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Value = table.Column<string>(maxLength: 512, nullable: false),
                    PropertyId = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPropertyValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPropertyValues_CarlErpProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpChartOfAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    AccountCode = table.Column<string>(maxLength: 16, nullable: false),
                    AccountName = table.Column<string>(maxLength: 512, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    AccountTypeId = table.Column<long>(nullable: false),
                    ParentAccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpChartOfAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpAccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "CarlErpAccountTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpChartOfAccounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpChartOfAccounts_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    CustomerCode = table.Column<string>(maxLength: 256, nullable: false),
                    CustomerName = table.Column<string>(maxLength: 512, nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Website = table.Column<string>(maxLength: 512, nullable: true),
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
                    Remark = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SameAsShippngAddress = table.Column<bool>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomers_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemCode = table.Column<string>(maxLength: 256, nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    ItemName = table.Column<string>(maxLength: 512, nullable: false),
                    PurchaseCost = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    SalePrice = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    ReorderPoint = table.Column<decimal>(type: "decimal(19,6)", nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TrackSerial = table.Column<bool>(nullable: false),
                    ItemTypeId = table.Column<long>(nullable: false),
                    SaleCurrenyId = table.Column<long>(nullable: true),
                    PurchaseCurrencyId = table.Column<long>(nullable: true),
                    SaleAccountId = table.Column<Guid>(nullable: true),
                    PurchaseAccountId = table.Column<Guid>(nullable: true),
                    InventoryAccountId = table.Column<Guid>(nullable: true),
                    PurchaseTaxId = table.Column<long>(nullable: true),
                    SaleTaxId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_InventoryAccountId",
                        column: x => x.InventoryAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "CarlErpItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_PurchaseAccountId",
                        column: x => x.PurchaseAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpCurrencies_PurchaseCurrencyId",
                        column: x => x.PurchaseCurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpTaxes_PurchaseTaxId",
                        column: x => x.PurchaseTaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpChartOfAccounts_SaleAccountId",
                        column: x => x.SaleAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpCurrencies_SaleCurrenyId",
                        column: x => x.SaleCurrenyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItems_CarlErpTaxes_SaleTaxId",
                        column: x => x.SaleTaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    VendorCode = table.Column<string>(maxLength: 256, nullable: false),
                    VendorName = table.Column<string>(maxLength: 512, nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Websit = table.Column<string>(maxLength: 512, nullable: true),
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
                    Remark = table.Column<string>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    SameAsShippngAddress = table.Column<bool>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendors_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(maxLength: 256, nullable: true),
                    LastName = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayNameAs = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerContactPersons_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCustomerCredits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(nullable: false),
                    ShipedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCustomerCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCredits_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCustomerCredits_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferOrderId = table.Column<Guid>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: true),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TransactionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssues_CarlErpTransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSaleOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
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
                    OrderNumber = table.Column<string>(maxLength: 128, nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ETD = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReceiveStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSaleOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrders_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PropertyId = table.Column<long>(nullable: false),
                    PropertyValueId = table.Column<long>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpProperties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemProperties_CarlErpPropertyValues_PropertyValueId",
                        column: x => x.PropertyValueId,
                        principalTable: "CarlErpPropertyValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSubItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ParentSubItemId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSubItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSubItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSubItems_CarlErpItems_ParentSubItemId",
                        column: x => x.ParentSubItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpTransferOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransferOrderId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransferOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpTransferOrderItems_CarlErpTransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 64, nullable: true),
                    FirstName = table.Column<string>(maxLength: 256, nullable: true),
                    LastName = table.Column<string>(maxLength: 256, nullable: true),
                    DisplayNameAs = table.Column<string>(maxLength: 512, nullable: true),
                    PhoneNumber = table.Column<string>(maxLength: 128, nullable: true),
                    VenderId = table.Column<Guid>(nullable: false),
                    IsPrimary = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpContactPersons_CarlErpVendors_VenderId",
                        column: x => x.VenderId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: true),
                    TransactionType = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    TransferOrderId = table.Column<Guid>(nullable: true),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpTransferOrders_TransferOrderId",
                        column: x => x.TransferOrderId,
                        principalTable: "CarlErpTransferOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceipts_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
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
                    OrderNumber = table.Column<string>(maxLength: 128, nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    ETA = table.Column<DateTime>(nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    CurrencyId = table.Column<long>(nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ReceiveStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrders_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(nullable: false),
                    ShipedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false),
                    TotalPaid = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCredit_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCredit_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpCusotmerCreditDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpCusotmerCreditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpCusotmerCreditDetails_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarlErpCusotmerCreditDetails_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpCusotmerCreditDetails_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    CustomerCreditId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FiFo = table.Column<bool>(nullable: false),
                    TotalOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDue = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    CustomerCreditId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePayments_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    ItemIssueId = table.Column<Guid>(nullable: true),
                    CustomerId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    OpenBalance = table.Column<decimal>(nullable: false),
                    TotalPaid = table.Column<decimal>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ETD = table.Column<DateTime>(nullable: false),
                    ReceivedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoices_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpSaleOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    SaleOrderId = table.Column<Guid>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    TotalIssueQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalInvoiceQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalIssueInvoiceQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    CheckStatus = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpSaleOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpSaleOrders_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "CarlErpSaleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpSaleOrderItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    ItemReceiptId = table.Column<Guid>(nullable: true),
                    VendorId = table.Column<Guid>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    SubTotal = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    OpenBalance = table.Column<decimal>(nullable: false),
                    TotalPaid = table.Column<decimal>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ETA = table.Column<DateTime>(nullable: false),
                    ReceivedStatus = table.Column<int>(nullable: false),
                    PaidStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBills_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBills_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBills_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    TaxId = table.Column<long>(nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Unit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(6,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalBillQty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalReceiptBillQty = table.Column<decimal>(nullable: false),
                    CheckStatus = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpPurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "CarlErpPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPurchaseOrderItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCredit",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorCreditId = table.Column<Guid>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false),
                    TransactionType = table.Column<int>(nullable: false),
                    LocationId = table.Column<long>(nullable: false),
                    SameAsShippingAddress = table.Column<bool>(nullable: false),
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
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCredit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "CarlErpLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCredit_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPayBills",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    FiFo = table.Column<bool>(nullable: false),
                    TotalOpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalPaymentDue = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    ReceiveFrom = table.Column<int>(nullable: false),
                    VendorCreditId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBills_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpVendorCreditDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    VendorCreditId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpVendorCreditDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpVendorCreditDetails_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptCustomerCreditItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemReceiptCustomerCreditId = table.Column<Guid>(nullable: false),
                    CustomerCreditItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptCustomerCreditItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpCusotmerCreditDetails_CustomerCreditItemId",
                        column: x => x.CustomerCreditItemId,
                        principalTable: "CarlErpCusotmerCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptCustomerCreditItem_CarlErpItemReceiptCustomerCredit_ItemReceiptCustomerCreditId",
                        column: x => x.ItemReceiptCustomerCreditId,
                        principalTable: "CarlErpItemReceiptCustomerCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpReceivePaymentDeails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ReceivePaymentId = table.Column<Guid>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    CustomerId = table.Column<Guid>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpReceivePaymentDeails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpCustomers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CarlErpCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpReceivePaymentDeails_CarlErpReceivePayments_ReceivePaymentId",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemIssueId = table.Column<Guid>(nullable: false),
                    SaleOrderItemId = table.Column<Guid>(nullable: true),
                    TransferOrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpSaleOrderItems_SaleOrderItemId",
                        column: x => x.SaleOrderItemId,
                        principalTable: "CarlErpSaleOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueItems_CarlErpTransferOrderItems_TransferOrderItemId",
                        column: x => x.TransferOrderItemId,
                        principalTable: "CarlErpTransferOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemReceiptItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemReceiptId = table.Column<Guid>(nullable: false),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TransferOrderItemId = table.Column<Guid>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemReceiptItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpPurchaseOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemReceiptItems_CarlErpTransferOrderItems_TransferOrderItemId",
                        column: x => x.TransferOrderItemId,
                        principalTable: "CarlErpTransferOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpJournals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    JournalNo = table.Column<string>(maxLength: 256, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CurrencyId = table.Column<long>(nullable: false),
                    ClassId = table.Column<long>(nullable: true),
                    GeneralJournalId = table.Column<Guid>(nullable: true),
                    Reference = table.Column<string>(maxLength: 256, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    JournalType = table.Column<int>(nullable: false),
                    VendorCreditId = table.Column<Guid>(nullable: true),
                    BillId = table.Column<Guid>(nullable: true),
                    ItemReceiptId = table.Column<Guid>(nullable: true),
                    ItemReceiptCustomerCreditId = table.Column<Guid>(nullable: true),
                    ItemIssueVendorCreditId = table.Column<Guid>(nullable: true),
                    ItemIssueId = table.Column<Guid>(nullable: true),
                    PayBillId = table.Column<Guid>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: true),
                    ReceivePaymentId = table.Column<Guid>(nullable: true),
                    CustomerCreditId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpJournals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpClasses_ClassId",
                        column: x => x.ClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpCurrencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "CarlErpCurrencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpCustomerCredits_CustomerCreditId",
                        column: x => x.CustomerCreditId,
                        principalTable: "CarlErpCustomerCredits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpJournals_GeneralJournalId",
                        column: x => x.GeneralJournalId,
                        principalTable: "CarlErpJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemIssues_ItemIssueId",
                        column: x => x.ItemIssueId,
                        principalTable: "CarlErpItemIssues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemIssueVendorCredit_ItemIssueVendorCreditId",
                        column: x => x.ItemIssueVendorCreditId,
                        principalTable: "CarlErpItemIssueVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemReceiptCustomerCredit_ItemReceiptCustomerCreditId",
                        column: x => x.ItemReceiptCustomerCreditId,
                        principalTable: "CarlErpItemReceiptCustomerCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpItemReceipts_ItemReceiptId",
                        column: x => x.ItemReceiptId,
                        principalTable: "CarlErpItemReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpReceivePayments_ReceivePaymentId",
                        column: x => x.ReceivePaymentId,
                        principalTable: "CarlErpReceivePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournals_CarlErpVendorCredit_VendorCreditId",
                        column: x => x.VendorCreditId,
                        principalTable: "CarlErpVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpPayBillDeail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PayBillId = table.Column<Guid>(nullable: false),
                    BillId = table.Column<Guid>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    OpenBalance = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpPayBillDeail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpPayBills_PayBillId",
                        column: x => x.PayBillId,
                        principalTable: "CarlErpPayBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpPayBillDeail_CarlErpVendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "CarlErpVendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemIssueVendorCreditItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    ItemIssueVendorCreditId = table.Column<Guid>(nullable: false),
                    VendorCreditItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemIssueVendorCreditItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpItemIssueVendorCredit_ItemIssueVendorCreditId",
                        column: x => x.ItemIssueVendorCreditId,
                        principalTable: "CarlErpItemIssueVendorCredit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemIssueVendorCreditItem_CarlErpVendorCreditDetails_VendorCreditItemId",
                        column: x => x.VendorCreditItemId,
                        principalTable: "CarlErpVendorCreditDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    ItemIssueItemId = table.Column<Guid>(nullable: true),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsItemIssue = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpInvoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "CarlErpInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpItemIssueItems_ItemIssueItemId",
                        column: x => x.ItemIssueItemId,
                        principalTable: "CarlErpItemIssueItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpSaleOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpSaleOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpInvoiceItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpBillItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    BillId = table.Column<Guid>(nullable: false),
                    ItemReceiptItemId = table.Column<Guid>(nullable: true),
                    OrderItemId = table.Column<Guid>(nullable: true),
                    ItemId = table.Column<Guid>(nullable: true),
                    TaxId = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    IsItemReceipt = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBillItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpBills_BillId",
                        column: x => x.BillId,
                        principalTable: "CarlErpBills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "CarlErpItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpItemReceiptItems_ItemReceiptItemId",
                        column: x => x.ItemReceiptItemId,
                        principalTable: "CarlErpItemReceiptItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpPurchaseOrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "CarlErpPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBillItems_CarlErpTaxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "CarlErpTaxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpJournalItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    JournalId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(19,6)", nullable: false),
                    Key = table.Column<int>(nullable: false),
                    Identifier = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpJournalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_CarlErpChartOfAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpJournalItems_CarlErpJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "CarlErpJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountCycles_StartDate_EndDate",
                table: "CarlErpAccountCycles",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAccountTypes_CreatorUserId",
                table: "CarlErpAccountTypes",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_BillId",
                table: "CarlErpBillItems",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_ItemId",
                table: "CarlErpBillItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_ItemReceiptItemId",
                table: "CarlErpBillItems",
                column: "ItemReceiptItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_OrderItemId",
                table: "CarlErpBillItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TaxId",
                table: "CarlErpBillItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBillItems_TenantId_CreatorUserId",
                table: "CarlErpBillItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_ItemReceiptId",
                table: "CarlErpBills",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_LocationId",
                table: "CarlErpBills",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_VendorId",
                table: "CarlErpBills",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpBills",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_AccountTypeId",
                table: "CarlErpChartOfAccounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_ParentAccountId",
                table: "CarlErpChartOfAccounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TaxId",
                table: "CarlErpChartOfAccounts",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpChartOfAccounts_TenantId_CreatorUserId",
                table: "CarlErpChartOfAccounts",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_ParentClassId",
                table: "CarlErpClasses",
                column: "ParentClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpClasses_TenantId_CreatorUserId_ClassName",
                table: "CarlErpClasses",
                columns: new[] { "TenantId", "CreatorUserId", "ClassName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpContactPersons_VenderId",
                table: "CarlErpContactPersons",
                column: "VenderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpContactPersons_TenantId_CreatorUserId",
                table: "CarlErpContactPersons",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCurrencies_CreatorUserId",
                table: "CarlErpCurrencies",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_CustomerCreditId",
                table: "CarlErpCusotmerCreditDetails",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_ItemId",
                table: "CarlErpCusotmerCreditDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_TaxId",
                table: "CarlErpCusotmerCreditDetails",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCusotmerCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpCusotmerCreditDetails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerContactPersons_CustomerId",
                table: "CarlErpCustomerContactPersons",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerContactPersons_TenantId_CreatorUserId",
                table: "CarlErpCustomerContactPersons",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_CustomerId",
                table: "CarlErpCustomerCredits",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_LocationId",
                table: "CarlErpCustomerCredits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpCustomerCredits",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_AccountId",
                table: "CarlErpCustomers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomers_TenantId_CreatorUserId_CustomerName_CustomerCode",
                table: "CarlErpCustomers",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerName", "CustomerCode" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpFormats_Name_Key",
                table: "CarlErpFormats",
                columns: new[] { "Name", "Key" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_InvoiceId",
                table: "CarlErpInvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_ItemId",
                table: "CarlErpInvoiceItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_ItemIssueItemId",
                table: "CarlErpInvoiceItems",
                column: "ItemIssueItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_OrderItemId",
                table: "CarlErpInvoiceItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TaxId",
                table: "CarlErpInvoiceItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoiceItems_TenantId_CreatorUserId",
                table: "CarlErpInvoiceItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_CustomerId",
                table: "CarlErpInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_ItemIssueId",
                table: "CarlErpInvoices",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_LocationId",
                table: "CarlErpInvoices",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpInvoices",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_ItemId",
                table: "CarlErpItemIssueItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_ItemIssueId",
                table: "CarlErpItemIssueItems",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_SaleOrderItemId",
                table: "CarlErpItemIssueItems",
                column: "SaleOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_TransferOrderItemId",
                table: "CarlErpItemIssueItems",
                column: "TransferOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueItems_TenantId_CreatorUserId",
                table: "CarlErpItemIssueItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_CustomerId",
                table: "CarlErpItemIssues",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_LocationId",
                table: "CarlErpItemIssues",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TransferOrderId",
                table: "CarlErpItemIssues",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemIssues",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_LocationId",
                table: "CarlErpItemIssueVendorCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_VendorCreditId",
                table: "CarlErpItemIssueVendorCredit",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_VendorId",
                table: "CarlErpItemIssueVendorCredit",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemIssueVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_ItemIssueVendorCreditId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_VendorCreditItemId",
                table: "CarlErpItemIssueVendorCreditItem",
                column: "VendorCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCreditItem_TenantId_CreatorUserId",
                table: "CarlErpItemIssueVendorCreditItem",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_ItemId",
                table: "CarlErpItemProperties",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_PropertyId",
                table: "CarlErpItemProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_PropertyValueId",
                table: "CarlErpItemProperties",
                column: "PropertyValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemProperties_CreatorUserId_TenantId",
                table: "CarlErpItemProperties",
                columns: new[] { "CreatorUserId", "TenantId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_CustomerCreditId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_CustomerId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_CustomerCreditItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "CustomerCreditItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_ItemReceiptCustomerCreditId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                column: "ItemReceiptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCreditItem_TenantId_CreatorUserId",
                table: "CarlErpItemReceiptCustomerCreditItem",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_ItemId",
                table: "CarlErpItemReceiptItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_ItemReceiptId",
                table: "CarlErpItemReceiptItems",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_OrderItemId",
                table: "CarlErpItemReceiptItems",
                column: "OrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_TransferOrderItemId",
                table: "CarlErpItemReceiptItems",
                column: "TransferOrderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptItems_TenantId_CreatorUserId",
                table: "CarlErpItemReceiptItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_LocationId",
                table: "CarlErpItemReceipts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TransferOrderId",
                table: "CarlErpItemReceipts",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_VendorId",
                table: "CarlErpItemReceipts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemReceipts",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_InventoryAccountId",
                table: "CarlErpItems",
                column: "InventoryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_ItemTypeId",
                table: "CarlErpItems",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseAccountId",
                table: "CarlErpItems",
                column: "PurchaseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseCurrencyId",
                table: "CarlErpItems",
                column: "PurchaseCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_PurchaseTaxId",
                table: "CarlErpItems",
                column: "PurchaseTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleAccountId",
                table: "CarlErpItems",
                column: "SaleAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleCurrenyId",
                table: "CarlErpItems",
                column: "SaleCurrenyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_SaleTaxId",
                table: "CarlErpItems",
                column: "SaleTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItems_TenantId_CreatorUserId",
                table: "CarlErpItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemTypes_CreatorUserId",
                table: "CarlErpItemTypes",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_AccountId",
                table: "CarlErpJournalItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_JournalId",
                table: "CarlErpJournalItems",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalItems_TenantId_CreatorUserId_Key_Identifier",
                table: "CarlErpJournalItems",
                columns: new[] { "TenantId", "CreatorUserId", "Key", "Identifier" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_BillId",
                table: "CarlErpJournals",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ClassId",
                table: "CarlErpJournals",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CurrencyId",
                table: "CarlErpJournals",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_CustomerCreditId",
                table: "CarlErpJournals",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_GeneralJournalId",
                table: "CarlErpJournals",
                column: "GeneralJournalId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_InvoiceId",
                table: "CarlErpJournals",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemIssueId",
                table: "CarlErpJournals",
                column: "ItemIssueId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemIssueVendorCreditId",
                table: "CarlErpJournals",
                column: "ItemIssueVendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemReceiptCustomerCreditId",
                table: "CarlErpJournals",
                column: "ItemReceiptCustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ItemReceiptId",
                table: "CarlErpJournals",
                column: "ItemReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_PayBillId",
                table: "CarlErpJournals",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_ReceivePaymentId",
                table: "CarlErpJournals",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_VendorCreditId",
                table: "CarlErpJournals",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_TenantId_CreatorUserId_JournalNo",
                table: "CarlErpJournals",
                columns: new[] { "TenantId", "CreatorUserId", "JournalNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_ParentLocationId",
                table: "CarlErpLocations",
                column: "ParentLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpLocations_TenantId_CreatorUserId_LocationName",
                table: "CarlErpLocations",
                columns: new[] { "TenantId", "CreatorUserId", "LocationName" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_BillId",
                table: "CarlErpPayBillDeail",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_PayBillId",
                table: "CarlErpPayBillDeail",
                column: "PayBillId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_VendorId",
                table: "CarlErpPayBillDeail",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBillDeail_TenantId_CreatorUserId",
                table: "CarlErpPayBillDeail",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_VendorCreditId",
                table: "CarlErpPayBills",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_TenantId_CreatorUserId",
                table: "CarlErpPayBills",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpProperties_TenantId_CreatorUserId",
                table: "CarlErpProperties",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPropertyValues_PropertyId",
                table: "CarlErpPropertyValues",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPropertyValues_TenantId_CreatorUserId",
                table: "CarlErpPropertyValues",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_ItemId",
                table: "CarlErpPurchaseOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_PurchaseOrderId",
                table: "CarlErpPurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_TaxId",
                table: "CarlErpPurchaseOrderItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrderItems_TenantId_CreatorUserId",
                table: "CarlErpPurchaseOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_CurrencyId",
                table: "CarlErpPurchaseOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_VendorId",
                table: "CarlErpPurchaseOrders",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPurchaseOrders_TenantId_CreatorUserId_OrderNumber",
                table: "CarlErpPurchaseOrders",
                columns: new[] { "TenantId", "CreatorUserId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_CustomerId",
                table: "CarlErpReceivePaymentDeails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_InvoiceId",
                table: "CarlErpReceivePaymentDeails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_ReceivePaymentId",
                table: "CarlErpReceivePaymentDeails",
                column: "ReceivePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePaymentDeails_TenantId_CreatorUserId",
                table: "CarlErpReceivePaymentDeails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_CustomerCreditId",
                table: "CarlErpReceivePayments",
                column: "CustomerCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_TenantId_CreatorUserId",
                table: "CarlErpReceivePayments",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_ItemId",
                table: "CarlErpSaleOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_SaleOrderId",
                table: "CarlErpSaleOrderItems",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_TaxId",
                table: "CarlErpSaleOrderItems",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrderItems_TenantId_CreatorUserId",
                table: "CarlErpSaleOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CurrencyId",
                table: "CarlErpSaleOrders",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_CustomerId",
                table: "CarlErpSaleOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSaleOrders_TenantId_CreatorUserId_OrderNumber",
                table: "CarlErpSaleOrders",
                columns: new[] { "TenantId", "CreatorUserId", "OrderNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubItems_ItemId",
                table: "CarlErpSubItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpSubItems_ParentSubItemId",
                table: "CarlErpSubItems",
                column: "ParentSubItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTaxes_TenantId_CreatorUserId",
                table: "CarlErpTaxes",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_ItemId",
                table: "CarlErpTransferOrderItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_TransferOrderId",
                table: "CarlErpTransferOrderItems",
                column: "TransferOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrderItems_TenantId_CreatorUserId",
                table: "CarlErpTransferOrderItems",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferFromClassId",
                table: "CarlErpTransferOrders",
                column: "TransferFromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferFromLocationId",
                table: "CarlErpTransferOrders",
                column: "TransferFromLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferToClassId",
                table: "CarlErpTransferOrders",
                column: "TransferToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TransferToLocationId",
                table: "CarlErpTransferOrders",
                column: "TransferToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransferOrders_TenantId_CreatorUserId_TransferNo",
                table: "CarlErpTransferOrders",
                columns: new[] { "TenantId", "CreatorUserId", "TransferNo" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_LocationId",
                table: "CarlErpVendorCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_VendorId",
                table: "CarlErpVendorCredit",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_ItemId",
                table: "CarlErpVendorCreditDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_TaxId",
                table: "CarlErpVendorCreditDetails",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_VendorCreditId",
                table: "CarlErpVendorCreditDetails",
                column: "VendorCreditId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCreditDetails_TenantId_CreatorUserId",
                table: "CarlErpVendorCreditDetails",
                columns: new[] { "TenantId", "CreatorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_AccountId",
                table: "CarlErpVendors",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendors_TenantId_CreatorUserId_VendorName_VendorCode",
                table: "CarlErpVendors",
                columns: new[] { "TenantId", "CreatorUserId", "VendorName", "VendorCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpAccountCycles");

            migrationBuilder.DropTable(
                name: "CarlErpBillItems");

            migrationBuilder.DropTable(
                name: "CarlErpCache");

            migrationBuilder.DropTable(
                name: "CarlErpContactPersons");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerContactPersons");

            migrationBuilder.DropTable(
                name: "CarlErpFormats");

            migrationBuilder.DropTable(
                name: "CarlErpInvoiceItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCreditItem");

            migrationBuilder.DropTable(
                name: "CarlErpItemProperties");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCreditItem");

            migrationBuilder.DropTable(
                name: "CarlErpJournalItems");

            migrationBuilder.DropTable(
                name: "CarlErpPayBillDeail");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePaymentDeails");

            migrationBuilder.DropTable(
                name: "CarlErpSubItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueItems");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCreditDetails");

            migrationBuilder.DropTable(
                name: "CarlErpPropertyValues");

            migrationBuilder.DropTable(
                name: "CarlErpCusotmerCreditDetails");

            migrationBuilder.DropTable(
                name: "CarlErpJournals");

            migrationBuilder.DropTable(
                name: "CarlErpPurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpSaleOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpTransferOrderItems");

            migrationBuilder.DropTable(
                name: "CarlErpProperties");

            migrationBuilder.DropTable(
                name: "CarlErpBills");

            migrationBuilder.DropTable(
                name: "CarlErpInvoices");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropTable(
                name: "CarlErpPayBills");

            migrationBuilder.DropTable(
                name: "CarlErpReceivePayments");

            migrationBuilder.DropTable(
                name: "CarlErpPurchaseOrders");

            migrationBuilder.DropTable(
                name: "CarlErpSaleOrders");

            migrationBuilder.DropTable(
                name: "CarlErpItems");

            migrationBuilder.DropTable(
                name: "CarlErpItemReceipts");

            migrationBuilder.DropTable(
                name: "CarlErpItemIssues");

            migrationBuilder.DropTable(
                name: "CarlErpVendorCredit");

            migrationBuilder.DropTable(
                name: "CarlErpCustomerCredits");

            migrationBuilder.DropTable(
                name: "CarlErpItemTypes");

            migrationBuilder.DropTable(
                name: "CarlErpCurrencies");

            migrationBuilder.DropTable(
                name: "CarlErpTransferOrders");

            migrationBuilder.DropTable(
                name: "CarlErpVendors");

            migrationBuilder.DropTable(
                name: "CarlErpCustomers");

            migrationBuilder.DropTable(
                name: "CarlErpClasses");

            migrationBuilder.DropTable(
                name: "CarlErpLocations");

            migrationBuilder.DropTable(
                name: "CarlErpChartOfAccounts");

            migrationBuilder.DropTable(
                name: "CarlErpAccountTypes");

            migrationBuilder.DropTable(
                name: "CarlErpTaxes");
        }
    }
}
