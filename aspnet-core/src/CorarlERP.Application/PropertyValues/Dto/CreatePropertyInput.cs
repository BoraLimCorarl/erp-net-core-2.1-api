using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
   
    public class CreatePropertyInput
    {
        [Required]
        public string Name { get; set; }
        public bool IsRequired { get; set; }
        public bool IsStatic { get; set; }
        public bool IsUnit { get; set; }
        public bool IsStandardCostGroup { get; set; }     
        [Required]
        public List<CreatePropertyValueInput> Values { get; set; }

        public bool IsItemGroup { get; set; }
    }
}
