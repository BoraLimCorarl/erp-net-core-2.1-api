using Abp.AutoMapper;
using CorarlERP.Classes.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.PhysicalCounts.Dto
{
    [AutoMapFrom(typeof(PhysicalCount.PhysicalCount))]
    public class PhysicalCountDetailOutput
    {
        public Guid Id { get; set; }
        public string PhysicalCountNo { get; set; }
        public DateTime PhysicalCountDate { get; set; }
        public string Reference { get; set; }
        public TransactionStatus Status { get; set; }

        public List<ItemSummaryDto> ItemFilter { get; set; }

        public long LocationId { get; set; }
        public LocationSummaryOutput Location { get; set; }
        public long ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }        

        public string Memo { get; set; }
        public List<PhysicalItemDetailDto> Items { get; set; }
        public Guid? ItemIssueId { get; set; }
        public Guid? ItemReceiptId { get; set; }

    }
}
