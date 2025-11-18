using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace CorarlERP.Migrations
{
    public partial class AddMigrationTransactionTypeAndItemFumular : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "CarlErpPropertyValues",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JournalTransactionTypeId",
                table: "CarlErpJournals",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BarcodeSameAsItemCode",
                table: "CarlErpItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ItemCodeSetting",
                table: "AbpTenants",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CarlErpItemCodeFormulas",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemCodeFormulas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpJournalTransactionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsIssue = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    InventoryTransactionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpJournalTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemCodeFormulaItemTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    ItemCodeFormulaId = table.Column<long>(nullable: false),
                    ItemTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemCodeFormulaItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemCodeFormulaItemTypes_CarlErpItemCodeFormulas_Ite~",
                        column: x => x.ItemCodeFormulaId,
                        principalTable: "CarlErpItemCodeFormulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemCodeFormulaItemTypes_CarlErpItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "CarlErpItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarlErpItemCodeFormulaProperties",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    PropertyId = table.Column<long>(nullable: false),
                    ItemCodeFormulaId = table.Column<long>(nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    Separator = table.Column<string>(maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpItemCodeFormulaProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpItemCodeFormulaProperties_CarlErpItemCodeFormulas_It~",
                        column: x => x.ItemCodeFormulaId,
                        principalTable: "CarlErpItemCodeFormulas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpItemCodeFormulaProperties_CarlErpProperties_Property~",
                        column: x => x.PropertyId,
                        principalTable: "CarlErpProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournals_JournalTransactionTypeId",
                table: "CarlErpJournals",
                column: "JournalTransactionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaItemTypes_ItemCodeFormulaId",
                table: "CarlErpItemCodeFormulaItemTypes",
                column: "ItemCodeFormulaId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaItemTypes_ItemTypeId",
                table: "CarlErpItemCodeFormulaItemTypes",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaProperties_ItemCodeFormulaId",
                table: "CarlErpItemCodeFormulaProperties",
                column: "ItemCodeFormulaId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulaProperties_PropertyId",
                table: "CarlErpItemCodeFormulaProperties",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemCodeFormulas_Name",
                table: "CarlErpItemCodeFormulas",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalTransactionTypes_InventoryTransactionType",
                table: "CarlErpJournalTransactionTypes",
                column: "InventoryTransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpJournalTransactionTypes_Name",
                table: "CarlErpJournalTransactionTypes",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpJournals_CarlErpJournalTransactionTypes_JournalTrans~",
                table: "CarlErpJournals",
                column: "JournalTransactionTypeId",
                principalTable: "CarlErpJournalTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"INSERT INTO  ""CarlErpJournalTransactionTypes""(""Id"" ,""CreationTime"" ,""CreatorUserId"" ,""Name"",""IsIssue"",""IsDefault"",""Active"",""InventoryTransactionType"",""TenantId"")
                                SELECT gen_random_uuid(), NOW(), t2.""CreatorUserId"", t1.IndxName, t1.IdxIssue, '1', '1', t1.Idx, t2.""Id"" FROM(
                                      select 1  Idx, 'Purchase' IndxName, false IdxIssue
                                union select 2  Idx, 'Sale Return' IndxName, false IdxIssue
                                union select 3  Idx, 'Transfer' IndxName, false IdxIssue
                                union select 4  Idx, 'Adjustment' IndxName, false IdxIssue
                                union select 5  Idx, 'Other' IndxName, false IdxIssue
                                union select 7  Idx, 'Sale' IndxName, true IdxIssue
                                union select 8  Idx, 'Purchase Return' IndxName, true IdxIssue
                                union select 9  Idx, 'Transfer' IndxName, true IdxIssue
                                union select 10 Idx, 'Adjustment' IndxName, true IdxIssue
                                union select 11 Idx, 'Other' IndxName, true IdxIssue
                                union select 13 Idx, 'Physical Count' IndxName, true IdxIssue
                                union select 14 Idx, 'Physical Count' IndxName, false IdxIssue
                                union select 15 Idx, 'Production' IndxName, false IdxIssue
                                union select 16 Idx, 'Production' IndxName, true IdxIssue
                                ) as t1
                                full join ""AbpTenants"" as t2
                                on 1 = 1
                                where t2.""IsDeleted"" = false");

            migrationBuilder.Sql(@"UPDATE ""CarlErpJournals""
                                SET    ""JournalTransactionTypeId"" = t2.""Id""
                                 from(
                                      select 1  Idx, 'Purchase' IndxName, 0 IdxIssue, 3 journalType
                                union select 2  Idx, 'Sale Return' IndxName, 0 IdxIssue, 13 journalType
                                union select 3  Idx, 'Transfer' IndxName, 0 IdxIssue, 10 journalType
                                union select 4  Idx, 'Adjustment' IndxName, 0 IdxIssue, 11 journalType
                                union select 5  Idx, 'Other' IndxName, 0 IdxIssue, 12 journalType
                                union select 7  Idx, 'Sale' IndxName, 1 IdxIssue, 7 journalType
                                union select 8  Idx, 'Purchase Return' IndxName, 1 IdxIssue, 17 journalType
                                union select 9  Idx, 'Transfer' IndxName, 1 IdxIssue, 14 journalType
                                union select 10 Idx, 'Adjustment' IndxName, 1 IdxIssue, 15 journalType
                                union select 11 Idx, 'Other' IndxName, 1 IdxIssue, 16 journalType
                                union select 13 Idx, 'Physical Count' IndxName, 1 IdxIssue, 18 journalType
                                union select 14 Idx, 'Physical Count' IndxName, 0 IdxIssue, 19 journalType
                                union select 15 Idx, 'Production' IndxName, 0 IdxIssue, 23 journalType
                                union select 16 Idx, 'Production' IndxName, 1 IdxIssue, 22 journalType
                                ) t1
                                 join ""CarlErpJournalTransactionTypes"" as t2
                                on t1.Idx = t2.""InventoryTransactionType""
                                WHERE  ""CarlErpJournals"".""JournalType"" = t1.journalType and ""CarlErpJournals"".""TenantId"" = t2.""TenantId""");

            migrationBuilder.Sql(@"update ""AbpTenants"" set ""ItemCodeSetting"" = (case when ""AutoItemCode"" = true then 1 else 0 end)");

            migrationBuilder.DropColumn(
               name: "AutoItemCode",
               table: "AbpTenants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpJournals_CarlErpJournalTransactionTypes_JournalTrans~",
                table: "CarlErpJournals");

            migrationBuilder.DropTable(
                name: "CarlErpItemCodeFormulaItemTypes");

            migrationBuilder.DropTable(
                name: "CarlErpItemCodeFormulaProperties");

            migrationBuilder.DropTable(
                name: "CarlErpJournalTransactionTypes");

            migrationBuilder.DropTable(
                name: "CarlErpItemCodeFormulas");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpJournals_JournalTransactionTypeId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "CarlErpPropertyValues");

            migrationBuilder.DropColumn(
                name: "JournalTransactionTypeId",
                table: "CarlErpJournals");

            migrationBuilder.DropColumn(
                name: "BarcodeSameAsItemCode",
                table: "CarlErpItems");

            migrationBuilder.DropColumn(
                name: "ItemCodeSetting",
                table: "AbpTenants");

            migrationBuilder.AddColumn<bool>(
                name: "AutoItemCode",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);
        }
    }
}
