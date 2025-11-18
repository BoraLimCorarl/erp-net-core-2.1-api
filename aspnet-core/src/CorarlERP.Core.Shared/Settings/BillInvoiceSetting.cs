using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.Settings
{
    public enum BillInvoiceSettingType
    {
        Bill = 1,
        Invoice =2,
    }


    [Table("CarlErpBillInvoiceSettings")]
    public class BillInvoiceSetting : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public BillInvoiceSettingType SettingType { get; protected set; }

        [Required]
        public bool ReferenceSameAsGoodsMovement { get; private set; }
        
        public bool TurnOffStockValidationForImportExcel { get; private set; }

        public static BillInvoiceSetting Create(int? tenantId, long creatorUserId, BillInvoiceSettingType settingType, bool referenceSameAsGoodsMovement, bool turnOffStockValidationForImportExcel)
        {
            return new BillInvoiceSetting()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                SettingType = settingType,
                ReferenceSameAsGoodsMovement = referenceSameAsGoodsMovement,
                TurnOffStockValidationForImportExcel = turnOffStockValidationForImportExcel,

            };
        }

        public void Update(long lastModifiedUserId, BillInvoiceSettingType settingType, bool referenceSameAsGoodsMovement, bool turnOffStockValidationForImportExcel)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            SettingType = settingType;
            ReferenceSameAsGoodsMovement = referenceSameAsGoodsMovement;
            TurnOffStockValidationForImportExcel = turnOffStockValidationForImportExcel;
        }
    }
}
