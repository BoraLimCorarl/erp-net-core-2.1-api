using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.BatchNos;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Currencies;
using CorarlERP.Galleries;
using CorarlERP.ItemTypes;
using CorarlERP.Taxes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Items
{
    [Table("CarlErpItems")]
    public class Item : AuditedEntity<Guid>, IMayHaveTenant
    {
        public const int MaxItemCodeLength = 256;
        public const int MaxItemNameLength = 512;

        #region Properties

        [Required]
        [MaxLength(MaxItemCodeLength)]
        public string ItemCode { get; private set; }

        public int? TenantId { get; set; }

        [Required]
        [MaxLength(MaxItemNameLength)]
        public string ItemName { get; private set; }

        public decimal? PurchaseCost { get; private set; }

        public decimal? SalePrice { get; private set; }

        public decimal? ReorderPoint { get; private set; }
        public string Description { get; private set; }
        public bool TrackSerial { get; private set; }
        public Member Member { get; set; }
        //Item Type
        [Required]
        public long ItemTypeId { get; private set; }
        public ItemType ItemType { get; private set; }

        public ICollection<ItemProperty> Properties { get; private set; }

        //Chart of Account
        public long? SaleCurrenyId { get; private set; }
        public Currency SaleCurrency { get; private set; }

        public long? PurchaseCurrencyId { get; private set; }
        public Currency PurchaseCurrency { get; private set; }

        public Guid? SaleAccountId { get; private set; }
        public ChartOfAccount SaleAccount { get; set; }

        public Guid? PurchaseAccountId { get; private set; }
        public ChartOfAccount PurchaseAccount { get; private set; }

        public Guid? InventoryAccountId { get; private set; }
        public ChartOfAccount InventoryAccount { get; private set; }

        //Tax
        public long? PurchaseTaxId { get; private set; }
        public Tax PurchaseTax { get; private set; }

        public long? SaleTaxId { get; private set; }
        public Tax SaleTax { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        #endregion

        public bool ShowSubItems { get; private set; }
        public bool SetShowSubItems(bool show) => ShowSubItems = show;

        public Guid? ImageId { get; private set; }
        public virtual Gallery Image { get; private set; }
        public void SetImage(Guid? imageId) => ImageId = imageId;

        public string Barcode { get; private set; }

        public bool UseBatchNo { get; private set; }
        public bool AutoBatchNo { get; private set; }
        public long? BatchNoFormulaId { get; set; }
        public BatchNoFormula BatchNoFormula { get; private set; }
        public bool TrackExpiration { get; private set; }
        public bool BarcodeSameAsItemCode { get; private set; }

        public decimal Min { get; private set; }
        public decimal Max { get; private set; }

        #region Public Methods


        public void SetMinMax (decimal min , decimal max)
        {
            this.Min = min;
            this.Max = max;
        }

        public static Item Create(int? tenantId, long creatorUserId, string itemName, string itemCode, decimal? salePrice,
            decimal? purchaseCost, decimal? reorderPoint, bool trackSerial,
            long? saleCurrenyId, long? purchaseCurrencyId, long? purchaseTaxId, long?
            saleTaxId, Guid? saleAccountId, Guid? purchaseAccountId, Guid? inventoryAccountId,
            long itemTypeId, string description, string barcode, bool useBatchNo, bool autoBatchNo, long? batchFormulaId, bool trakeExpiration, bool barcodeSameAsItemCode)
        {
            return new Item()
            {
                Id = Guid.NewGuid(),
                TrackSerial = trackSerial,
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ItemCode = itemCode,
                ItemName = itemName,
                SalePrice = salePrice,
                SaleAccountId = saleAccountId,
                SaleCurrenyId = saleCurrenyId,
                SaleTaxId = saleTaxId,
                PurchaseAccountId = purchaseAccountId,
                PurchaseCost = purchaseCost,
                PurchaseCurrencyId = purchaseCurrencyId,
                PurchaseTaxId = purchaseTaxId,
                InventoryAccountId = inventoryAccountId,
                ReorderPoint = reorderPoint,
                ItemTypeId = itemTypeId,
                IsActive = true,
                Properties = new List<ItemProperty>(),
                Description = description,
                Barcode = barcodeSameAsItemCode ? itemCode : barcode,
                UseBatchNo = useBatchNo,
                AutoBatchNo = autoBatchNo,
                BatchNoFormulaId = batchFormulaId,
                TrackExpiration = trakeExpiration,
                BarcodeSameAsItemCode = barcodeSameAsItemCode,
            };

        }
        
        public void Update(long lastModifiedUserId, string itemName, string itemCode, decimal? salePrice,
            decimal? purchaseCost, decimal? reorderPoint, bool trackSerial,
            long? saleCurrenyId, long? purchaseCurrencyId, long? purchaseTaxId, long?
            saleTaxId, Guid? saleAccountId, Guid? purchaseAccountId, Guid? inventoryAccountId,
            long itemTypeId, string description, string barcode, bool useBatchNo, bool autoBatchNo, long? batchFormulaId, bool trakeExpiration,
            bool barcodeSameAsItemCode)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            ItemCode = itemCode;
            ItemName = itemName;
            SalePrice = salePrice;
            TrackSerial = trackSerial;
            SaleAccountId = saleAccountId;
            SaleCurrenyId = saleCurrenyId;
            SaleTaxId = saleTaxId;
            PurchaseAccountId = purchaseAccountId;
            PurchaseCost = purchaseCost;
            PurchaseCurrencyId = purchaseCurrencyId;
            PurchaseTaxId = purchaseTaxId;
            InventoryAccountId = inventoryAccountId;
            ReorderPoint = reorderPoint;
            ItemTypeId = itemTypeId;
            Description = description;
            Barcode = barcodeSameAsItemCode ? itemCode : barcode;
            UseBatchNo = useBatchNo;
            AutoBatchNo = autoBatchNo;
            BatchNoFormulaId = batchFormulaId;
            TrackExpiration = trakeExpiration;
            BarcodeSameAsItemCode = barcodeSameAsItemCode;
        }

        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }

        public void UpdateMember(Member member)
        {
            Member = member;
        }

        //Item Properties

        public ItemProperty AddProperty(long creatorUserId, long propertyId, long? propertyValueId)
        {
            var @property = ItemProperty.Create(this.TenantId, creatorUserId, propertyValueId, propertyId, Id);

            this.Properties.Add(@property);

            return @property;
        }

        public void RemoveProperty(Guid itemPropertyId)
        {
            var @property = Properties.FirstOrDefault(p => p.Id == itemPropertyId);
            if (@property == null) return;

            Properties.Remove(@property);
        }

        public ItemProperty UpdateProperty(Guid itemPropertyId, long userId, long propertyId, long? propertyValueId)
        {
            var @property = this.Properties.FirstOrDefault(p => p.Id == itemPropertyId);
            property?.Update(userId, propertyValueId, propertyId, Id);

            return property;
        }

        public void UpdateUseBatch(long userId, bool trackSerial, bool useBatchNo, bool autoBatchNo, long? batchFormulaId, bool trackExpiration,string barcode)
        {
            LastModifierUserId = userId;
            LastModificationTime = Clock.Now;
            TrackSerial = trackSerial;
            UseBatchNo = useBatchNo;
            AutoBatchNo = autoBatchNo;
            BatchNoFormulaId = batchFormulaId;
            TrackExpiration = trackExpiration;
            Barcode = barcode;
        }


         //   ItemCode = itemcode;
            
       // }

        #endregion




    }
}