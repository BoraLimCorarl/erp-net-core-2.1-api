using CorarlERP.ReportTemplates;
using CorarlERP.ReportTemplates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Reports.Dto
{
    public class ReportOutput
    {
        public string TemplateName { get; set; }
        public TemplateType TemplateType { get; set; }
        public string HeaderTitle { get; set; }
        public string Sortby { get; set; }
        public string Groupby { get; set; }
        public PermissionReadWrite PermissionReadWrite { get; set; }
        public List<CollumnOutput> ColumnInfo { get; set; }
        public List<CreateOrUpdateMemberGroupItemTamplate> MemberGroupItemTamplate { get; set; }

        public DefaultSaveTemplateOutput DefaultTemplate { get; set; }
        public DefaultSaveTemplateOutput DefaultTemplate2 { get; set; }
        public DefaultSaveTemplateOutput DefaultTemplate3 { get; set; }
    }

    public class ReportDefaultTemplate
    {
        public string TemplateName { get; set; }
        public int SortOrder { get; set; }
        public ReportType Type { get; set; }
        public string PermissionNames { get; set; }
        public string Icon { get; set; }
        public string RoutePath { get; set; }
        
    }

    public class AllReport
    {
        public string TemplateName { get; set; }
        public string PermissionNames { get; set; }
        public string Icon { get; set; }
        public string BackGroundColor { get; set; }
        public int Id { get; set; }
        public ReportCategory ReportCategory { get; set; }
        public List<ReportDefaultTemplate> Items { get; set; }
    }
    public class CollumnOutput
    {
        public Guid? Id { get; set; }
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public int SortOrder { get; set; }
        public bool Visible { get; set; }//for default check box outside Frontend
        public bool AllowGroupby { get; set; }
        public bool AllowFilter { get; set; }
        public decimal ColumnLength { get; set; }
        public ColumnType ColumnType { get; set; }
        public string AllowFunction { get; set; }
        public List<MoreFunction> MoreFunction { get; set; }
        public string DefaultValue { get; set; }
        public bool IsDisplay { get; set; } // For Collumn only 
        public bool DisableDefault { get; set; }

        public bool AllowShowHideFilter { get; set; }
        public bool ShowHideFilter { get; set; }
        public bool IsRequired { get; set; }
        public List<GroupHeaderList> GroupHeader { get; set; }
    }

    public class GroupHeaderList: CollumnOutput
    {
        
    }

    public class DefaultSaveTemplateOutput
    {
        public string ColumnName { get; set; }
        public string ColumnTitle { get; set; }
        public ReportType DefaultValue { get; set; }
        
    }

    public class MoreFunction
    {
        public string KeyName { get; set; }
    }
    
}
