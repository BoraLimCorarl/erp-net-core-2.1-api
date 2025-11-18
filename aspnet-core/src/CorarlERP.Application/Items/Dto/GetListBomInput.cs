using Abp.Runtime.Validation;
using CorarlERP.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
        public class GetListBomInput : PagedSortedAndFilteredInputDto, IShouldNormalize
        {          
            public bool? IsActive { get; set; }  
            public List<Guid> ItemIds { get; set; } 
            public void Normalize()
            {
                if (string.IsNullOrEmpty(Sorting))
                {
                    Sorting = "Name";
                }

        }
    }
    public class GetListBomExcelInput {

        public string Filter { get; set; }
        public bool? IsActive { get; set; }
        public List<Guid> ItemIds { get; set; }
    }


}
