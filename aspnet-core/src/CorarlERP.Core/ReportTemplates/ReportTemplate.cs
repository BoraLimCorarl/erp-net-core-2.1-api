using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates
{
    [Table("CarlErpReportTemplate")]
    public class ReportTemplate : AuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public ReportType ReportType { get; private set; }//mean for sub item categories
        public ReportCategory ReportCategory { get; private set; }//For categories 
        public const string DefaultName = "Default";

        public const int TemplateNameLength = 250;
        public const int HeaderTitleLength = 512;
        public const int SortbyLength = 250;
        public const int GroupbyLength = 250;
        public TemplateType TemplateType { get; private set; }//Property of showing public or only me 
        public string TemplateName { get; private set; }
        public string HeaderTitle { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
        public string Sortby { get; private set; }
        public string Groupby { get; private set; }

        public string DefaultTemplateReport { get; private set; } // first default template 

        public string DefaultTemplateReport2 { get; private set; } // second defualt template
        public string DefaultTemplateReport3 { get; private set; } // third defualt template

        public PermissionReadWrite? PermissionReadWrite { get; set; }

        public virtual ICollection<ReportFilterTemplate> Filters { get; protected set; }
        public virtual ICollection<ReportColumnTemplate> Columns { get; protected set; }


        public static ReportTemplate Create(int tenantId,
                                            long? creatorUserId,
                                            ReportType reportType,
                                            string templateName,
                                            TemplateType templateType,
                                            ReportCategory reportCategory,
                                            string headerTitle,
                                            string sortBy,
                                            string groupby,
                                            bool isDefault,
                                            string defaultTemplateReport,
                                            List<ReportFilterTemplate> filters,
                                            List<ReportColumnTemplate> columns,
                                            PermissionReadWrite? permissionReadWrite,
                                            string defaultTemplateReport2,
                                            string defaultTemplateReport3)
        {
            return new ReportTemplate()
            {
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                ReportType = reportType,
                ReportCategory = reportCategory,
                TemplateName = templateName,
                HeaderTitle = headerTitle,
                Sortby = sortBy,
                IsDefault = isDefault,
                TemplateType = templateType,
                IsActive = true,
                Filters = filters,
                Groupby = groupby,
                Columns = columns,
                DefaultTemplateReport = defaultTemplateReport,
                PermissionReadWrite = permissionReadWrite,
                DefaultTemplateReport2 = defaultTemplateReport2,
                DefaultTemplateReport3 = defaultTemplateReport3
            };
        }
        
        public void Update(
            long lastModifiedUserId, 
            //ReportType reportType, 
            string templateName,
            TemplateType templateType,
            //ReportCategory reportCategory,
            string headerTitle,
            string sortBy,
            string groupBy,
            string defaultTemplateReport,
            PermissionReadWrite? permissionReadWrite,
            string defaultTemplateReport2,
            string defaultTemplateReport3)
        {
            this.LastModifierUserId = lastModifiedUserId;
            this.LastModificationTime = Clock.Now;
            this.TemplateName = templateName;
            this.TemplateType = templateType;
            this.HeaderTitle = headerTitle;
            //this.ReportCategory = reportCategory;
            //this.ReportType = reportType;
            this.Sortby = sortBy;
            this.DefaultTemplateReport = defaultTemplateReport;
            this.Groupby = groupBy;
            this.PermissionReadWrite = permissionReadWrite;
            this.DefaultTemplateReport2 = defaultTemplateReport2;
            this.DefaultTemplateReport3 = defaultTemplateReport3;
        }

        public void Enable()
        {
            this.IsActive = true;
        }

        public void Disable()
        {
            this.IsActive = false;
        }

        #region Filters
        public void AddFilter(long creatorUserId, string filterName, 
                string filterValue, 
                bool visible, 
                int sortOrder,
                ColumnType filterType,
                string defaultValueId,
                bool allowShowHideFilter,
                bool showHideFilter)
        {
            this.Filters.Add(ReportFilterTemplate.Create(this.TenantId, 
                creatorUserId, filterName, filterValue, visible, sortOrder, filterType, defaultValueId,allowShowHideFilter,showHideFilter));
        }

        public ReportFilterTemplate RemoveFilter(Guid filterId)
        {
            var filter = this.Filters.FirstOrDefault(u => u.Id == filterId);
            if (filter != null) this.Filters.Remove(filter);

            return filter;
        }

        public void UpdateFilter(Guid filterId, 
            long modifiedUserId, string filterName, 
            string filterValue,
            bool visible, int sortOrder,
            ColumnType filterType,
            string defaultValueId,
            bool allowShowHideFilter,
            bool showHideFilter)
        {
            var filter = this.Filters.FirstOrDefault(u => u.Id == filterId);
            if (filter != null) filter.Update(modifiedUserId, filterName, filterValue, visible, sortOrder, filterType, defaultValueId, allowShowHideFilter,showHideFilter);
        }

        #endregion

        //#region Groupbys
        //public void AddGroupby(long groupbyId, long creatorUserId, string groupbyName, string groupbyTitle, bool none, bool selected)
        //{
        //    this.Groupbys.Add(ReportGroupbyTemplate.Create(groupbyId, this.TenantId, creatorUserId, groupbyName, groupbyTitle, none, selected));
        //}

        //public ReportGroupbyTemplate RemoveGroupby(long groupbyId)
        //{
        //    var groupby = this.Groupbys.FirstOrDefault(u => u.Id == groupbyId);
        //    if (groupby != null) this.Groupbys.Remove(groupby);

        //    return groupby;
        //}

        //public void UpdateGroupby(long filterId, long modifiedUserId, string groupbyName, string groupbyTitle, bool none, bool selected)
        //{
        //    var groupby = this.Groupbys.FirstOrDefault(u => u.Id == filterId);
        //    if (groupby != null) groupby.Update(modifiedUserId, groupbyName, groupbyTitle, none, selected);
        //}
        //#endregion

        #region Columns
        public void AddColumn(long creatorUserId, string columnName, 
            string columnTitle, int columnLength, ColumnType columnType,
            int sortOrder, bool visible, 
            bool allowGroupby, bool allowFilter, string translateKey, 
            bool disableDefault, bool isDisplay)
        {
            this.Columns.Add(ReportColumnTemplate.Create(this.TenantId, creatorUserId, columnName, 
                columnTitle, columnLength, columnType, sortOrder, visible, 
                allowGroupby,allowFilter, translateKey, disableDefault, isDisplay));
        }

        public ReportColumnTemplate RemoveColumn(Guid columnId)
        {
            var column = this.Columns.FirstOrDefault(u => u.Id == columnId);
            if (column != null) this.Columns.Remove(column);

            return column;
        }

        public void UpdateColumn(Guid filterId, long modifiedUserId, string columnName, string columnTitle,
            int columnLength, ColumnType columnType, int sortOrder, bool visible,
            bool allowGroupby, bool allowFilter, string translateKey, bool disableDefault, bool isDisplay)
        {
            var column = this.Columns.FirstOrDefault(u => u.Id == filterId);
            if (column != null) column.Update(modifiedUserId, columnName, columnTitle, columnLength, 
                columnType, sortOrder, visible, allowGroupby, allowFilter, translateKey, disableDefault, isDisplay);
        }
        #endregion



    }

    public enum ReportPeriod
    {
        Monthly = 1,
        Quarterly = 2,
        Semesterly = 3,
        Annually = 4
    }

    public enum ReportType
    {
        ReportType_Journal = 1,
        ReportType_Inventory = 2,
        ReportType_BalanceSheet = 3,
        ReportType_ProfitAndLoss = 4,
        ReportType_Purchasing = 5,
        ReportType_SaleInvoice = 6,
        ReportType_InventoryDetail = 7,
        ReportType_Ledger = 8,
        ReportType_CustomerAging = 9,
        ReportType_CustomerByInvoice = 10,
        ReportType_VendorAging = 11,
        ReportType_VendorByBill = 12,
        ReportType_InventoryTransaction = 13,
        ReportType_StockBalance = 14,
        ReportType_SaleReturn = 15,
        ReportType_Cash = 16,
        ReportType_SaleInviceDetail = 17,
        ReportType_CashFlow = 18,

        ReportType_AssetBalance = 19,
        ReportType_AssetItemDetail = 20,

        ReportType_PurchaseOrderSummary = 21,
        ReportType_PurchaseOrderBillDetail = 22,

        ReportType_SaleOrderSummary = 23,
        ReportType_SaleOrderInvoiceDetail = 24,
        ReportType_Traceability = 25,
        ReportType_BatchNoBalance = 26,
        ReportType_ProductionPlan = 27,
        ReportType_ARByInvoiceWithPayment = 28,
        ReportType_ProductionOrder = 29,
        ReportType_SaleOrderDetail = 30,
        ReportType_ProfitByInvoice = 31,        
        ReportType_DirectCashFlow = 32,
        ReportType_DriectCashFlowTemplate = 33,
        ReportType_TrialBalance = 34,
        ReportType_DeliveryScheduleSummary = 35,
        ReportType_DeliveryScheduleDetail = 36,
        ReportType_SaleOrderByItemProperty = 37,
        ReportType_DeliveryScheduleByItemProperty = 38,
        ReportType_SaleByItemProperty = 39,
        ReportType_PurchaseByItemProperty = 40,
        ReportType_QCTest = 41,
    }

    public enum ReportCategory
    {
        VendorReport = 1,
        CustomerReport = 2,
        InventoryReport = 3,
        AccountingReport = 4,
        PurchaseOrderReport = 5,
        SaleOrderReport = 6,
        ProductionReport = 7,
        DeliveryScheduleReport = 8,
        QCTestReport = 9
    }
}
