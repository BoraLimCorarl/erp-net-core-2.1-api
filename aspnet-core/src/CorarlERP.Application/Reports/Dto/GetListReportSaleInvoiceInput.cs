using Abp.Runtime.Validation;
using CorarlERP.Dto;
using CorarlERP.ReportTemplates;
using System;
using System.Collections.Generic;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public enum OrderApply { 
        All = 0,
        Applied = 1,
        NotApply = 2
    }

    public class GetListReportSaleInvoiceInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        public BillType BillTypes { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<long> AccountTypes { get; set; }
        public List<long> SaleTypes { get; set; }
        public List<JournalType> JournalTypes { get; set; }
        public CurrencyFilter CurrencyFilter { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }
        public OrderApply OrderApply { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetSaleInvoiceReportInput : GetListReportSaleInvoiceInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetSaleInvoiceDetailReportInput : GetSaleInvoiceReportInput
    {

    }


    public class GetListSaleInvoiceReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListSaleInvoiceReportOutput> Items { get; set; }
    }

    public class GetListSaleInvoiceDetailReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListSaleInvoiceDetailReportOutput> Items { get; set; }
    }


    public class GetListSaleReturnReportGroupByOutput
    {
        public string KeyName { get; set; }
        public List<GetListSaleReturnReportOutput> Items { get; set; }
    }


    public class GetListReportSaleReturnInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<Guid> Accounts { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        public BillType BillTypes { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<long> AccountTypes { get; set; }
        public CurrencyFilter CurrencyFilter { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetSaleReturnReportInput : GetListReportSaleReturnInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

    public class GetListReportSaleInvoiceByItemPropertyInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public bool IsLoadMore { get; set; }
        public List<string> ColumnNamesToSum { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<Guid?> Items { get; set; }
        public List<Guid> Customers { get; set; }
        public List<long> CustomerTypes { get; set; }
        public List<long> Locations { get; set; }
        public List<long?> Users { get; set; }
        //public BillType BillTypes { get; set; }
        public bool UsePagination { get; set; }
        public string GroupBy { get; set; }
        public List<long> SaleTypes { get; set; }
        public List<GetListPropertyFilter> PropertyDics { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Date.Date";
            }
        }
    }
    public class GetSaleInvoiceByItemPropertyReportInput : GetListReportSaleInvoiceByItemPropertyInput
    {
        public ReportOutput ReportOutput { get; set; }
    }

}
