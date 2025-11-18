using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.SaleOrders.Dto
{
    public class GetSaleOrderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<long> CustomerTypes { get; set; }
        public List<Guid> Customers { get; set; }
        public List<Guid> Items { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<TransactionStatus> Status { get; set; }
        public List<long?> Users { get; set; }
        public List<long> Locations { get; set; }
        public List<DeliveryStatus> DeliveryStatuses { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderNumber";
            }

        }
    }


    public class GetSaleOrderHeaderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public Guid? SaleOrderId { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }
        public List<long?> MultiCurrencys { get; set; }
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderNumber";
            }

        }
    }

    public class GetSaleOrderItemInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
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
