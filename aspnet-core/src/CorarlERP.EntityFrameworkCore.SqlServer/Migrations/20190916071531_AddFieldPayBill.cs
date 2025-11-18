using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class AddFieldPayBill : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @" 
                    alter table CarlErpPayBillDeail add myColumn varchar(200)
                    go

                    --Insert into Paybill items 
                    insert into CarlErpPayBillDeail(Id, myColumn, CreationTime, CreatorUserId, TenantId, PayBillId, VendorCreditId, VendorId, DueDate, OpenBalance, Payment, TotalAmount, MultiCurrencyOpenBalance, MultiCurrencyPayment, MultiCurrencyTotalAmount)
                    select
                    NEWID(), 
                    'migration_data',
                    getdate(), 
                    pb.CreatorUserId, 
                    pb.TenantId, 
                    pb.Id PayBillId, 
                    pb.VendorCreditId, 
                    vc.VendorId, 
                    vc.DueDate, 
                    vc.Total - 
                    (
                    select case when sum(p.TotalPayment) is null then 0 else sum(p.TotalPayment) end from CarlErpPayBills p 
                    join CarlErpJournals jj on jj.PayBillId = p.Id
                    where p.VendorCreditId is not null and p.VendorCreditId = vc.Id and (jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime < j.CreationTime))
					), 
                    pb.TotalPayment, 
                    vc.Total -
                    (
                    select case when sum(p.TotalPayment) is null then 0 else sum(p.TotalPayment) end from CarlErpPayBills p 
                    join CarlErpJournals jj on jj.PayBillId = p.Id
                    where p.VendorCreditId is not null and  p.VendorCreditId = vc.Id and ( 
						jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime <= j.CreationTime)
					)
                    ), 
                    vc.MultiCurrencyTotal - 
                    (
                    select case when sum(p.MultiCurrencyTotalPayment) is null then 0 else sum(p.MultiCurrencyTotalPayment) end from CarlErpPayBills p 
                    join CarlErpJournals jj on jj.PayBillId = p.Id
                    where p.VendorCreditId is not null and  p.VendorCreditId = vc.Id  and (jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime < j.CreationTime))
                    ), 
                    pb.MultiCurrencyTotalPayment, 
                    vc.MultiCurrencyTotal -
                    (
                    select case when sum(p.MultiCurrencyTotalPayment) is null then 0 else sum(p.MultiCurrencyTotalPayment) end from CarlErpPayBills p 
                    join CarlErpJournals jj on jj.PayBillId = p.Id
                    where p.VendorCreditId is not null and  p.VendorCreditId = vc.Id  and ( 
						jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime <= j.CreationTime)
					)
                    )
                    from CarlErpJournals as j 
                    join CarlErpPayBills as pb on pb.Id = j.PayBillId
                    join CarlErpVendorCredit vc on vc.Id = pb.VendorCreditId
                    where pb.VendorCreditId is not null
                    go

					update ji set ji.Identifier = pbd.Id, ji.[Key] = 2 
					from CarlErpPayBillDeail pbd
					join CarlErpPayBills pb on pb.Id = pbd.PayBillId
					join CarlErpVendorCredit vc on vc.Id = pb.VendorCreditId
					join CarlErpJournals j on j.PayBillId = pbd.PayBillId
					join CarlErpJournalItems ji on ji.JournalId = j.Id 
					where ji.Identifier is null and pb.VendorCreditId is not null and pbd.myColumn = N'migration_data' 
                    go

                    update rp set  
					rp.TotalPaymentVendorCredit = rp.TotalPayment, 
					rp.TotalOpenBalanceVendorCredit = rpd.OpenBalance,
				    rp.TotalPaymentDueVendorCredit = rpd.TotalAmount,
					rp.MultiCurrencyTotalPaymentVendorCredit = rp.MultiCurrencyTotalPayment 
					from CarlErpPayBills rp
					join CarlErpPayBillDeail rpd on rp.Id = rpd.PayBillId and rpd.myColumn = N'migration_data' 
					where ReceiveFrom = 2 
					go

                    alter table CarlErpPayBillDeail drop column myColumn
                    go

					
					-- Update TotalPaymentInvoice = TotalPayment
					update CarlErpPayBills set TotalPaymentBill = TotalPayment, MultiCurrencyTotalPaymentBill = MultiCurrencyTotalPayment
					go

					-- Set TotalPayment = 0 for Cash Payment
					update CarlErpPayBills set TotalPayment = 0, MultiCurrencyTotalPayment = 0  where ReceiveFrom = 2 
                ");
            }


            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpPayBills_CarlErpVendorCredit_VendorCreditId",
                table: "CarlErpPayBills");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpPayBills_VendorCreditId",
                table: "CarlErpPayBills");

            migrationBuilder.DropColumn(
                name: "VendorCreditId",
                table: "CarlErpPayBills");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VendorCreditId",
                table: "CarlErpPayBills",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpPayBills_VendorCreditId",
                table: "CarlErpPayBills",
                column: "VendorCreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpPayBills_CarlErpVendorCredit_VendorCreditId",
                table: "CarlErpPayBills",
                column: "VendorCreditId",
                principalTable: "CarlErpVendorCredit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
