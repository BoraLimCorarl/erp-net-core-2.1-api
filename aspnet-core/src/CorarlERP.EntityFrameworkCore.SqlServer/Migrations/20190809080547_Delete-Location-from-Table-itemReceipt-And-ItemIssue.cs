using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class DeleteLocationfromTableitemReceiptAndItemIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"
                    --Bill
                    Update j Set j.LocationId = b.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpBills AS b ON j.BillId = b.Id
                    WHERE j.BillId is not null; 
                    go
                    --Vendor Credit 
                    Update j Set j.LocationId = vc.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpVendorCredit AS vc ON j.VendorCreditId = vc.Id
                    WHERE j.VendorCreditId is not null; 
                    go
                    --Item Receipt
                    Update j Set j.LocationId = IR.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpItemReceipts AS IR ON j.ItemReceiptId = IR.Id
                    WHERE j.ItemReceiptId is not null; 
                    go
                    --Item Receipt Customer Credit
                    Update j Set j.LocationId = IRC.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpItemReceiptCustomerCredit AS IRC ON j.ItemReceiptCustomerCreditId = IRC.Id
                    WHERE j.ItemReceiptCustomerCreditId is not null; 
                    go
                    --Item Issue Vendor Credit
                    Update j Set j.LocationId = ISV.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpItemIssueVendorCredit AS ISV ON j.ItemIssueVendorCreditId = ISV.Id
                    WHERE j.ItemIssueVendorCreditId is not null; 
                    go
                    --Item Issue
                    Update j Set j.LocationId = ISS.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpItemIssues AS ISS ON j.ItemIssueId = ISS.Id
                    WHERE j.ItemIssueId is not null; 
                    go
                    --Invoice
                    Update j Set j.LocationId = INV.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpInvoices AS INV ON j.InvoiceId = INV.Id
                    WHERE j.InvoiceId is not null; 
                    go
                    --Customer Credit
                    Update j Set j.LocationId = CC.LocationId
                    From CarlErpJournals AS j
                    Inner join CarlErpCustomerCredits AS CC ON j.CustomerCreditId = CC.Id
                    WHERE j.CustomerCreditId is not null; 
                    go
                    --Pay Bill
                    Update j Set j.LocationId = t.LocationId
                    From CarlErpJournals AS j
                    inner join AbpTenants as t on j.TenantId = t.Id
                    Inner join CarlErpPayBills AS pb ON j.PayBillId = pb.Id
                    WHERE j.PayBillId is not null; 
                    go
                    --Receipt Payment
                    Update j Set j.LocationId = t.LocationId
                    From CarlErpJournals AS j
                    inner join AbpTenants as t on j.TenantId = t.Id
                    Inner join CarlErpReceivePayments AS rp ON j.ReceivePaymentId = rp.Id
                    WHERE j.ReceivePaymentId is not null; 
                    go
                    --Withdraw
                    Update j Set j.LocationId = t.LocationId
                    From CarlErpJournals AS j
                    inner join AbpTenants as t on j.TenantId = t.Id
                    Inner join CarlErpWithdraws AS dp ON j.WithdrawId = dp.Id
                    WHERE j.WithdrawId is not null; 
                    go
                    --Deposit
                    Update j Set j.LocationId = t.LocationId
                    From CarlErpJournals AS j
                    inner join AbpTenants as t on j.TenantId = t.Id
                    Inner join CarlErpDeposits AS dp ON j.DepositId = dp.Id
                    WHERE j.DepositId is not null; 
                    go
                    --Bank Transfer
                    Update bt Set bt.FromLocationId = t.LocationId, bt.ToLocationId = t.LocationId
                    From CarlErpBankTransfers AS bt
                    inner join AbpTenants as t on bt.TenantId = t.Id
                    WHERE t.LocationId is not null; 
                    go
                ");
            }

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpBills_CarlErpLocations_LocationId",
                table: "CarlErpBills");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpLocations_LocationId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpDeposits_CarlErpLocations_LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpInvoices_CarlErpLocations_LocationId",
                table: "CarlErpInvoices");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpLocations_LocationId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpLocations_LocationId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpLocations_LocationId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpLocations_LocationId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPayBills_CarlErpLocations_LocationId",
                table: "CarlErpPayBills");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpLocations_LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpLocations_LocationId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpLocations_LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpWithdraws_LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCredit_LocationId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpReceivePayments_LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPayBills_LocationId",
                table: "CarlErpPayBills");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_LocationId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_LocationId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_LocationId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_LocationId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoices_LocationId",
                table: "CarlErpInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpDeposits_LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomerCredits_LocationId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBills_LocationId",
                table: "CarlErpBills");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpBills");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpWithdraws");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpDeposits");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "CarlErpBills");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemReceipts",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpItemReceiptCustomerCredit",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemIssueVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpItemIssues",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpInvoices",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpCustomerCredits",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId",
                table: "CarlErpBills",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemReceipts");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpItemReceiptCustomerCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId",
                table: "CarlErpItemIssueVendorCredit");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpItemIssues");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpInvoices");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId",
                table: "CarlErpBills");

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpWithdraws",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpVendorCredit",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpReceivePayments",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpPayBills",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpItemReceipts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpItemIssueVendorCredit",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpItemIssues",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpInvoices",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpDeposits",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpCustomerCredits",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LocationId",
                table: "CarlErpBills",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpWithdraws_LocationId",
                table: "CarlErpWithdraws",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_LocationId",
                table: "CarlErpVendorCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_LocationId",
                table: "CarlErpReceivePayments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_LocationId",
                table: "CarlErpPayBills",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_LocationId",
                table: "CarlErpItemReceipts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceipts_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemReceipts",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemReceiptCustomerCredit_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_LocationId",
                table: "CarlErpItemIssueVendorCredit",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssueVendorCredit_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpItemIssueVendorCredit",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_LocationId",
                table: "CarlErpItemIssues",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpItemIssues_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpItemIssues",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_LocationId",
                table: "CarlErpInvoices",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpInvoices_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpInvoices",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpDeposits_LocationId",
                table: "CarlErpDeposits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_LocationId",
                table: "CarlErpCustomerCredits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpCustomerCredits_TenantId_CreatorUserId_CustomerId_LocationId",
                table: "CarlErpCustomerCredits",
                columns: new[] { "TenantId", "CreatorUserId", "CustomerId", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_LocationId",
                table: "CarlErpBills",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpBills_TenantId_CreatorUserId_VendorId_LocationId",
                table: "CarlErpBills",
                columns: new[] { "TenantId", "CreatorUserId", "VendorId", "LocationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpBills_CarlErpLocations_LocationId",
                table: "CarlErpBills",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpCustomerCredits_CarlErpLocations_LocationId",
                table: "CarlErpCustomerCredits",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpDeposits_CarlErpLocations_LocationId",
                table: "CarlErpDeposits",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpInvoices_CarlErpLocations_LocationId",
                table: "CarlErpInvoices",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssues_CarlErpLocations_LocationId",
                table: "CarlErpItemIssues",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemIssueVendorCredit_CarlErpLocations_LocationId",
                table: "CarlErpItemIssueVendorCredit",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceiptCustomerCredit_CarlErpLocations_LocationId",
                table: "CarlErpItemReceiptCustomerCredit",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpItemReceipts_CarlErpLocations_LocationId",
                table: "CarlErpItemReceipts",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPayBills_CarlErpLocations_LocationId",
                table: "CarlErpPayBills",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpLocations_LocationId",
                table: "CarlErpReceivePayments",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpVendorCredit_CarlErpLocations_LocationId",
                table: "CarlErpVendorCredit",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpWithdraws_CarlErpLocations_LocationId",
                table: "CarlErpWithdraws",
                column: "LocationId",
                principalTable: "CarlErpLocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
