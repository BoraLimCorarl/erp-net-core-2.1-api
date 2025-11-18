using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.LayaltyAndMemberships.Dto
{
    public class GetListCardInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public List<CardStatus> CardStatus { get; set; }     
        public List<Guid>Customers { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(this.Sorting))
            {
                Sorting = "CardId";
            }
        }
    }

    public class GetListCardExcelInput {
        public string Filter { get; set; }
        public List<CardStatus> CardStatus { get; set; }
        public List<Guid> Customers { get; set; }
    }

}
