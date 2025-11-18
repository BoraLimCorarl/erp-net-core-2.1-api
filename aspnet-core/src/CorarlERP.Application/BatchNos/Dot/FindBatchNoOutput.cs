using Abp.Configuration;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public class FindBatchNoOutput
    {
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public long ItemTypeId { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public long LotId { get; set; }
        public string LotName { get; set; }
        public decimal ReceiptQty { get; set; }
        public decimal IssueQty { get; set; }
        public decimal BalanceQty { get; set; }
        public bool Selected { get; set; }
        
        public long? PurchaseTaxId { get; set; }
        public long? SaleTaxId { get; set; }
        public decimal SaleTaxRate { get; set; }
        public Guid? InventoryAccountId { get; set; }
        public string InventoryAccountCode { get; set; }
        public string InventoryAccountName { get; set; }
        public Guid? SaleAccountId { get; set; }
        public string SaleAccountCode { get; set; }
        public string SaleAccountName { get; set; }
        public Guid? PurchaseAccountId { get; set; }
        public string PurchaseAccountCode { get; set; }
        public string PurchaseAccountName { get; set; }
        public decimal? SalePrice { get; set; }
    }
}
