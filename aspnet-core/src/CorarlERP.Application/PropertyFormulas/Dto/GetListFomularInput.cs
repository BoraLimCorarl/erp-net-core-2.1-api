using Abp.AutoMapper;
using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyFormulas.Dto
{    
    public class GetListFomularPropertyInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {    
        public bool? IsActive { get; set; }      
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }

        }
      
    }
    [AutoMapFrom(typeof(ItemCodeFormula))]
    public class GetListFomularPropertyOutput
    {
        public bool UseItemProperty { get;  set; }
        public bool UseCustomGenerate { get;  set; }
        public long Id { get; set; }
        public List<string> ItemTypeName { get; set; }
        public bool IsActive { get; set; }
        public bool UseManual { get; set; }
    }

    public class CustomItemCodeInput
    {
        public long? ItemTypeId { get; set; }
    }

}
