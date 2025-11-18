using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.Taxes.Dto
{

    public class CreateTaxInput
    {
        [Required]
        public string TaxName { get; set; }

        [Required]
        public decimal TaxRate { get; set; }
    }
}
