using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ItemTypes
{
    [Table("CarlErpItemTypes")]
    public class ItemType : AuditedEntity<long>
    {
        public const int MaxNameLength = 512;

        [Required]
        [MaxLength(MaxNameLength)]
        public string Name { get; set; }
        
        public bool DisplayInventoryAccount { get; private set; }
        public bool DisplayPurchase { get; private set; }
        public bool DisplaySale { get; private set; }
        public bool DisplayReorderPoint { get; private set; }
        public bool DisplayTrackSerialNumber { get; private set; }
        public bool DisplaySubItem { get; private set; }

        public bool DisplayItemMenu { get; private set; }

        public static ItemType Create(long? creatorUserId, string name,  bool displayInventoryAccount,
            bool displayPurchase, bool displaySale, bool displayReorderPoint, bool displayTrackSerialNumber, bool displaySubItem,
            bool displayItemMenu)
        {
            return new ItemType()
            {
                Name = name,
                DisplayInventoryAccount = displayInventoryAccount,
                DisplayPurchase = displayPurchase,
                DisplayReorderPoint = displayReorderPoint,
                DisplaySale = displaySale,
                DisplaySubItem = displaySubItem,
                DisplayTrackSerialNumber = displayTrackSerialNumber,
                DisplayItemMenu = displayItemMenu
            };
        }
        public void Update(long? lastModifiedUserId, string name, bool displayInventoryAccount,
            bool displayPurchase, bool displaySale, bool displayReorderPoint, bool displayTrackSerialNumber, bool displaySubItem,
            bool displayItemMenu)
        {
            LastModifierUserId = lastModifiedUserId;
            Name = name;
            DisplayInventoryAccount = displayInventoryAccount;
            DisplayPurchase = displayPurchase;
            DisplayReorderPoint = displayReorderPoint;
            DisplaySale = displaySale;
            DisplaySubItem = displaySubItem;
            DisplayTrackSerialNumber = displayTrackSerialNumber;
            DisplayItemMenu = displayItemMenu;
        }
    }
}
