using Microsoft.EntityFrameworkCore.Migrations;

namespace CorarlERP.Migrations
{
    public partial class UpdateSaleOrderApplyReturn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (ActiveProvider == "Microsoft.EntityFrameworkCore.SqlServer")
            {
                migrationBuilder.Sql(
				@"
					update CarlErpSaleOrders set
					ReceiveStatus = case when q.IssueQty = 0 then 4 else 
									case when q.OrderQty - q.IssueQty + q.ReturnQty = 0 then 6 
									else 5 end end
					from CarlErpSaleOrders o
					join (
						select oi.SaleOrderId, 
						sum(oi.Qty) as OrderQty, 
						sum(case when ii.SaleOrderItemId is null then 0 else ii.IssueQty end) +
						sum(case when i.OrderItemId is null then 0 else i.IssueQty end) as IssueQty,
						sum(case when ii.SaleOrderItemId is null then 0 else ii.ReturnQty end) + 
						sum(case when i.OrderItemId is null then 0 else i.ReturnQty end) as ReturnQty

						from CarlErpSaleOrderItems oi 

						left join (
							select 
							ii.SaleOrderItemId,
							sum(ii.Qty) as IssueQty,
							sum(case when ri1.Id is null then 0 else ri1.Qty end) +
							sum(case when ri2.Id is null then 0 else ri2.Qty end) as ReturnQty

							from CarlErpItemIssueItems ii 
							left join CarlErpItemReceiptCustomerCreditItem ri1 on ri1.ItemIssueSaleItemId = ii.Id
							left join CarlErpCustomerCreditDetails ci on ci.ItemIssueSaleItemId = ii.Id
							left join CarlErpItemReceiptCustomerCreditItem ri2 on ri2.CustomerCreditItemId = ci.Id

							where ii.SaleOrderItemId is not null 
							group by ii.SaleOrderItemId
						) ii on ii.SaleOrderItemId = oi.Id

						left join (
							select 
							i.OrderItemId,
							sum(ii2.Qty) as IssueQty,
							sum(case when ri3.Id is null then 0 else ri3.Qty end) +
							sum(case when ri4.Id is null then 0 else ri4.Qty end) as ReturnQty

							from CarlErpInvoiceItems i
							join CarlErpItemIssueItems ii2 on ii2.Id = i.ItemIssueItemId
							left join CarlErpItemReceiptCustomerCreditItem ri3 on ri3.ItemIssueSaleItemId = ii2.Id
							left join CarlErpCustomerCreditDetails ci2 on ci2.ItemIssueSaleItemId = ii2.Id
							left join CarlErpItemReceiptCustomerCreditItem ri4 on ri4.CustomerCreditItemId = ci2.Id
							where i.ItemIssueItemId is not null and i.OrderItemId is not null
							group by i.OrderItemId
						) i on i.OrderItemId = oi.Id
						group by oi.SaleOrderId
					) q on o.Id = q.SaleOrderId;

					go

					update CarlErpSaleOrders set IssueCount = oi.IssueCount
					from CarlErpSaleOrders o
					join (
						select oi.SaleOrderId, count(distinct i.ItemIssueId) IssueCount from CarlErpSaleOrderItems oi
						left join (
							select ii.Id, ii.SaleOrderItemId as OrderItemId, ii.ItemIssueId 
							from CarlErpItemIssueItems ii 
							where ii.SaleOrderItemId is not null 	
							union 
							select si.Id, i.OrderItemId, si.ItemIssueId
							from CarlErpInvoiceItems i 
							join CarlErpItemIssueItems si on si.Id = i.ItemIssueItemId
							where i.OrderItemId is not null 
						) i on i.OrderItemId = oi.Id
						group by oi.SaleOrderId
					) oi on o.Id = oi.SaleOrderId;
                ");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
