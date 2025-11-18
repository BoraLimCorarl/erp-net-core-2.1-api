using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates.Dto
{
    public class ReportTemplateOutput
    {
        public long? Id { get; set; }
        public bool TemplatePermission { get; set; }
        public string TemplateName { get; set; }
        public string HeaderTitle { get; set; }
        public TemplateType TemplateType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string Sortby { get; set; }
        public string Groupby { get; set; }
        public string DefaultTemplateReport { get; set; }
        public string DefaultTemplateReport2 { get; set; }
        public string DefaultTemplateReport3 { get; set; }
        public List<ReportColumnTemplateOutput> Columns { get; set; }
        public List<ReportFilterTemplateOutput> Filters { get; set; }
        public List<CreateOrUpdateMemberGroupItemTamplate> MemberGroupItemTamplates { get; set; }
        public PermissionReadWrite? PermissionReadWrite { get; set; }
    }


    [AutoMapFrom(typeof(ReportFilterTemplate))]
    public class ReportFilterTemplateOutput
    {
        public Guid Id { get; set; }
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

    [AutoMapFrom(typeof(ReportColumnTemplate))]
    public class ReportColumnTemplateOutput
    {
        public Guid Id { get; set; }
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public int SortOrder { get; set; }
        public bool Visible { get; set; }
        public bool IsActive { get; set; }
        public bool AllowGroupby { get; set; }
        public decimal ColumnLength { get; set; }
        public ColumnType ColumnType { get; set; }
        public string AllowFunction { get; set; }
        public bool DisableDefault { get; set; }
        public bool IsDisplay { get; set; }
    }
}
