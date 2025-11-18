using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.JournalTransactionTypes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.InventoryTransactions.Dto
{
    public class ListInventoryInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> Items { get; set; }
        public List<Guid?> Customers { get; set; }
        public List<Guid?> Vendors { get; set; }
        public List<long?> Users { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<JournalType> JournalTypes { get; set; }
        public List<long> Lcoations { get; set; }
        public List<Guid?> Accounts { get; set; }
        public List<long> InventoryTypes { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
}
