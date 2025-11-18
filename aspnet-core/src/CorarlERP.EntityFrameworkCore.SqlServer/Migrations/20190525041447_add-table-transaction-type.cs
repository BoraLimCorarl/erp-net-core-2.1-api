using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class addtabletransactiontype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TransactionTypeSaleId",
                table: "CarlErpItemIssues",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TransactionTypeSaleId",
                table: "CarlErpInvoices",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarlErpTransactionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    TransactionTypeName = table.Column<string>(maxLength: 512, nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpTransactionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TransactionTypeSaleId",
                table: "CarlErpItemIssues",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TransactionTypeSaleId",
                table: "CarlErpInvoices",
                column: "TransactionTypeSaleId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpTransactionTypes_TenantId_CreatorUserId_TransactionTypeName",
                table: "CarlErpTransactionTypes",
                columns: new[] { "TenantId", "CreatorUserId", "TransactionTypeName" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInvoices_CarlErpTransactionTypes_TransactionTypeSaleId",
                table: "CarlErpInvoices",
                column: "TransactionTypeSaleId",
                principalTable: "CarlErpTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpTransactionTypes_TransactionTypeSaleId",
                table: "CarlErpItemIssues",
                column: "TransactionTypeSaleId",
                principalTable: "CarlErpTransactionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInvoices_CarlErpTransactionTypes_TransactionTypeSaleId",
                table: "CarlErpInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpTransactionTypes_TransactionTypeSaleId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropTable(
                name: "CarlErpTransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_TransactionTypeSaleId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoices_TransactionTypeSaleId",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "TransactionTypeSaleId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "TransactionTypeSaleId",
                table: "CarlErpInvoices");
        }
    }
}
