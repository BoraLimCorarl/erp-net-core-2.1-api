using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ReportTemplates
{
    //Base on Corarl HR report 
    [Table("CarlErpReportFilterTemplate")]
    public class ReportFilterTemplate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public const int FilterNameLength = 250;
        

        public string FilterName { get; private set; }

        //FilterValue examples
        //1. DateRange: Monthly;01/01/2017;31/01/2017
        //2. Site: 1;2;3;4 (multiple Id)
        //3. Employees: 4534543DFD;SFDDFr3423;SEWRWER2323423;SERREW33242342;
        public string FilterValue { get; private set; }
        public bool IsActive { get; private set; }
        public int SortOrder { get; private set; }
        public bool Visible { get; private set; }

        public ColumnType FilterType { get; private set; }
        public string DefaultValueId { get; private set; }

        public long ReportTemplateId { get; private set; }
        public ReportTemplate ReportTemplate { get; private set; }

        public bool AllowShowHideFilter { get; set; }
        public bool ShowHideFilter { get; set; }

        private ReportFilterTemplate() { }

        public static ReportFilterTemplate Create(
                                            int? tenantId,
                                            long? creatorUserId,
                                            string filterName,
                                            string filterValue,
                                            bool visible,
                                            int sortOrder,
                                            ColumnType filterType,
                                            string defaultValueId,
                                            bool allowShowHideFilter,
                                            bool showHideFilter)
        {
            return new ReportFilterTemplate()
            {

                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                FilterName = filterName,
                IsActive = true,
                FilterValue = filterValue,
                Visible = visible,
                SortOrder = sortOrder,
                FilterType = filterType,
                DefaultValueId = defaultValueId,
                AllowShowHideFilter= allowShowHideFilter,
                ShowHideFilter = showHideFilter
            };
        }

        public static ReportFilterTemplate Create(
                                           int? tenantId,
                                           long creatorUserId,
                                           string filterName,
                                           string filterValue,
                                           bool visible,
                                           int sortOrder,
                                           long reportTemplateId,
                                           ColumnType filterType,
                                           string defaultValueId,
                                           bool allowShowHideFilter,
                                           bool showHideFilter)
        {
            return new ReportFilterTemplate()
            {

                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                FilterName = filterName,
                IsActive = true,
                FilterValue = filterValue,
                Visible = visible,
                SortOrder = sortOrder,
                ReportTemplateId = reportTemplateId,
                FilterType = filterType,
                DefaultValueId = defaultValueId,
                AllowShowHideFilter = allowShowHideFilter,
                ShowHideFilter = showHideFilter
            };
        }

        public void Update(long lastModifiedUserId,
                           string filterName,
                           string filterValue,
                           bool visible,
                           int sortOrder,
                           ColumnType filterType,
                           string defaultValueId,
                           bool allowShowHideFilter,
                           bool showHideFilter)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.FilterName = filterName;
            this.FilterValue = filterValue;
            this.Visible = visible;
            this.SortOrder = sortOrder;
            this.FilterType = filterType;
            this.DefaultValueId = defaultValueId;
            this.AllowShowHideFilter = allowShowHideFilter;
            this.ShowHideFilter = showHideFilter;
        }

        public void Enable()
        {
            this.IsActive = true;
        }

        public void Disable()
        {
            this.IsActive = false;
        }
    }
}
