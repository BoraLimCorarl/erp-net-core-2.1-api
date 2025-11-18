using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Update_Tenant_Relationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpVendorCredit",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPaid",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpInvoices",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpCustomerCredits",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPaid",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpBills",
                type: "decimal(19,6)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoSequence",
                table: "AbpTenants",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CarlErpAutoSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    DocumentType = table.Column<int>(nullable: false),
                    DefaultPrefix = table.Column<string>(nullable: true),
                    SymbolFormat = table.Column<string>(nullable: true),
                    NumberFormat = table.Column<string>(nullable: true),
                    CustomFormat = table.Column<bool>(nullable: false),
                    YearFormat = table.Column<int>(nullable: true),
                    LastAutoSequenceNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpAutoSequences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpAutoSequences_DocumentType_NumberFormat_YearFormat_DefaultPrefix",
                table: "CarlErpAutoSequences",
                columns: new[] { "DocumentType", "NumberFormat", "YearFormat", "DefaultPrefix" });


            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                // do something SQL Server - specific
                migrationBuilder.Sql(@" INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId,  TenantId) 
                                        SELECT NewId(), 1, 'PO', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 2, 'IR', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 3, 'IRCC', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 4, 'Bill', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 5, 'VC', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 6, 'PB', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 7, 'SO', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 8, 'INV', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 9, 'CC', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 10, 'IIS', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 11, 'IIVC', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 12, 'RP', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 14, 'BTO', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 15, 'DPS', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 16, 'PDO', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 17, 'TO', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a

                                        INSERT INTO CarlErpAutoSequences (	Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat,
									                                        YearFormat, CreationTime, CreatorUserId, TenantId) 
                                        SELECT NewId(), 18, 'PC', '-', '0001', 1, 2, GETUTCDATE(), NULL, a.Id FROM AbpTenants a");

            }

            //migrationBuilder.Sql("DECLARE " +
            //    "@creatorUserId BIGINT, " +
            //    "@tenantId int; " +
            //    "DECLARE tempCursor CURSOR FOR " +
            //    "SELECT Id, TenantId FROM AbpUsers WHERE TenantId IN(SELECT Id FROM AbpTenants WHERE IsDeleted = 0) GROUP BY TenantId, Id " +
            //    "" +
            //    "OPEN tempCursor " +
            //    "FETCH NEXT FROM tempCursor INTO @creatorUserId, @tenantId " +
            //    "WHILE @@FETCH_STATUS = 0 " +
            //    "BEGIN " +
            //        "INSERT INTO CarlErpAutoSequences" +
            //                "(Id, DocumentType, DefaultPrefix, SymbolFormat, NumberFormat, CustomFormat, YearFormat, CreatorUserId, CreationTime, TenantId) " +
            //        "VALUES " +
            //        "" +
            //                "(NewId(), 1, 'PO', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'IR', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'IRCC', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'Bill', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'VC', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'PB', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'SO', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'INV', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'CC', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'IIS', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'IIVC', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'RP', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'BTO', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'DPS', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'WD', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'PDO', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'TO', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "(NewId(), 1, 'PC', '-', '0001', 1, 2, GETUTCDATE(), @creatorUserId, @tenantId), " +
            //                "FETCH NEXT FROM tempCursor INTO @creatorUserId, @tenantId " +
            //    "END " +
            //    "CLOSE tempCursor; " +
            //    "DEALLOCATE tempCursor; ");




        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpAutoSequences");

            migrationBuilder.DropColumn(
                name: "IsAutoSequence",
                table: "AbpTenants");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpVendorCredit",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPaid",
                table: "CarlErpInvoices",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpInvoices",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpCustomerCredits",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPaid",
                table: "CarlErpBills",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OpenBalance",
                table: "CarlErpBills",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,6)");
        }
    }
}
