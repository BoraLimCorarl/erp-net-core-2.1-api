using Abp.AutoMapper;
namespace CorarlERP.ItemTypes.Dto
{
    [AutoMapFrom(typeof(ItemType))]
    public class ItemTypeDetailOutput
    {
        public string Name { get; set; }
        public long? Id { get; set; }
        public bool DisplayInventoryAccount { get; set; }
        public bool DisplayPurchase { get; set; }
        public bool DisplaySale { get; set; }
        public bool DisplayReorderPoint { get; set; }
        public bool DisplayTrackSerialNumber { get; set; }
        public bool DisplaySubItem { get;set; }
        public bool DisplayItemMenu { get; set; }
    }
}
