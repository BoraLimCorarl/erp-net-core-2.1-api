using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates.Dto
{
    public class CreateTemplate
    {
        public ReportType ReportType { get; set; }
        public string TemplateName { get; set; }
        public TemplateType TemplateType { get; set; }
        public ReportCategory reportCategory { get; set; }
        public string HeaderTitle { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string Sortby { get; set; }
        public string Groupby { get; set; }
        public string DefaultTemplateReport { get; set; }
        public string DefaultTemplateReport2 { get; set; }
        public string DefaultTemplateReport3 { get; set; }
        public List<GetFilterTemplateOutput> Filters { get; set; }
        public List<GetColumnTemplateOutput> Columns { get; set; }

        public List<CreateOrUpdateMemberGroupItemTamplate> MemberGroupItemTamplate { get; set; }

        public PermissionReadWrite? PermissionReadWrite { get; set; }

    }

    public class UpdateTemplate : CreateTemplate
    {
        public long Id { get; set; }
    }

    public class ReportFilters
    {
        public string FilterName { get; set; }
        public string FilterValue { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public bool Visible { get; set; }

    }
    public class GetFilterTemplateOutput
    {
        public Guid? FilterId { get; set; }
        public string FilterName { get; set; }
        public string FilterValue { get; set; }
        public bool IsActive { get; set; }
        public bool Visible { get; set; }
        public int SortOrder { get; set; }
        public ColumnType FilterType { get; set; }
        public string DefaultValueId { get; set; }
        public bool AllowShowHideFilter { get; set; }
        public bool ShowHideFilter { get; set; }
    }

    public class GetColumnTemplateOutput
    {
        public Guid? ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public int SortOrder { get; set; }
        public bool Visible { get; set; }
        public bool IsActive { get; set; }
        public bool AllowGroupby { get; set; }
        public bool DisableDefault { get; set; }
        public bool IsDisplay { get; set; }
        public bool AllowFilter { get; set; }
        public decimal ColumnLength { get; set; }
        public ColumnType ColumnType { get; set; }
        public string AllowFunction { get; set; }
    }


    public class GetReportTemplateOutput
    {
        public long Id { get; set; }
        public ReportCategory ReportCategory { get; set; }
        public string TemplateName { get; set; }
        public TemplateType TemplateType { get; set; }
        public string Icon { get; set; }
        public string RoutePath { get; set; }
        public string BackGroundColor { get; set; }
        public PermissionReadWrite? PermissionReadWrite { get; set; }

    }
}
