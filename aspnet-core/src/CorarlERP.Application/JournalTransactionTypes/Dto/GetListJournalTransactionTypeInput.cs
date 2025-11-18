using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.JournalTransactionTypes.Dto
{
   public class GetListJournalTransactionTypeInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool? IsActive { get; set; }
        public List<long>InventoryTypes { get; set; }
        public List<InventoryTransactionType> InventoryTransactionType { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "Name";
            }
        }
    }
}
