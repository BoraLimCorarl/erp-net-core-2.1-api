using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CorarlERP.ChartOfAccounts.Dto
{

    public class CreateChartAccountInput
    {
        [Required]
        public string AccountCode { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public long TaxId { get; set; }

        [Required]
        public long AccountTypeId { get; set; }

        public Guid? ParentAccountId { get; set; }

        public string Description { get; set; }
        public SubAccountType? SubAccountType { get; set; }


    }

    public class CreateImportExcel
    {
        public long Id { get; set; }
        public string AccountCode { get; set; }

        public string AccountName { get; set; }
    
        public long TaxId { get; set; }
     
        public long AccountTypeId { get; set; }        

        public string Description { get; set; }

        public Guid? ParentAccountId { get; set; }
        public SubAccountType? SubAccountType { get; set; }
    }

}
