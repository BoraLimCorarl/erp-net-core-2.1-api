using Abp.Application.Editions;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.MultiTenancy;
using CorarlERP.PackageEditions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Subscriptions
{

  
    [Table("CarlErpSubscriptions")]
    public class Subscription : AuditedEntity<Guid>
    {
        public Tenant Tenant { get; private set; }
        public int TenantId { get; private set; }
        public int? Duration { get; private set; }

        private DateTime? _subscriptionDate;
        public DateTime? SubscriptionDate {
            get => _subscriptionDate.HasValue ? _subscriptionDate : StartDate;
            private set => _subscriptionDate = value; 
        }

        public DateTime? StartDate { get; private set; }
        public DateTime? InvoiceDate { get; private set; } 
        public Guid? PackageId { get; private set; }
        public Package Package { get; private set; }
        public Guid? UpgradeFromSubscriptionId { get; private set; }
        public void SetUpdateFromSubscription(Guid id) => UpgradeFromSubscriptionId = id;
        public Edition Edition { get; private set; }
        public int? EditionId { get; private set; }
        public DateTime? Endate { get; private set; }
        public DurationType? DurationType { get; private set; }
        public bool Unlimited { get; private set; }
        public bool IsTrail { get; private set; }
        public bool ShowWarning { get; private set; }
        public DateTime? SubScriptionEndDate { get; private set; }
        public decimal PackagePrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal UpgradeDeduction { get; private set; }
        public decimal TotalPrice { get; private set; } //Total Price = Package Price - Discount - UpdateDeduction
        public SubscriptionType SubscriptionType { get; private set; }

        public static Subscription Create(int tenantId, long? userId, int? duration, DateTime? subscriptionDate, DateTime? startDate, DateTime? endDate, int? editionId, DurationType? durationType, bool unlimited,bool isTrail,bool showWarning, decimal packagePrice, decimal totalPrice, SubscriptionType type, DateTime? invoiceDate, decimal discount, decimal upgradeDeduction, Guid? packageId)
        {
            return new Subscription()
            {
                Id = new Guid(),
                TenantId = tenantId,
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Duration = duration,
                EditionId = editionId,
                SubscriptionDate = subscriptionDate,
                StartDate = startDate,
                Endate = unlimited ? (DateTime?) null : endDate,
                DurationType = durationType,
                Unlimited = unlimited,
                ShowWarning = showWarning,
                IsTrail = isTrail,
                PackagePrice = packagePrice,
                TotalPrice = totalPrice,
                SubscriptionType = type,
                InvoiceDate = invoiceDate,
                Discount = discount,
                UpgradeDeduction = upgradeDeduction,
                PackageId = packageId
            };
        }
       
        public void Update(long lastModifiedUserId, int? duration, DateTime? subscriptionDate, DateTime? startDate, DateTime? endDate, int? editionId, DurationType? durationType, bool unlimited, bool isTrail, bool showWarning, decimal packagePrice, decimal totalPrice, SubscriptionType type, DateTime? invoiceDate, decimal discount, decimal upgradeDeduction, Guid? packageId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Duration = duration;
            EditionId = editionId;
            SubscriptionDate = subscriptionDate;
            StartDate = startDate;
            Unlimited = unlimited;
            Endate = unlimited ? (DateTime?)null : endDate;
            DurationType = durationType;
            IsTrail = isTrail;
            ShowWarning = showWarning;
            PackagePrice = packagePrice;
            TotalPrice = totalPrice;
            SubscriptionType = type;
            InvoiceDate = invoiceDate;
            Discount = discount;
            UpgradeDeduction = upgradeDeduction;
            PackageId = packageId;
        }

        public void SetRenewEndDate(DateTime? renewEndate)
        {
            this.SubScriptionEndDate = renewEndate;
        }

    }
}
