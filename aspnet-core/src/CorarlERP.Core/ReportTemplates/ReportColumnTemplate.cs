using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.ReportTemplates
{
    [Table("CarlErpReportColumnTemplate")]
    public class ReportColumnTemplate : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public const int ColumnTitleLength = 512;
        public const int ColumnNameLength = 250;
        public const int AllowFunctionLength = 256;

        public string ColumnName { get; private set; }
        public string ColumnTitle { get; private set; }
        public int SortOrder { get; private set; }
        public bool Visible { get; private set; }
        public bool IsDisplay { get; private set; }
        public bool IsActive { get; private set; }
        public bool AllowGroupby { get; private set; }
        public bool AllowFilter { get; private set; }
        public decimal ColumnLength { get; private set; }
        public ColumnType ColumnType { get; private set; }
        public string AllowFunction { get; private set; }
        public bool DisableDefault { get; private set; }
        public long ReportTemplateId { get; private set; }
        public ReportTemplate ReportTemplate { get; private set; }

        public void SetIsDisplay(bool isDisplay) { IsDisplay = isDisplay; }

        private ReportColumnTemplate() { }

        public static ReportColumnTemplate Create(
                                            int? tenantId,
                                            long? creatorUserId,
                                            string columnName,
                                            string columnTitle,
                                            decimal columnLength,
                                            ColumnType columnType,
                                            int sortOrder,
                                            bool visible,
                                            bool allowGroupby,
                                            bool allowFilter,
                                            string allowFunction,
                                            bool disableDefault,
                                            bool isDisplay
            )
        {
            return new ReportColumnTemplate()
            {
                DisableDefault = disableDefault,
                IsDisplay = isDisplay,
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ColumnName = columnName,
                ColumnTitle = columnTitle,
                ColumnLength = columnLength,
                ColumnType = columnType,
                IsActive = true,
                SortOrder = sortOrder,
                Visible = visible,
                AllowGroupby = allowGroupby,
                AllowFilter = allowFilter,
                AllowFunction = allowFunction
            };
        }

        public static ReportColumnTemplate Create(
                                            int? tenantId,
                                            long creatorUserId,
                                            string columnName,
                                            string columnTitle,
                                            decimal columnLength,
                                            ColumnType columnType,
                                            int sortOrder,
                                            bool visible,
                                            bool allowGroupby,
                                            string allowFunction,
                                            bool allowFilter,
                                            long reportTemplateId,
                                            bool disableDefault,
                                            bool isDisplay)
        {
            return new ReportColumnTemplate()
            {
                DisableDefault = disableDefault,
                IsDisplay = isDisplay,
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ColumnName = columnName,
                ColumnTitle = columnTitle,
                ColumnLength = columnLength,
                ColumnType = columnType,
                IsActive = true,
                SortOrder = sortOrder,
                Visible = visible,
                AllowGroupby = allowGroupby,
                AllowFunction = allowFunction,
                AllowFilter = allowFilter,
                ReportTemplateId = reportTemplateId
            };
        }

        public void Update(long lastModifiedUserId,
                           string columnName,
                           string columnTitle,
                           decimal columnLength,
                           ColumnType columnType,
                           int sortOrder,
                           bool visible,
                           bool allowGroupby,
                           bool allowFilter,
                           string allowFunction,
                           bool disableDefault,
                           bool isDisplay)
        {
            this.DisableDefault = disableDefault;
            this.IsDisplay = isDisplay;
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.ColumnName = columnName;
            this.ColumnTitle = columnTitle;
            this.ColumnLength = columnLength;
            this.ColumnType = columnType;
            this.SortOrder = sortOrder;
            this.Visible = visible;
            this.AllowGroupby = allowGroupby;
            this.AllowFilter = allowFilter; 
            this.AllowFunction = allowFunction;

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

    public enum ColumnType
    {
        Date = 1,
        Number = 2,
        Money = 3, // use this default of general total rouding digit
        String = 4,
        Object = 5,
        StatusCode = 6,
        Language = 7,
        AutoNumber = 8,
        Array = 9,
        Bool = 10,
        Group = 11,
        RoundingDigit = 12, // use this default of unit cost rouding digit
        ItemProperty = 13, // for dynamic filter of property
        Percentage = 14
    }

    public enum BillType
    {
        SelectAll = 0,
        Item = 1,
        Account = 2
    }

}
