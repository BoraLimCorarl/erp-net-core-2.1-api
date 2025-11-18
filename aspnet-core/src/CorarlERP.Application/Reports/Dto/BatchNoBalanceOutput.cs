using Abp.Application.Services.Dto;
using CorarlERP.Inventories.Data;
using CorarlERP.Migrations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class BatchNoBalanceOutput
    {
        public DateTime Date { get; set; }
        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Guid BatchNoId { get; set; }
        public string BatchNumber { get; set; }
        public string LocationName { get; set; }
        public long LocationId { get; set; }
        public string LotName { get; set; }
        public long LotId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal InQty { get; set; }
        public decimal OutQty { get; set; }
        public decimal BalanceQty { get; set; }
        public string Unit { get; set; }
        public decimal NetWeight { get; set; }
        public DateTime ToDate { get; set; }
        public int Aging => Convert.ToInt32(ToDate.Subtract(Date).TotalDays); 
        public int? Expiring => ExpirationDate.HasValue ? Convert.ToInt32(ExpirationDate.Value.Subtract(ToDate).TotalDays) : (int?) null;

        public List<ItemPropertySummary> Properties { get; set; }
        public string ItemGroup { get; set; }
    }

    public class BatchNoBalanceReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<BatchNoBalanceOutput> Items { get; set; }
    }

}
