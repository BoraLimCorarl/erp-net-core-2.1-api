using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Classes.Dto
{
    public class CreateClassInput
    {
        public long? ParentClassId { get; set; }

        //[Required]
        public string ClassName { get; set; }

        public bool ClassParent { get; set; }       
    }
}
