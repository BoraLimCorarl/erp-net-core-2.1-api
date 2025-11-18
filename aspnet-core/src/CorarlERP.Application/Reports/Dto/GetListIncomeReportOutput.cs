using CorarlERP.Dto;
using CorarlERP.enumStatus;
using CorarlERP.Locations.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{

    public class GetIncomeInput 
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<long>Locations { get; set; }
        public ViewOption ViewOption { get; set; }
    }

    public class GetIncomeReportInput: GetIncomeInput
    {
        public bool IsHasSubHeader { get; set; }
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetListIncomeReportOutput
    {
        public long Id { get; set; }
        public string AccountTypeName { get; set; }
        public int RoundingDigit { get; set; }
        public List<IncomeAccountDetailOutput> AccountLists { get; set; }
        public List<LocationSummaryOutput> LocationSummaryHeader { get; set; }
        public Dictionary<long, TotalLocationColumns> TotalLocationSummaryByAccountType { get; set; }
        public decimal TotalAmount { get; set; }
        public TypeOfAccount AccountType { get; set; }

    }

    public class TotalLocationColumns
    {
        public string LocationName { get; set; }
        public long LocationId { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
    }


    public enum  ViewOption {
        Standard = 1,
        Location = 2,
        Month = 3,
        Class = 4
    }

    public class IncomeAccountDetailOutput
    {
        public Guid Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage {
            get
            {
                return (TotalTemp == 0 ? 0 : (Total / TotalTemp) * 100);
            }
        }

        public Dictionary<long, TotalLocationColumns> TotalLocationColumns { get; set; }
        public decimal TotalTemp { get; set; }
    }

}
