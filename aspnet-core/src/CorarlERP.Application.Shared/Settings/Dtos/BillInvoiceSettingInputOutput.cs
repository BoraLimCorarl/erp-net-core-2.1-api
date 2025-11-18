using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Settings.Dtos
{
    public class BillInvoiceSettingInputOutput
    {
        public long Id { get; set; }
        public BillInvoiceSettingType SettingType { get; set; }
        public bool ReferenceSameAsGoodsMovement { get; set; }
        public bool TurnOffStockValidationForImportExcel { get; set; }

    }
}
