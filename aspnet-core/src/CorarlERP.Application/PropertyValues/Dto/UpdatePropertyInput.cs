using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
    public class UpdatePropertyInput
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsUnit { get; set; }
        public bool IsRequired { get; set; }
        public bool IsStatic { get; set; }
        public bool IsItemGroup { get; set; }
        public bool IsStandardCostGroup { get; set; }
    }


    public class ImportUpdatePropertyInput
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool IsUnit { get; set; }
        public bool IsRequired { get; set; }
        public bool IsStatic { get; set; }
        public bool IsItemGroup { get; set; }
        public bool IsStandardCostGroup { get; set; }
        public List<ImportEditPropertyValueInput> PropertyValues { get; set; }
    }

}
