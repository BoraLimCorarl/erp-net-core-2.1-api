using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class Add_Table_Bank_Transfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "VendorId",
                table: "CarlErpWithdraws",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiveFromVendorId",
                table: "CarlErpDeposits",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.CreateTable(
                name: "CarlErpBankTransfers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    BankTransferNo = table.Column<string>(maxLength: 128, nullable: false),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    BankTransferDate = table.Column<DateTime>(nullable: false),
                    TransferToClassId = table.Column<long>(nullable: false),
                    TransferFromClassId = table.Column<long>(nullable: false),
                    Memo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    BankTransferToAccountId = table.Column<Guid>(nullable: false),
                    BankTransferFromAccountId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(19,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarlErpBankTransfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpChartOfAccounts_BankTransferFromAccountId",
                        column: x => x.BankTransferFromAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpChartOfAccounts_BankTransferToAccountId",
                        column: x => x.BankTransferToAccountId,
                        principalTable: "CarlErpChartOfAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpClasses_TransferFromClassId",
                        column: x => x.TransferFromClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarlErpBankTransfers_CarlErpClasses_TransferToClassId",
                        column: x => x.TransferToClassId,
                        principalTable: "CarlErpClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_BankTransferFromAccountId",
                table: "CarlErpBankTransfers",
                column: "BankTransferFromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_BankTransferToAccountId",
                table: "CarlErpBankTransfers",
                column: "BankTransferToAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TransferFromClassId",
                table: "CarlErpBankTransfers",
                column: "TransferFromClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TransferToClassId",
                table: "CarlErpBankTransfers",
                column: "TransferToClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBankTransfers_TenantId_CreatorUserId_Amount_BankTransferNo",
                table: "CarlErpBankTransfers",
                columns: new[] { "TenantId", "CreatorUserId", "Amount", "BankTransferNo" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarlErpBankTransfers");

            migrationBuilder.AlterColumn<Guid>(
                name: "VendorId",
                table: "CarlErpWithdraws",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiveFromVendorId",
                table: "CarlErpDeposits",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
