using Abp.AutoMapper;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Vendors.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ItemIssues.Dto
{
    [AutoMapFrom(typeof(ItemIssue))]
    public class GetListItemIssueOutput
    {
        public long? CreationTimeIndex { get; set; }
        public DateTime? CreationTime { get; set; }
        public Guid Id { get; set; }
        public string JournalNo { get; set; }
        public string Reference { get; set; }
        public string LocationName { get; set; }
        public Guid? CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public Guid? VendorId { get; set; }
        public VendorSummaryOutput Vendor { get; set; }
        public UserDto User { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public TransactionStatus Status { get; set; }
        public int CountItem { get; set; }
        public string Memo { get; set; }
        public JournalType Type { get; set; }
        public string TypeName { get; set; }
        public string AccountName { get; set; }
        public Guid AccountId { get; set; }
        public bool IsCanVoidOrDraftOrClose { get; set; }
        public string JournalTransactionTypeName { get; set; }
    }
}
