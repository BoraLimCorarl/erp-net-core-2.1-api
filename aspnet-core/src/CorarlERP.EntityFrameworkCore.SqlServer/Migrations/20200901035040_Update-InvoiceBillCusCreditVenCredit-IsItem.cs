using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateInvoiceBillCusCreditVenCreditIsItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsItem",
                table: "CarlErpVendorCredit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsItem",
                table: "CarlErpInvoices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsItem",
                table: "CarlErpCustomerCredits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsItem",
                table: "CarlErpBills",
                nullable: false,
                defaultValue: false);

            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
                @"                     
                    update CarlErpBills 
                    set IsItem = bi.IsItem
                    from (select BillId, case when Min(ItemId) is null then 0 else 1 end IsItem from CarlErpBillItems bi group by bi.BillId) bi
                    join CarlErpBills b 
                    on b.Id = bi.BillId

                    go

                    update CarlErpInvoices 
                    set IsItem = bi.IsItem
                    from (select InvoiceId, case when Min(ItemId) is null then 0 else 1 end IsItem from CarlErpInvoiceItems bi group by bi.InvoiceId) bi
                    join CarlErpInvoices b 
                    on b.Id = bi.InvoiceId

                    go

                    update CarlErpCustomerCredits 
                    set IsItem = bi.IsItem
                    from (select CustomerCreditId, case when Min(ItemId) is null then 0 else 1 end IsItem from CarlErpCustomerCreditDetails bi group by bi.CustomerCreditId) bi
                    join CarlErpCustomerCredits b 
                    on b.Id = bi.CustomerCreditId

                    go

                    update CarlErpVendorCredit 
                    set IsItem = bi.IsItem
                    from (select VendorCreditId, case when Min(ItemId) is null then 0 else 1 end IsItem from CarlErpVendorCreditDetails bi group by bi.VendorCreditId) bi
                    join CarlErpVendorCredit b 
                    on b.Id = bi.VendorCreditId                   
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsItem",
                table: "CarlErpVendorCredit");

            migrationBuilder.DropColumn(
                name: "IsItem",
                table: "CarlErpInvoices");

            migrationBuilder.DropColumn(
                name: "IsItem",
                table: "CarlErpCustomerCredits");

            migrationBuilder.DropColumn(
                name: "IsItem",
                table: "CarlErpBills");
        }
    }
}
