using CorarlERP.ChartOfAccounts.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Items.Dto
{
    public class UpdateItemInput : CreateItemInput
    {
        public Guid Id { get; set; }
        public bool IsGenerateItemCode { get; set; }
    }
}
