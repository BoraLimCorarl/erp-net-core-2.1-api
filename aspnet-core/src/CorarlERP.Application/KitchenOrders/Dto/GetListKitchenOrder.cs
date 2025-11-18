using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.KitchenOrders.Dto
{
    public class GetListKitchenOrderInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }     
        public List<Guid> Customers { get; set; }      
        public List<long?> Users { get; set; }        
        public List<TransactionStatus> Status { get; set; }  
        public List<long> Locations { get; set; }    
        public List<long> ClassIds { get; set; }
        public List<Guid> ItemIds { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "OrderDate.Date";
            }
        }
    }

    public class GetListKitchenOrderOutput {
        public TransactionStatus Status { get; set; }
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }    
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Reference { get; set; }      
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }    
        public long? LocationId { get; set; }
        public string LocationName { get; set; }       
        public Guid Id { get; set; }
   

    }



}
