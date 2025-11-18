using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateReceivePayment_Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @" 
                    alter table CarlErpReceivePaymentDeails add myColumn varchar(200)
                    go

                    --Insert into Receive Payment items 
                    insert into CarlErpReceivePaymentDeails(Id, myColumn, CreationTime, CreatorUserId, TenantId, ReceivePaymentId, CustomerCreditId, CustomerId, DueDate, OpenBalance, Payment, TotalAmount, MultiCurrencyOpenBalance, MultiCurrencyPayment, MultiCurrencyTotalAmount)
                    select
                    NEWID(), 
                    'migration_data',
                    getdate(), 
                    pb.CreatorUserId, 
                    pb.TenantId, 
                    pb.Id ReceivePaymentId, 
                    pb.CustomerCreditId, 
                    vc.CustomerId, 
                    vc.DueDate, 
                    vc.Total - 
                    (
                    select case when sum(p.TotalPayment) is null then 0 else sum(p.TotalPayment) end from CarlErpReceivePayments p 
                    join CarlErpJournals jj on jj.ReceivePaymentId = p.Id
                    where p.CustomerCreditId is not null and p.CustomerCreditId = vc.Id and (jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime < j.CreationTime))
                    ), 
                    pb.TotalPayment, 
                    vc.Total -
                    (
                    select case when sum(p.TotalPayment) is null then 0 else sum(p.TotalPayment) end from CarlErpReceivePayments p 
                    join CarlErpJournals jj on jj.ReceivePaymentId = p.Id
                    where p.CustomerCreditId is not null and  p.CustomerCreditId = vc.Id and ( 
	                    jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime <= j.CreationTime)
                    )
                    ), 
                    vc.MultiCurrencyTotal - 
                    (
                    select case when sum(p.MultiCurrencyTotalPayment) is null then 0 else sum(p.MultiCurrencyTotalPayment) end from CarlErpReceivePayments p 
                    join CarlErpJournals jj on jj.ReceivePaymentId = p.Id
                    where p.CustomerCreditId is not null and  p.CustomerCreditId = vc.Id  and (jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime < j.CreationTime))
                    ), 
                    pb.MultiCurrencyTotalPayment, 
                    vc.MultiCurrencyTotal -
                    (
                    select case when sum(p.MultiCurrencyTotalPayment) is null then 0 else sum(p.MultiCurrencyTotalPayment) end from CarlErpReceivePayments p 
                    join CarlErpJournals jj on jj.ReceivePaymentId = p.Id
                    where p.CustomerCreditId is not null and  p.CustomerCreditId = vc.Id  and ( 
	                    jj.Date < j.Date or (jj.Date = j.Date and jj.CreationTime <= j.CreationTime)
                    )
                    )
                    from CarlErpJournals as j 
                    join CarlErpReceivePayments as pb on pb.Id = j.ReceivePaymentId
                    join CarlErpCustomerCredits vc on vc.Id = pb.CustomerCreditId
                    where pb.CustomerCreditId is not null
                    go

                    update ji set ji.Identifier = pbd.Id, ji.[Key] = 2 
                    from CarlErpReceivePaymentDeails pbd
                    join CarlErpReceivePayments pb on pb.Id = pbd.ReceivePaymentId
                    join CarlErpCustomerCredits vc on vc.Id = pb.CustomerCreditId
                    join CarlErpJournals j on j.ReceivePaymentId = pbd.ReceivePaymentId
                    join CarlErpJournalItems ji on ji.JournalId = j.Id 
                    where ji.Identifier is null and pb.CustomerCreditId is not null and pbd.myColumn = N'migration_data' 


					update rp set  
					rp.TotalPaymentCustomerCredit = rp.TotalPayment, 
					rp.TotalOpenBalanceCustomerCredit = rpd.OpenBalance,
				    rp.TotalPaymentDueCustomerCredit = rpd.TotalAmount,
					rp.MultiCurrencyTotalPaymentCustomerCredit = rp.MultiCurrencyTotalPayment 
					from CarlErpReceivePayments rp
					join CarlErpReceivePaymentDeails rpd on rp.Id = rpd.ReceivePaymentId and rpd.myColumn = N'migration_data' 
					where ReceiveFrom = 2 
					

                    alter table CarlErpReceivePaymentDeails drop column myColumn
                    go

					
					-- Update TotalPaymentInvoice = TotalPayment
					update CarlErpReceivePayments set TotalPaymentInvoice = TotalPayment, MultiCurrencyTotalPaymentInvoice = MultiCurrencyTotalPayment
					go

					-- Set TotalPayment = 0 for Cash Payment
					update CarlErpReceivePayments set TotalPayment = 0, MultiCurrencyTotalPayment = 0  where ReceiveFrom = 2 

                ");
            }

            migrationBuilder.DropForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropIndex(
                name: "IX_CarlErpReceivePayments_CustomerCreditId",
                table: "CarlErpReceivePayments");

            migrationBuilder.DropColumn(
                name: "CustomerCreditId",
                table: "CarlErpReceivePayments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CustomerCreditId",
                table: "CarlErpReceivePayments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarlErpReceivePayments_CustomerCreditId",
                table: "CarlErpReceivePayments",
                column: "CustomerCreditId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarlErpReceivePayments_CarlErpCustomerCredits_CustomerCreditId",
                table: "CarlErpReceivePayments",
                column: "CustomerCreditId",
                principalTable: "CarlErpCustomerCredits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
