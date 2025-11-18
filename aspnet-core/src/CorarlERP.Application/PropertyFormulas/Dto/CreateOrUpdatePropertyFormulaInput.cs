using CorarlERP.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.PropertyFormulas.Dto
{
   public class CreateOrUpdatePropertyFormulaInput
    {
        public long? Id { get; set; }
        public bool UseItemProperty { get; set; }
        public bool UseCustomGenerate { get; set; }
        public bool UseManual { get; set; }
        public List<CreateOrUpdateItemCodeFormulaPropertyOutput> Items { get; set; }
        public List<CreateOrUpdateItemCodeFormulaItemTypeInput> ItemCodeFormulaItemTypes { get; set; }
        public CreateOrUpdateItemFormulaCustomInput ItemFormulaCustoms { get; set; }

    }


    public class CreateOrUpdateItemFormulaCustomInput {
        public string Prefix { get; set; }
        public string ItemCode { get; set; }   
        public long ItemCodeFormulaId { get;  set; }
        public long? Id { get; set; }
    }


    public class CreateOrUpdateItemCodeFormulaItemTypeInput
    {
        public long? Id { get; set; }
        public long ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public long ItemCodeFormulaId { get; set; }
    }
    public class GetDetailPropertyFormulaOutput {

        public CreateOrUpdateItemFormulaCustomInput ItemFormulaCustoms { get; set; }
        public long Id { get; set; }    
        public string Separator { get; set; }
        public bool UseItemProperty { get;  set; }
        public bool UseCustomGenerate { get; set; }
        public bool UseManual { get; set; }
        public List<GetDetailItemCodeFormulaProperty> ItemCodeFormulaProperties { get; set; }
        public List<CreateOrUpdateItemCodeFormulaItemTypeInput> ItemCodeFormulaItemTypes { get; set; }
        public string ItemTypeName { get; set; }
    }
    public class CreateOrUpdateAutoItemCodeInput
    {
        public string ItemCode { get; set; }
        public string Prefix { get; set; }
        public int TenantId { get; set; }
        public ItemCodeSetting ItemCodeSetting { get; set; }
    }

   

}
