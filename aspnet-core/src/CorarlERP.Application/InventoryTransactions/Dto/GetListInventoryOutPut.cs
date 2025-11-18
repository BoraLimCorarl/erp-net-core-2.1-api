using Abp.Runtime.Validation;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.InventoryTransactions.Dto
{
    public class GetListInventoryOutPut
    {
        public long? CreationTimeIndex { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public Guid? CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public UserDto User { get; set; }
        public Guid? VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public TransactionStatus Status { get; set; }
        public int CountItem { get; set; }
        public string Memo { get; set; }
        public JournalType Type { get; set; }
        public string TypeName { get; set; }
        public string AccountName { get; set; }
        public string LocationName { get; set; }
        public Guid AccountId { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }
        public long? LocationId { get; set; }
        public string LastModifiedUserName { get; set; }
        public DateTime? LastModifiedTime { get; set; }
        public string JournalTransactionTypeName {get;set;}
        public Guid JournalId { get; set; }
    }
  
}
