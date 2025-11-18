using Abp.Configuration;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.BatchNos.Dto
{
    public enum BatchNoState
    {
        Available = 0,
        NotInstock = 1,
        NegativeStock = 2
    }

    public class FindBatchNoInput: PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<Guid> Items { get; set; }
        public DateTime ToDate { get; set; }
        public List<long> Locations { get; set; }
        public List<long> Lots { get; set; }
        public bool? IsStandard { get; set; }
        public BatchNoState? State { get; set; }
        public List<Guid> ExceptIds { get; set; }
        public bool? IsSerial { get; set; }
        public bool? TrackExpiration { get; set; }
        public DateTime? ExpiredFrom { get; set; }
        public DateTime? ExpiredTo { get; set; }
        public Guid? ProductionPlanId { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "BatchNumber";
            }
        }
    }
}
