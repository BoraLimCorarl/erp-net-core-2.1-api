using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Customers;
using CorarlERP.Locations;
using CorarlERP.SaleOrders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
namespace CorarlERP.DeliverySchedules
{
    [Table("CarlErpDeliverySchedules")]
    public class DeliverySchedule : AuditedEntity<Guid>, IMayHaveTenant
    {

        public const int MaxNameLength = 100;
        public const int MaxMemoLength = 1020;
        public const int MaxDescriptionLength = 520;
        public int? TenantId { get;  set ; }
        [MaxLength(MaxNameLength)]
        public string DeliveryNo { get; private set; }
        public long LocationId { get; private set; }  
        public Location Location { get; private set; }
        public Guid CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime InitialDeliveryDate { get; private set; }   
        public DateTime FinalDeliveryDate { get; private set; }
        [MaxLength(MaxDescriptionLength)]
        public string Reference { get; private set; }
        [MaxLength(MaxMemoLength)]
        public string Memo { get; private set; }     
        public Guid? SaleOrderId { get; private set; } 
        public SaleOrder SaleOrder { get; private set; }

        public TransactionStatus Status { get; private set; }
        public DeliveryStatus ReceiveStatus { get; private set; }
        public void UpdateReceiveStatusToPending()
        {
            ReceiveStatus = DeliveryStatus.ShipPending;
        }
        public void UpdateReceiveStatusToPartial()
        {
            ReceiveStatus = DeliveryStatus.ShipPartial;
        }
        public void UpdateReceiveStatusToReceiveAll()
        {
            ReceiveStatus = DeliveryStatus.ShipAll;
        }

        public void UpdateStatusToDraft()
        {
            Status = TransactionStatus.Draft;
        }
        public void UpdateStatusToClose()
        {
            Status = TransactionStatus.Close;
        }
        public void UpdateStatusToVoid()
        {
            Status = TransactionStatus.Void;
        }
        public void UpdateStatusToPublish()
        {
            Status = TransactionStatus.Publish;
        }

        public static DeliverySchedule Create(int? tenantId, long creatorUserId, Guid customerId,         
           string reference, string deliveryNo, DateTime initialDeliveryDate, DateTime finalDeliveryDate,
           string memo, 
           long locationId,
           Guid? saleOrderId)
        {
            return new DeliverySchedule()
            {
              
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CustomerId = customerId,            
                Reference = reference,            
                Memo = memo,              
                Status = TransactionStatus.Publish,             
                LocationId = locationId,
                DeliveryNo = deliveryNo,
                InitialDeliveryDate = initialDeliveryDate,
                FinalDeliveryDate = finalDeliveryDate,
                SaleOrderId = saleOrderId,
            };
        }

        public void Update(long lastModifiedUserId, Guid customerId,
           string reference, string deliveryNo, DateTime initialDeliveryDate, DateTime finalDeliveryDate,
           string memo, 
           long locationId,
           Guid? saleOrderId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CustomerId = customerId;
            Reference = reference;
            Memo = memo;
            LocationId = locationId;
            DeliveryNo = deliveryNo;
            InitialDeliveryDate = initialDeliveryDate;
            FinalDeliveryDate = finalDeliveryDate;
            SaleOrderId = saleOrderId;
        }
    }
}
