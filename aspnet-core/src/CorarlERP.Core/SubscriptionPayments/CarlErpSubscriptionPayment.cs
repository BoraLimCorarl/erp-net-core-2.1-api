using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CorarlERP.MultiTenancy;
using Abp.Timing;
using Abp.Application.Editions;

namespace CorarlERP.SubscriptionPayments
{
    public enum SubscriptionType
    {
        Subscribe = 0,
        Renew = 1,
        Upgrade = 2,
        Downgrade = 3,
    }

    public enum SubscriptionPaymentMethod
    {
        KHQR = 1,
    }

    [Table("CarlErpSubscriptionPayments")]
    public class CorarlSubscriptionPayment : AuditedEntity<Guid>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public Tenant Tenant { get; private set; }
        public DateTime SubscriptionDate { get; private set; }
        public DateTime AffectedDate { get; private set; }
        public int Duration { get; private set; }
        public DurationType DurationType { get; private set; }
        public DateTime EndDate { get; private set; }
        public SubscriptionPaymentMethod PaymentMethod { get; private set; }
        public decimal PackagePrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public int EditionId { get; private set; }
        public Edition Edition { get; private set; }
        public SubscriptionType SubscriptionType { get; private set; }

        public static CorarlSubscriptionPayment Create(int tenantId, long userId, SubscriptionType subscriptionType, int editionId, DateTime subscriptionDate, DateTime affectedDate, DateTime endDate, int duration, DurationType durationType, SubscriptionPaymentMethod paymentMethod, decimal packagePrice, decimal totalPrice)
        {
            return new CorarlSubscriptionPayment
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreationTime = Clock.Now,
                CreatorUserId = userId,
                SubscriptionType = subscriptionType,
                EditionId = editionId,
                SubscriptionDate = subscriptionDate,
                AffectedDate = affectedDate,
                EndDate = endDate,
                Duration = duration,
                DurationType = durationType,
                PaymentMethod = paymentMethod,
                PackagePrice = packagePrice,
                TotalPrice = totalPrice
            };
        }

        public void Update(long userId, SubscriptionType subscriptionType, int editionId, DateTime subscriptionDate, DateTime affectedDate, DateTime endDate, int duration, DurationType durationType, SubscriptionPaymentMethod paymentMethod, decimal packagePrice, decimal totalPrice)
        {
            LastModificationTime = Clock.Now;
            LastModifierUserId = userId;
            SubscriptionType = subscriptionType;
            EditionId = editionId;
            SubscriptionDate = subscriptionDate;
            AffectedDate = affectedDate;
            EndDate = endDate;
            Duration = duration;
            DurationType = durationType;
            PaymentMethod = paymentMethod;
            PackagePrice = packagePrice;
            TotalPrice = totalPrice;
        }
    }
}
