using CorarlERP.BatchNos;
using CorarlERP.Dto;
using CorarlERP.Inventories.Data;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class InventoryTransactionInput : PagedSortedAndFilteredInputDto 
    {
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid> InventoryAccounts { get; set; }
        public List<long?> AccountTypes { get; set; }
        public List<JournalType> JournalTypes { get; set; }

        //public List<long?> ItemProperties { get; set; }
        public List<GetListPropertyFilter> ItemProperties { get; set; }
        public List<Guid> Items { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        public List<long> Lots { get; set; }
        public bool UsePagination { get; set; }
        public bool IsLoadMore { get; set; }
        public string GroupBy { get; set; }
        public bool ShowGroupHeaderIfNoRecords { get; set; }
        public List<Guid> JournalTransactionTypeIds { get; set; }
        public List<long> InventoryTypes { get; set; }
    }
    
    public class GetInventoryTransactionExportReportInput : InventoryTransactionInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetListTransactionReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListInventoryTransationReportOutput> Items { get; set; }
    }

    public class GetListInventoryTransationReportOutput
    {

        public Guid? Id { get; set; }
        public JournalType? JournalType { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal TotalQty { get; set; }
        public decimal Beginning { get; set; }
        public decimal TotalInQty { get; set; }
        public decimal TotalOutQty { get; set; }
        public Guid InventoryAccountId { get; set; }
        public string InventoryAccountName { get; set; }
        public string InventoryAccountCode { get; set; }
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string User { get; set; }
        public string LotName { get; set; }
        public long LotId { get; set; }
        public string PartnerName { get; set; }
        public string Memo { get; set; }
        public string Reference { get; set; }
        public string JournalTypeName { get; set; }
        public int Rank { get; set; }
        public string JournalNo { get; set; }
        public decimal NetWeight { get => Unit == null ? 0 : Unit.NetWeight * (TotalInQty - TotalOutQty); }

        public UnitDto Unit { get; set; }
        public List<ItemPropertySummary> Properties { get; set; }
        public long? CreationTimeIndex { get; set; }
        public DateTime CreationTime { get; set; }
        public string ProductionProcessName { get; set; }
        public string Description { get; set; }
        public string JournalTransactionTypeName { get; set; }
        public Guid? JournalTransactionTypeId {get;set;}
        public bool? Issue { get; set; }
        public List<BatchNoItemOutput> ItemBatchNos { get; set; }
    }
    
}
