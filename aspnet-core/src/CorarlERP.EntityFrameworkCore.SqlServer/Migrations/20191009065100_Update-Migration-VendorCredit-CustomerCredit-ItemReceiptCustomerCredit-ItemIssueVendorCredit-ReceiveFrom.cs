using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateMigrationVendorCreditCustomerCreditItemReceiptCustomerCreditItemIssueVendorCreditReceiveFrom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                 
                  UPDATE CarlErpItemIssueVendorCredit
                         SET ReceiveFrom = 13
                 go 
                  Update CarlErpItemReceiptCustomerCredit
                         SET ReceiveFrom = 14   
                 go 
                  Update CarlErpVendorCredit
                         SET ReceiveFrom = 1
                 go 
                  Update CarlErpCustomerCredits
                         SET ReceiveFrom = 1

                --update vendor credit account posting
                update ji 
                set ji.AccountId = i.PurchaseAccountId 
                from CarlErpJournalItems ji
                join CarlErpJournals j on j.Id = ji.JournalId 
                join CarlErpVendorCreditDetails vcd on vcd.Id = ji.Identifier
                join CarlErpItems i on i.Id = vcd.ItemId
                where ji.TenantId = i.TenantId and j.VendorCreditId is not null and Identifier is not null
                go

                --journal item to insert for item issue vendor credit
                insert into CarlErpJournalItems (Id, CreationTime, CreatorUserId, TenantId, JournalId, AccountId, Debit, Credit, [Key], Identifier)
                select 
                NEWID() Id,
                ji.CreationTime,
                ji.CreatorUserId,
                ji.TenantId,
                ji.JournalId,
                i.PurchaseAccountId,
                ji.Credit Debit,
                ji.Debit Credit,
                6 , 
                ji.Identifier
                from CarlErpJournalItems ji
                join CarlErpJournals j on j.Id = ji.JournalId 
                join CarlErpItemIssueVendorCreditItem iivci on iivci.Id = ji.Identifier
                join CarlErpItems i on i.Id = iivci.ItemId
                where ji.TenantId = i.TenantId and j.ItemIssueVendorCreditId is not null and Identifier is not null
                go

                --update customer credit account posting
                update ji 
                set ji.AccountId = i.SaleAccountId 
                from CarlErpJournalItems ji
                join CarlErpJournals j on j.Id = ji.JournalId 
                join CarlErpCusotmerCreditDetails ccd on ccd.Id = ji.Identifier
                join CarlErpItems i on i.Id = ccd.ItemId
                where ji.TenantId = i.TenantId and j.CustomerCreditId is not null and Identifier is not null
                go

                --journal item to insert for item receipt customer credit
                insert into CarlErpJournalItems (Id, CreationTime, CreatorUserId, TenantId, JournalId, AccountId, Debit, Credit, [Key], Identifier)
                select 
                NEWID() Id,
                ji.CreationTime,
                ji.CreatorUserId,
                ji.TenantId,
                ji.JournalId,
                i.PurchaseAccountId,
                ji.Credit Debit,
                ji.Debit Credit,
                6 , 
                ji.Identifier
                from CarlErpJournalItems ji
                join CarlErpJournals j on j.Id = ji.JournalId 
                join CarlErpItemReceiptCustomerCreditItem ircci on ircci.Id = ji.Identifier
                join CarlErpItems i on i.Id = ircci.ItemId
                where ji.TenantId = i.TenantId and j.ItemReceiptCustomerCreditId is not null and Identifier is not null
                go

                --journal items to delete
                delete from CarlErpJournalItems where Id in(
                select ji.Id from CarlErpJournalItems ji
                join CarlErpJournals j on j.Id = ji.JournalId 
                where (j.ItemIssueVendorCreditId is not null or j.ItemReceiptCustomerCreditId is not null) and Identifier is null
                )
                go

                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
