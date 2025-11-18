using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;
using static CorarlERP.PurchaseOrders.PurchaseOrder;

namespace CorarlERP.PurchaseOrders.Dto
{
   public class GetPurchaseOrderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long> VendorTypes { get; set; }
        public List<Guid> Vendors { get; set; }
        public List <Guid>Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long?> Users { get; set; }
        public List<long> Locations { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderNumber";
            }
          
        }
    }
    public class GetTotalPurchaseOrderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> Currencys { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderNumber";
            }

        }
    }

    public class GetPurchaseOrderItemInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Vendors { get; set; }
        public List<long?> MultiCurrencys { get; set; }
        public List<Guid> ItemIds { get; set; }
        public List<long?> LocationIds { get; set; }
        public Guid? ExceptId { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderNumber";
            }

        }
    }
}
