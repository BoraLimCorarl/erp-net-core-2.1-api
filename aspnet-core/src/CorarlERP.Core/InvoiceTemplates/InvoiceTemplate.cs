using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Galleries;
using CorarlERP.TransactionTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.InvoiceTemplates
{
   
    [Table("CarlErpInoviceTemplates")]
    public class InvoiceTemplate : AuditedEntity<Guid>, IMayHaveTenant
    {
        
        public const int MaxNameLength = 512;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; private set; }
        public int? TenantId { get; set; }

        public string TemplateOption { get; private set; }
        public bool IsActive { get; private set; }

        public Guid GalleryId { get;set; }
        public Gallery Gallery { get; set; }

        public InvoiceTemplateType TemplateType { get; private set; }
        public bool ShowDetail { get; private set; }
        public bool ShowSummary { get; private set; }

        public static InvoiceTemplate Create(int? tenantId, long? creatorUserId, string name, Guid galleryId, string templateOption, InvoiceTemplateType templateType, bool showDetail, bool showSummary)
        {
            return new InvoiceTemplate()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                Name = name,
                GalleryId = galleryId,
                TemplateOption = templateOption,
                TemplateType = templateType,
                ShowDetail = showDetail,
                ShowSummary = showSummary,
                IsActive = true
            };
        }
        public void Update(long? lastModifiedUserId, string name, Guid galleryId, string templateOption, InvoiceTemplateType templateType, bool showDetail, bool showSummary)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Name = name;
            GalleryId = galleryId;
            TemplateOption = templateOption;
            TemplateType = templateType;
            ShowDetail = showDetail;
            ShowSummary = showSummary;
        }

        public void SetIsActive(bool isActive) => IsActive = isActive;
    }
}
