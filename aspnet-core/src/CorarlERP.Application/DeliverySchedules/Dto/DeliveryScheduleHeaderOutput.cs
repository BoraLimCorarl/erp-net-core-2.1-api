using Abp.AutoMapper;
using Abp.Runtime.Validation;
using CorarlERP.Customers.Dto;
using CorarlERP.Dto;
using CorarlERP.Items.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.DeliverySchedules.Dto
{
    [AutoMapFrom(typeof(DeliverySchedule))]
    public class DeliveryScheduleHeaderOutput
    {
        public string Memo { get; set; }
        public Guid Id { get; set; }
        public string DeliveryNo { get; set; }     
        public string Reference { get; set; }          
        public int CountDeliveryItems { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerSummaryOutput Customer { get; set; }
        public DateTime InitialDeliveryDate { get; set; }
        public DateTime FinalDeliveryDate { get; set; }
        public Guid SaleOrderId { get; set; }
    }
    public class GetDeliveryScheduleHeaderListInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Customers { get; set; }
        public List<long?> Locations { get; set; }   
        public void Normalize()
        {

            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "DeliveryNo";
            }

        }
    }
    [AutoMapFrom(typeof(DeliverySchedule))]
    public class GetListDeliveryItemDetail
    {
        public long LocationId { get; set; }
        public Guid Id { get; set; }
        public decimal Remain { get; set; }     
        public string Memo { get; set; }    
        public Guid CustomerId { get; set; }
        public CustomerSummaryDetailOutput Customer { get; set; }
        public DateTime InitialDeliveryDate { get; set; }
        public DateTime FinalDeliveryDate { get; set; }
        public string DeliveryNo { get; set; }          
        public List<DeliveryItemSummaryOut> Items { get; set; }
        public string Reference { get; set; }
        public string SaleOrderId { get; set; }
        public int CountDeliveryItems { get; set; }

    }

    [AutoMapFrom(typeof(DeliverySchedule))]
    public class DeliveryScheduleSummaryOutput
    {
        public Guid Id { get; set; }
        public string DeliveryNo { get; set; }
        public string StatusCode { get; set; }
        public DateTime InitialDeliveryDate { get; set; }
    }

    [AutoMapFrom(typeof(DeliveryScheduleItem))]
    public class DeliveryItemSummaryOut
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public ItemSummaryDetailOutput Item { get; set; }
        public decimal Qty { get; set; }     
        public decimal Price { get; set; }
        public decimal Remain
        {
            get; set;
        }
        public string DeliveryNo { get; set; }
        public string Description { get; set; }
        public Guid SaleOrderItemId { get; set; }
        public Guid SaleOrderId { get; set; }
        public string SaleOrderNumber { get; set; }
        public string SaleOrderReference { get; set; }
    }




}
