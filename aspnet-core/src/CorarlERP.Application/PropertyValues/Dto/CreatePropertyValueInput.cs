using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.PropertyValues.Dto
{
    public class CreatePropertyValueInput
    {
        [Required]
        public string Value { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsDefault { get; set; }
        public string Code { get; set; }
        public bool IsBaseUnit { get; set; }
        public long? BaseUnitId { get; set; }
        public decimal Factor { get; set; }
    }

    public class CreatePropertyValueInputExcel
    {
        public long Id { get; set; }
        public string PropertyName { get; set; }
        public string Value { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsDefault { get; set; }
        public string Code { get; set; }
        public bool IsBaseUnit { get; set; }
        public long? BaseUnitId { get; set; }
        public string BaseUnitName { get; set; }
        public decimal Factor { get; set; }
    }

    public class CreatePropertyValueDto {
        public long PropertyId { get; set; }
        public string Value { get; set; }
        public decimal NetWeight { get; set; }
        public bool IsDefault { get; set; }
        public string Code { get; set; }
        public bool IsBaseUnit { get; set; }
        public long? BaseUnitId { get; set; }
        public decimal Factor { get; set; }
    }

    public class AddPropertyValueInput: CreatePropertyValueInput
    {
        [Required]
        public long PropertyId { get; set; }
    }

    public class EditPropertyValueInput : CreatePropertyValueInput
    {
        [Required]
        public long Id { get; set; }
        public long? PropertyId { get; set; }

    }

    public class ImportEditPropertyValueInput : CreatePropertyValueInput
    {
       
        public long? Id { get; set; }
        public long? PropertyId { get; set; }

    }

}


