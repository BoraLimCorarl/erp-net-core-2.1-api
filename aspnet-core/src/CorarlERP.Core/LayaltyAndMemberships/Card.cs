using Abp.Timing;
using CorarlERP.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.LayaltyAndMemberships
{
    [Table("CarlErpCards")]
    public class Card : BaseAuditedEntity<Guid>
    {
        public const int MaxTagLength = 256;     
   
        [MaxLength(MaxTagLength)]
        public string CardId { get; private set; }
        public Customer Customer { get; private set; }
        public Guid? CustomerId { get; private set; }
        public string Remark { get; private set; }
        public CardStatus CardStatus { get; private set; }
        [MaxLength(MaxTagLength)]
        public string CardNumber { get; private set; }
        [MaxLength(MaxTagLength)]
        public string SerialNumber { get; private set; }

        public static Card Create(int? tenantId, long creatorUserId, string cardId, Guid? customerId, string remark, string cardNumber,string serialNumber)
        {
            return new Card()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CardStatus = CardStatus.Enable,
                Remark = remark,             
                CustomerId = customerId,            
                CardId = cardId,
                SerialNumber = serialNumber,
                CardNumber = cardNumber,

            };
        }
        public void Update(long lastModifiedUserId, string cardId, Guid? customerId, string remark,
            string cardNumber, string serialNumber)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Remark = remark;
           
            CustomerId = customerId;
           
            CardId = cardId;
            CardNumber = cardNumber;
            SerialNumber = serialNumber;
        }
        public void Enable()
        {
            this.CardStatus = CardStatus.Enable;
        }
        public void Disable()
        {
            this.CardStatus = CardStatus.Disable;
        }

        public void Deactivate()
        {
            this.CardStatus = CardStatus.Deactivate;
        }
    }

   public enum CardStatus {
        Enable = 1,
        Disable = 2,
        Deactivate = 3,
    }

}
