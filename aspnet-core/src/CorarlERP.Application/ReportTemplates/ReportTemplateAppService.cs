using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Authorization;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates.Dto;
using CorarlERP.UserGroups;
using CorarlERP.UserGroups.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.ReportTemplates
{

    public class ReportTemplateAppService : CorarlERPAppServiceBase, IReportTemplateAppService
    {
        private readonly IReportTemplateManager _reportTemplateManager;
        private readonly IRepository<ReportTemplate, long> _reportTemplateRepository;

        private readonly IGroupMemberItemTamplateManager _groupMemberItemTamplateManager;
        private readonly IRepository<GroupMemberItemTamplate, Guid> _groupMemberItemTamplateRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;

        public ReportTemplateAppService(IReportTemplateManager reportTemplateManager,
                             IRepository<ReportTemplate, long> reportTemplateRepository,
                             IGroupMemberItemTamplateManager groupMemberItemTamplateManager,
                             IRepository<GroupMemberItemTamplate, Guid> groupMemberItemTamplateRepository,
                             IRepository<UserGroupMember, Guid> userGroupMemberRepository)
        {
            _reportTemplateManager = reportTemplateManager;
            _reportTemplateRepository = reportTemplateRepository;
            _groupMemberItemTamplateManager = groupMemberItemTamplateManager;
            _groupMemberItemTamplateRepository = groupMemberItemTamplateRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Template_Create)]
        public async Task<ReportTemplateOutput> Create(CreateTemplate input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var filters = input.Filters.Select(s =>
                        ReportFilterTemplate.Create(tenantId, userId, s.FilterName,
                                s.FilterValue, s.Visible, s.SortOrder, s.FilterType, s.DefaultValueId, s.AllowShowHideFilter, s.ShowHideFilter)).ToList();

            var columns = input.Columns.Select(s =>
                            ReportColumnTemplate.Create(tenantId, userId,
                                s.ColumnName,
                                s.ColumnTitle,
                                s.ColumnLength,
                                s.ColumnType,
                                s.SortOrder,
                                s.Visible,
                                s.AllowGroupby,
                                s.AllowFilter,
                                s.AllowFunction,
                                s.DisableDefault,
                                s.IsDisplay)
                            ).ToList();

            var @entity = ReportTemplate.Create(tenantId, userId, input.ReportType,
                            input.TemplateName, input.TemplateType, input.reportCategory,
                            input.HeaderTitle, input.Sortby,
                            input.Groupby, input.IsDefault, input.DefaultTemplateReport, filters, columns,
                            input.PermissionReadWrite, input.DefaultTemplateReport2, input.DefaultTemplateReport3);

            //Add User Group item 

            if ((input.TemplateType == TemplateType.User || input.TemplateType == TemplateType.Group) && input.MemberGroupItemTamplate.Count() <= 0)
            {
                throw new UserFriendlyException(L("PleaseAddItem"));
            }
            foreach (var i in input.MemberGroupItemTamplate)
            {
                var @MemberGroupItemTamplate = GroupMemberItemTamplate.Create(tenantId, userId, i.UserGroupId, i.MemberUserId, i.PermissionReadWrite, entity);
                i.Id = MemberGroupItemTamplate.Id;
                CheckErrors(await _groupMemberItemTamplateManager.CreateAsync(@MemberGroupItemTamplate));
            }

            CheckErrors(await _reportTemplateManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();

            var outPut = new ReportTemplateOutput
            {
                Id = entity.Id,
                Columns = @entity.Columns.Select(t => new ReportColumnTemplateOutput
                {
                    AllowFunction = t.AllowFunction,
                    AllowGroupby = t.AllowGroupby,
                    ColumnLength = t.ColumnLength,
                    ColumnName = t.ColumnName,
                    ColumnTitle = t.ColumnTitle,
                    ColumnType = t.ColumnType,
                    DisableDefault = t.DisableDefault,
                    Id = t.Id,
                    IsActive = t.IsActive,
                    IsDisplay = t.IsDisplay,
                    SortOrder = t.SortOrder,
                    Visible = t.Visible,
                }).ToList(),
                IsActive = entity.IsActive,
                DefaultTemplateReport = @entity.DefaultTemplateReport,
                DefaultTemplateReport2 = @entity.DefaultTemplateReport2,
                DefaultTemplateReport3 = @entity.DefaultTemplateReport3,
                Filters = @entity.Filters.Select(t => new ReportFilterTemplateOutput
                {
                    Id = t.Id,
                    DefaultValueId = t.DefaultValueId,
                    FilterName = t.FilterName,
                    FilterType = t.FilterType,
                    FilterValue = t.FilterValue,
                    IsActive = t.IsActive,
                    SortOrder = t.SortOrder,
                    Visible = t.Visible,
                    AllowShowHideFilter = t.AllowShowHideFilter,
                    ShowHideFilter = t.ShowHideFilter,
                }).ToList(),
                Groupby = @entity.Groupby,
                HeaderTitle = @entity.HeaderTitle,
                IsDefault = @entity.IsDefault,
                MemberGroupItemTamplates = input.MemberGroupItemTamplate,
                PermissionReadWrite = @entity.PermissionReadWrite,
                Sortby = @entity.Sortby,
                TemplateName = @entity.TemplateName,
                TemplatePermission = true,
                TemplateType = @entity.TemplateType,

            };

            return outPut;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Template_Update)]
        public async Task<ReportTemplateOutput> Update(UpdateTemplate input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _reportTemplateManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(
                userId,
                input.TemplateName,
                input.TemplateType,
                input.HeaderTitle,
                input.Sortby,
                input.Groupby,
                input.DefaultTemplateReport,
                input.PermissionReadWrite,
                input.DefaultTemplateReport2, input.DefaultTemplateReport3);

            foreach (var t in input.Filters)
            {
                var item = entity.Filters.FirstOrDefault(s => s.Id == t.FilterId);
                if (item != null)
                {
                    item.Update(userId, t.FilterName, t.FilterValue, t.Visible, t.SortOrder, t.FilterType, t.DefaultValueId, t.AllowShowHideFilter, t.ShowHideFilter);
                }
                else
                {
                    var createFilter = ReportFilterTemplate.Create(tenantId, userId,
                            t.FilterName, t.FilterValue, t.Visible, t.SortOrder, entity.Id,
                            t.FilterType, t.DefaultValueId, t.AllowShowHideFilter, t.ShowHideFilter);
                    t.FilterId = createFilter.Id;
                    CheckErrors(await _reportTemplateManager.CreateFilterAsync(createFilter));
                }
            }

            foreach (var t in input.Columns)
            {
                var columnItem = entity.Columns.FirstOrDefault(s => s.Id == t.ColumnId);
                if (columnItem != null)
                {
                    columnItem.Update(userId, t.ColumnName, t.ColumnTitle,
                        t.ColumnLength,
                        t.ColumnType,
                        t.SortOrder,
                        t.Visible,
                        t.AllowGroupby,
                        t.AllowFilter,
                        t.AllowFunction,
                        t.DisableDefault,
                        t.IsDisplay);
                }
                else
                {
                    var createCols = ReportColumnTemplate.Create(tenantId, userId,
                        t.ColumnName,
                        t.ColumnTitle,
                        t.ColumnLength,
                        t.ColumnType,
                        t.SortOrder,
                        t.Visible,
                        t.AllowGroupby,
                        t.AllowFunction,
                        t.AllowFilter,
                        entity.Id,
                        t.DisableDefault,
                         t.IsDisplay);
                    t.ColumnId = createCols.Id;
                    CheckErrors(await _reportTemplateManager.CreateColumnAsync(createCols));
                }


            }

            //Add User Group Item and Update 
            var userGroupMembers = await _groupMemberItemTamplateRepository.GetAll().Where(u => u.ReportTemplateId == entity.Id).ToListAsync();
            foreach (var c in input.MemberGroupItemTamplate)
            {
                if (c.Id != null)
                {
                    var userGroupMember = userGroupMembers.FirstOrDefault(u => u.Id == c.Id);
                    if (userGroupMember != null)
                    {
                        userGroupMember.Update(userId, c.UserGroupId, c.MemberUserId, c.PermissionReadWrite);
                        CheckErrors(await _groupMemberItemTamplateManager.UpdateAsync(userGroupMember));
                    }
                }
                else if (c.Id == null)
                {
                    var userGroupMember = GroupMemberItemTamplate.Create(tenantId, userId, c.UserGroupId, c.MemberUserId, c.PermissionReadWrite, entity.Id);
                    c.Id = userGroupMember.Id;
                    CheckErrors(await _groupMemberItemTamplateManager.CreateAsync(userGroupMember));

                }
            }

            var toDeleteUserGroupMember = userGroupMembers.Where(u => !input.MemberGroupItemTamplate.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteUserGroupMember)
            {
                CheckErrors(await _groupMemberItemTamplateManager.RemoveAsync(t));
            }

            CheckErrors(await _reportTemplateManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            //return new NullableIdDto<long>() { Id = entity.Id };

            var outPut = new ReportTemplateOutput
            {
                Id = input.Id,
                Columns = input.Columns.Select(t => new ReportColumnTemplateOutput
                {
                    AllowFunction = t.AllowFunction,
                    AllowGroupby = t.AllowGroupby,
                    ColumnLength = t.ColumnLength,
                    ColumnName = t.ColumnName,
                    ColumnTitle = t.ColumnTitle,
                    ColumnType = t.ColumnType,
                    DisableDefault = t.DisableDefault,
                    Id = t.ColumnId.Value,
                    IsActive = t.IsActive,
                    IsDisplay = t.IsDisplay,
                    SortOrder = t.SortOrder,
                    Visible = t.Visible,
                }).ToList(),
                IsActive = input.IsActive,
                DefaultTemplateReport = input.DefaultTemplateReport,
                DefaultTemplateReport2 = input.DefaultTemplateReport2,
                DefaultTemplateReport3 = input.DefaultTemplateReport3,
                Filters = input.Filters.Select(t => new ReportFilterTemplateOutput
                {
                    Id = t.FilterId.Value,
                    DefaultValueId = t.DefaultValueId,
                    FilterName = t.FilterName,
                    FilterType = t.FilterType,
                    FilterValue = t.FilterValue,
                    IsActive = t.IsActive,
                    SortOrder = t.SortOrder,
                    Visible = t.Visible,
                    AllowShowHideFilter = t.AllowShowHideFilter,
                    ShowHideFilter = t.ShowHideFilter,
                }).ToList(),
                Groupby = input.Groupby,
                HeaderTitle = input.HeaderTitle,
                IsDefault = input.IsDefault,
                MemberGroupItemTamplates = input.MemberGroupItemTamplate,
                PermissionReadWrite = input.PermissionReadWrite,
                Sortby = input.Sortby,
                TemplateName = input.TemplateName,
                TemplatePermission = true,
                TemplateType = input.TemplateType,
            };

            return outPut;
        }

        public async Task<ReportTemplateOutput> GetDetail(EntityDto<long> input)
        {
            var userId = AbpSession.GetUserId();
            var userPermission = await GetUserPermissions(userId);
            bool IsUserPermission = userPermission.Contains(AppPermissions.Pages_Tenant_Report_CanEditAllTemplate);

            var userGroups = await (from userGroup in _groupMemberItemTamplateRepository.GetAll()
                                    .Include(u => u.UserGroup)
                                    .AsNoTracking()
                                    .Where(u => u.ReportTemplateId == input.Id)
                                    join userMemberItem in _userGroupMemberRepository.GetAll().AsNoTracking()
                                    .Where(t => t.MemberId == userId)
                                    on userGroup.UserGroupId equals userMemberItem.UserGroupId
                                    orderby userGroup.PermissionReadWrite descending
                                    select new
                                    {
                                        MemberId = userMemberItem.MemberId,
                                        ReportTemplateId = userGroup.ReportTemplateId,
                                        PermissionReadWrite = userGroup.PermissionReadWrite,
                                    }).FirstOrDefaultAsync();

            var results = await (from template in _reportTemplateRepository.GetAll()
                                 join usergroup in _groupMemberItemTamplateRepository.GetAll()
                                 on template.Id equals usergroup.ReportTemplateId
                                 into ps
                                 from usergroup in ps.DefaultIfEmpty()
                                 where (template.Id == input.Id)
                                 select new ReportTemplateOutput
                                 {
                                     Id = template.Id,
                                     Columns = template.Columns.Select(c => new ReportColumnTemplateOutput
                                     {
                                         AllowGroupby = c.AllowGroupby,
                                         AllowFunction = c.AllowFunction,
                                         ColumnLength = c.ColumnLength,
                                         ColumnName = c.ColumnName,
                                         ColumnTitle = c.ColumnTitle,
                                         ColumnType = c.ColumnType,
                                         Id = c.Id,
                                         IsActive = c.IsActive,
                                         SortOrder = c.SortOrder,
                                         Visible = c.Visible,
                                         IsDisplay = c.IsDisplay,
                                         DisableDefault = c.DisableDefault
                                     }).OrderBy(t => t.SortOrder).ToList(),
                                     DefaultTemplateReport = template.DefaultTemplateReport,
                                     Groupby = template.Groupby,
                                     HeaderTitle = template.HeaderTitle,
                                     IsActive = template.IsActive,
                                     IsDefault = template.IsDefault,
                                     MemberGroupItemTamplates = ps
                                         .Select(m => new CreateOrUpdateMemberGroupItemTamplate
                                         {
                                             MemberUser = ObjectMapper.Map<MemberDetail>(m.MemberUser),
                                             UserGroup = ObjectMapper.Map<UserGroupDetailOutput>(m.UserGroup),
                                             UserGroupId = m.UserGroupId,
                                             Id = m.Id,
                                             MemberUserId = m.MemberUserId,
                                             PermissionReadWrite = m.PermissionReadWrite,
                                         }).ToList(),
                                     TemplatePermission = template.TemplateType == TemplateType.Gloable && template.PermissionReadWrite == PermissionReadWrite.ReadWrite ? true
                                                        : template.TemplateType == TemplateType.User && ps != null && ps.Any(x => x.PermissionReadWrite == PermissionReadWrite.ReadWrite && x.MemberUserId == userId) == true ? true
                                                        : template.TemplateType == TemplateType.Group && userGroups != null && userGroups.PermissionReadWrite == PermissionReadWrite.ReadWrite && userGroups.MemberId == userId == true ? true
                                                        : userId == template.CreatorUserId ? true
                                                        : template.TemplateType == TemplateType.OnlyMe ? true
                                                        : IsUserPermission ? true
                                                        : false,
                                     DefaultTemplateReport2 = template.DefaultTemplateReport2,
                                     DefaultTemplateReport3 = template.DefaultTemplateReport3,
                                     PermissionReadWrite = template.PermissionReadWrite,
                                     Sortby = template.Sortby,
                                     TemplateName = template.TemplateName,
                                     TemplateType = template.TemplateType,
                                     Filters = template.Filters.Select(f => new ReportFilterTemplateOutput
                                     {
                                         DefaultValueId = f.DefaultValueId,
                                         Visible = f.Visible,
                                         FilterName = f.FilterName,
                                         FilterType = f.FilterType,
                                         FilterValue = f.FilterValue,
                                         Id = f.Id,
                                         IsActive = f.IsActive,
                                         SortOrder = f.SortOrder,
                                         AllowShowHideFilter = f.AllowShowHideFilter,
                                         ShowHideFilter = f.ShowHideFilter,
                                     }).ToList(),
                                 }).FirstOrDefaultAsync();
            return results;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ViewOriginal)]
        public async Task<List<AllReport>> GetReportTemplate()
        {
            var userId = AbpSession.GetUserId();
            var userPermission = await GetUserPermissions(userId);
            var userPermissionDict = userPermission.ToDictionary(u => u);

            var result = new List<AllReport>() {
                new AllReport()
                {
                    ReportCategory = ReportCategory.InventoryReport,
                    TemplateName = L("Inventory"),
                    Id = 1,
                    PermissionNames = AppPermissions.Pages_Tenant_Report_Inventory,
                    BackGroundColor = "#34CFDA",
                    Icon = "icon-m-inventory",
                    Items = new List<ReportDefaultTemplate>() {
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Inventory_Summary,
                            Icon = "icon-m-inventory",
                            RoutePath = "inventory-valuation-summary",
                            SortOrder = 2,
                            TemplateName = L("InventoryValuationSummaryReport"),
                            Type = ReportType.ReportType_Inventory
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Inventory_Detail,
                            Icon = "icon-m-inventory",
                            RoutePath = "inventory-valuation-detail",
                            SortOrder = 3,
                            TemplateName = L("InventoryValuationDetailReport"),
                            Type = ReportType.ReportType_InventoryDetail
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Inventory_InventoryTransaction,
                            Icon = "icon-m-inventory",
                            RoutePath = "inventory-transaction",
                            SortOrder = 4,
                            TemplateName = L("InventoryTransactionReport"),
                            Type = ReportType.ReportType_InventoryTransaction
                        },
                         new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Stock_Balance,
                            Icon = "icon-m-inventory",
                            RoutePath = "stock-balance-summary",
                            SortOrder = 5,
                            TemplateName = L("StockBalanceSummaryReport"),
                            Type = ReportType.ReportType_StockBalance
                        },

                         new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Asset_Balance,
                            Icon = "icon-m-inventory",
                            RoutePath = "asset-balance",
                            SortOrder = 6,
                            TemplateName = L("AssetBalanceReport"),
                            Type = ReportType.ReportType_AssetBalance
                        },
                         new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Asset_Balance,
                            Icon = "icon-m-inventory",
                            RoutePath = "asset-item-detail",
                            SortOrder = 7,
                            TemplateName = L("AssetItemDetailReport"),
                            Type = ReportType.ReportType_AssetItemDetail
                        },
                          new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Traceability,
                            Icon = "icon-m-inventory",
                            RoutePath = "batch-no-traceability",
                            SortOrder = 8,
                            TemplateName = L("BatchNoTraceability"),
                            Type = ReportType.ReportType_Traceability
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_BatchNoBalance,
                            Icon = "icon-m-inventory",
                            RoutePath = "batch-no-balance",
                            SortOrder = 9,
                            TemplateName = L("BatchNoBalance"),
                            Type = ReportType.ReportType_BatchNoBalance
                        },
                    }
                },

                new AllReport()
                {
                    ReportCategory = ReportCategory.VendorReport,
                    TemplateName = L("Vendor"),
                    Id = 1,
                    BackGroundColor = "#46A3EC",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_Vendor,
                    Icon = "icon-Vendor",
                    Items = new List<ReportDefaultTemplate>()
                    {
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath = "purchasing",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Purchasing,
                            SortOrder= 1,
                            TemplateName = L("PurchasingReport"),
                            Type = ReportType.ReportType_Purchasing,
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="vendor-aging",
                            PermissionNames =AppPermissions.Pages_Tenant_Report_Vendor_VendorAging,
                            SortOrder= 2,
                            TemplateName = L("VendorAgingReport"),
                            Type = ReportType.ReportType_VendorAging
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="vendor-by-bill",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Vendor_VendorByBill,
                            SortOrder= 3,
                            TemplateName = L("VendorByBillReport"),
                            Type = ReportType.ReportType_VendorByBill
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="purchase-by-item-property",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Vendor_PurchaseByItemProperty,
                            SortOrder= 4,
                            TemplateName = L("PurchaseByItemPropertyReport"),
                            Type = ReportType.ReportType_PurchaseByItemProperty
                        },
                    }
                },
                new AllReport()
                {
                    ReportCategory = ReportCategory.CustomerReport,
                    TemplateName = L("Customers"),
                    BackGroundColor = "#FE8385",
                    Id = 1,
                    PermissionNames = AppPermissions.Pages_Tenant_Report_Customer,
                    Icon = "icon-m-customer",
                    Items = new List<ReportDefaultTemplate>() {
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="sale-invoice",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleInvoice,
                            SortOrder= 1,
                            TemplateName = L("SaleInvoiceReport"),
                            Type = ReportType.ReportType_SaleInvoice
                        },
                         new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="sale-invoice-detail",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleInvoiceDetail,
                            SortOrder= 5,
                            TemplateName = L("SaleInvoiceDetail"),
                            Type = ReportType.ReportType_SaleInviceDetail
                        },

                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="sale-return",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleReturn,
                            SortOrder= 4,
                            TemplateName = L("SaleReturnReport"),
                            Type = ReportType.ReportType_SaleReturn
                        },

                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="customer-aging",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Customer_CustomerAging,
                            SortOrder= 2,
                            TemplateName = L("CustomerAgingReport"),
                            Type = ReportType.ReportType_CustomerAging
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="customer-by-invoice",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoice,
                            SortOrder= 3,
                            TemplateName = L("CustomerByInvoiceReport"),
                            Type = ReportType.ReportType_CustomerByInvoice
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="ar-by-invoice-payment",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Customer_CustomerByInvoiceWithPayment,
                            SortOrder= 6,
                            TemplateName = L("ARByInvoiceWithPaymentReport"),
                            Type = ReportType.ReportType_ARByInvoiceWithPayment
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="profit-by-invoice",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_ProfitByInvoice,
                            SortOrder= 7,
                            TemplateName = L("ProfitByInvoice"),
                            Type = ReportType.ReportType_ProfitByInvoice
                        },
                        new ReportDefaultTemplate(){
                            Icon= "",
                            RoutePath ="sale-invoice-by-item-property",
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleInvoiceByProperty,
                            SortOrder= 8,
                            TemplateName = L("SaleInvoiceByItemPropertyReport"),
                            Type = ReportType.ReportType_SaleByItemProperty
                        },
                    }
                },
                new AllReport()
                {
                    ReportCategory = ReportCategory.AccountingReport,
                    TemplateName = L("Accounting"),
                    Id = 1,
                    BackGroundColor = "#E8A464",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_Accounting,
                    Icon = "icon-m-accounting",
                    Items = new List<ReportDefaultTemplate>(){
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Journal,
                            Icon = "icon-notebook",
                            RoutePath = "journal",
                            SortOrder = 1,
                            TemplateName = L("JournalReport"),
                            Type = ReportType.ReportType_Journal,
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_BalanceSheet,
                            Icon = "icon-m-balance-sheet",
                            RoutePath = "balance-sheet",
                            SortOrder = 2,
                            TemplateName = L("BalanceSheetReport"),
                            Type = ReportType.ReportType_BalanceSheet
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Income,
                            Icon = "icon-m-profit-loss",
                            RoutePath = "profit-loss",
                            SortOrder = 3,
                            TemplateName = L("ProfitAndLossReport"),
                            Type= ReportType.ReportType_ProfitAndLoss
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Ledger,
                            RoutePath = "ledger",
                            SortOrder = 4,
                            TemplateName = L("LedgerReport"),
                            Type= ReportType.ReportType_Ledger
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_Cash,
                            RoutePath = "cash",
                            SortOrder = 5,
                            TemplateName = L("CashReport"),
                            Type= ReportType.ReportType_Cash
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_CashFlow,
                            RoutePath = "cash-flow",
                            SortOrder = 6,
                            TemplateName = L("CashFlowReport"),
                            Type= ReportType.ReportType_CashFlow
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_CashFlow,
                            RoutePath = "direct-cash-flow",
                            SortOrder = 7,
                            TemplateName = L("DirectCashFlowReport"),
                            Type= ReportType.ReportType_DirectCashFlow
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_TrialBalance,
                            RoutePath = "trial-balance",
                            SortOrder = 8,
                            TemplateName = L("TrialBalanceReport"),
                            Type = ReportType.ReportType_TrialBalance
                        },
                    }
                },
                new AllReport()
                {
                    ReportCategory = ReportCategory.PurchaseOrderReport,
                    TemplateName = L("PurchaseOrder"),
                    Id = 1,
                    BackGroundColor = "#6A0DAD",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_PurchaseOrder,
                    Icon = "icon-Vendor",
                    Items = new List<ReportDefaultTemplate>(){
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_PurchaseOrder,
                            Icon = "icon-notebook",
                            RoutePath = "purchase-order-summary",
                            SortOrder = 1,
                            TemplateName = L("PurchaseOrderSummary"),
                            Type = ReportType.ReportType_PurchaseOrderSummary,
                        },
                    }
                },
                new AllReport()
                {
                    ReportCategory = ReportCategory.SaleOrderReport,
                    TemplateName = L("SaleOrder"),
                    Id = 1,
                    BackGroundColor = "#75CD6D",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_SaleOrder,
                    Icon = "icon-m-customer",
                    Items = new List<ReportDefaultTemplate>(){
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleOrder,
                            Icon = "icon-notebook",
                            RoutePath = "sale-order-summary",
                            SortOrder = 1,
                            TemplateName = L("SaleOrderSummary"),
                            Type = ReportType.ReportType_SaleOrderSummary,
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleOrderDetail,
                            Icon = "icon-notebook",
                            RoutePath = "sale-order-detail",
                            SortOrder = 2,
                            TemplateName = L("SaleOrderDetail"),
                            Type = ReportType.ReportType_SaleOrderDetail,
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_SaleOrderByItemProperty,
                            Icon = "icon-notebook",
                            RoutePath = "sale-order-by-item-property",
                            SortOrder = 3,
                            TemplateName = L("SaleOrderByItemProperty"),
                            Type = ReportType.ReportType_SaleOrderByItemProperty,
                        },
                    }
                },

               new AllReport()
                {
                    ReportCategory = ReportCategory.DeliveryScheduleReport,
                    TemplateName = L("DeliverySchedule"),
                    Id = 1,
                    BackGroundColor = "#f4516c",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_DeliverySchedule,
                    Icon = "app-icon icon-calendar-icon",
                    Items = new List<ReportDefaultTemplate>(){
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_DeliverySchedule_Summary,
                            Icon = "icon-notebook",
                            RoutePath = "delivery-schedule-summary",
                            SortOrder = 1,
                            TemplateName = L("DeliveryScheduleSummary"),
                            Type = ReportType.ReportType_DeliveryScheduleSummary,
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_DeliverySchedule_Detail,
                            Icon = "app-icon icon-calendar-icon",
                            RoutePath = "delivery-schedule-detail",
                            SortOrder = 2,
                            TemplateName = L("DeliveryScheduleDetail"),
                            Type = ReportType.ReportType_DeliveryScheduleDetail,
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_DeliveryScheduleByItemProperty,
                            Icon = "app-icon icon-calendar-icon",
                            RoutePath = "delivery-schedule-by-item-property",
                            SortOrder = 3,
                            TemplateName = L("DeliveryScheduleByItemProperty"),
                            Type = ReportType.ReportType_DeliveryScheduleByItemProperty,
                        },
                    }
                },

                new AllReport()
                {
                    ReportCategory = ReportCategory.ProductionReport,
                    TemplateName = L("Production"),
                    Id = 1,
                    PermissionNames = AppPermissions.Pages_Tenant_Report_Production,
                    BackGroundColor = "#00CE74",
                    Icon = "icon-production-plan",
                    Items = new List<ReportDefaultTemplate>() {

                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_ProductionPlan,
                            Icon = "icon-m-inventory",
                            RoutePath = "production",
                            SortOrder = 10,
                            TemplateName = L("ProductionPlan"),
                            Type = ReportType.ReportType_ProductionPlan
                        },
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_ProductionOrder,
                            Icon = "icon-m-inventory",
                            RoutePath = "production-order",
                            SortOrder = 11,
                            TemplateName = L("ProductionOrders"),
                            Type = ReportType.ReportType_ProductionOrder
                        },
                    }
                },

                new AllReport()
                {
                    ReportCategory = ReportCategory.QCTestReport,
                    TemplateName = L("QCTest"),
                    Id = 1,
                    BackGroundColor = "#de8a0d",
                    PermissionNames = AppPermissions.Pages_Tenant_Report_QCTest,
                    Icon = "icon-Vendor",
                    Items = new List<ReportDefaultTemplate>(){
                        new ReportDefaultTemplate()
                        {
                            PermissionNames = AppPermissions.Pages_Tenant_Report_QCTest,
                            Icon = "icon-notebook",
                            RoutePath = "qc-test-report",
                            SortOrder = 1,
                            TemplateName = L("QCTest"),
                            Type = ReportType.ReportType_QCTest,
                        },
                    }
                },

            }
            .Where(t => userPermissionDict.ContainsKey(t.PermissionNames))
            .Select(t =>
            {
                t.Items = t.Items.Where(u => userPermissionDict.ContainsKey(u.PermissionNames)).ToList();
                return t;
            })
            .ToList();

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_ViewTemplate)]
        public async Task<PagedResultDto<GetReportTemplateOutput>> GetList(GetReportTemplateInput input)
        {
            var userId = AbpSession.GetUserId();
            var userPermission = await GetUserPermissions(userId);
            bool IsUserPermission = userPermission.Contains(AppPermissions.Pages_Tenant_Report_CanEditAllTemplate);
            var template = await GetReportTemplate();

            var userGroups = await (from groupusers in _groupMemberItemTamplateRepository.GetAll().AsNoTracking()
                                        .Include(u => u.UserGroup)
                                    join userGroupMember in _userGroupMemberRepository.GetAll()
                                    on groupusers.UserGroupId equals userGroupMember.UserGroupId
                                    where (userGroupMember.MemberId == userId || groupusers.CreatorUserId == userId)
                                    select new
                                    {
                                        ReportTemplateId = groupusers.ReportTemplateId,
                                        UserMemberId = userGroupMember.MemberId,
                                    }).ToListAsync();


            var userMemberGroups = await _groupMemberItemTamplateRepository.GetAll().AsNoTracking()
                              .Where(u => u.MemberUserId == userId || u.CreatorUserId == userId)
                              .Select(t => new
                              {

                                  ReportTemplateId = t.ReportTemplateId,
                                  UserMemberId = t.MemberUserId
                              }).ToListAsync();

            var query = _reportTemplateRepository.GetAll().AsNoTracking()
                        .Where(t => t.TemplateType == TemplateType.Gloable
                                    || t.TemplateType == TemplateType.Group && userGroups.Any(x => x.ReportTemplateId == t.Id && x.UserMemberId == userId)
                                    || (t.TemplateType == TemplateType.User && userMemberGroups.Any(x => x.ReportTemplateId == t.Id && x.UserMemberId.Value == userId))
                                    || (t.TemplateType == TemplateType.OnlyMe && t.CreatorUserId == userId)
                                    || t.CreatorUserId == userId
                                    || IsUserPermission == true)
                        .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.TemplateName.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportCategory))
                        .WhereIf(input.Status != 0, p => p.TemplateType == input.Status)
                        .Join(template,
                            r => r.ReportCategory,
                            t => t.ReportCategory,
                            (r, t) => new { report = r, template = t })
                        .Select(x => new GetReportTemplateOutput
                        {

                            PermissionReadWrite = x.report.PermissionReadWrite,
                            Id = x.report.Id,
                            ReportCategory = x.template.ReportCategory,
                            TemplateName = x.report.TemplateName,
                            TemplateType = x.report.TemplateType,
                            Icon = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.Icon).FirstOrDefault(),
                            RoutePath = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.RoutePath).FirstOrDefault(),
                            BackGroundColor = x.template.BackGroundColor
                        }).Where(t => t.RoutePath != null);

            var all = await _reportTemplateRepository.GetAll().AsNoTracking().ToListAsync();

            #region close

            //var queryTest =  _reportTemplateRepository.GetAll().AsNoTracking()
            //                .Where(t => t.TemplateType == TemplateType.Gloable
            //                        || t.TemplateType == TemplateType.Group 
            //                        || (t.TemplateType == TemplateType.User && userMemberGroup.Any(x => x.ReportTemplateId == t.Id && x.UserMemberId.Value == userId))
            //                        || (t.TemplateType == TemplateType.OnlyMe && t.CreatorUserId == userId))
            //                .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.TemplateName.ToLower().Contains(input.Filter.ToLower()))
            //                .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportCategory))
            //                .WhereIf(input.Status != 0, p => p.TemplateType == input.Status);

            //var query = (from reportTamplate in _reportTemplateRepository.GetAll().AsNoTracking()

            //            .Where(t => t.TemplateType == TemplateType.Gloable || t.TemplateType == TemplateType.Group || t.TemplateType == TemplateType.User || (t.TemplateType == TemplateType.OnlyMe && t.CreatorUserId == userId))
            //            .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.TemplateName.ToLower().Contains(input.Filter.ToLower()))
            //            .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportCategory))
            //            .WhereIf(input.Status != 0, p => p.TemplateType == input.Status)

            //            .Join(template,
            //                r => r.ReportCategory,
            //                t => t.ReportCategory,
            //                (r, t) => new { report = r, template = t })
            //            .Select(x => new GetReportTemplateOutput
            //            {

            //                PermissionReadWrite = x.report.PermissionReadWrite,
            //                Id = x.report.Id,
            //                ReportCategory = x.template.ReportCategory,
            //                TemplateName = x.report.TemplateName,
            //                TemplateType = x.report.TemplateType,
            //                Icon = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.Icon).FirstOrDefault(),
            //                RoutePath = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.RoutePath).FirstOrDefault(),
            //                BackGroundColor = x.template.BackGroundColor
            //            }).Where(t => t.RoutePath != null)

            //             join groupmeberItem in _groupMemberItemTamplateRepository.GetAll()
            //                   //.Include(u=>u.UserGroup)
            //                  .Where(u=>u.MemberUserId == userId || u.CreatorUserId == userId)
            //             on reportTamplate.Id equals groupmeberItem.ReportTemplateId into ps
            //             from groupmeberItem in ps.DefaultIfEmpty()
            //             select new GetReportTemplateOutput {
            //                BackGroundColor = reportTamplate.BackGroundColor,
            //                Icon = reportTamplate.Icon,
            //                Id = reportTamplate.Id,
            //                PermissionReadWrite = reportTamplate.PermissionReadWrite,
            //                ReportCategory = reportTamplate.ReportCategory,
            //                RoutePath = reportTamplate.RoutePath,
            //                TemplateName = reportTamplate.TemplateName,
            //                TemplateType = reportTamplate.TemplateType,
            //            });
            #endregion

            var resultCount = await query.CountAsync();
            var @entities = await query.PageBy(input).ToListAsync();

            return new PagedResultDto<GetReportTemplateOutput>(resultCount, ObjectMapper.Map<List<GetReportTemplateOutput>>(@entities));
        }

        public async Task<PagedResultDto<GetReportTemplateOutput>> Find(GetReportFindTemplateInput input)
        {
            #region old code 
            var userId = AbpSession.GetUserId();
            var template = await GetReportTemplate();
            var query = _reportTemplateRepository.GetAll().AsNoTracking()
                        .Where(t => t.TemplateType == TemplateType.Gloable ||
                               (t.TemplateType == TemplateType.OnlyMe ||
                               t.TemplateType == TemplateType.Group ||
                               t.TemplateType == TemplateType.User && t.CreatorUserId == userId))
                        .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.TemplateName.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportCategory))
                        .WhereIf(input.ReportType != null, p => input.ReportType.Contains(p.ReportType))
                        .WhereIf(input.Status != 0, p => p.TemplateType == input.Status)
                        .Join(template,
                            r => r.ReportCategory,
                            t => t.ReportCategory,
                            (r, t) => new { report = r, template = t })
                        .Select(x => new GetReportTemplateOutput
                        {
                            Id = x.report.Id,
                            ReportCategory = x.template.ReportCategory,
                            TemplateName = x.report.TemplateName,
                            TemplateType = x.report.TemplateType,
                            Icon = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.Icon).FirstOrDefault(),
                            RoutePath = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.RoutePath).FirstOrDefault(),
                            BackGroundColor = x.template.BackGroundColor
                        });
            #endregion

            #region old code 
            //var userId = AbpSession.GetUserId();
            //var template = await GetReportTemplate();
            //var query1 = _groupMemberItemTamplateRepository.GetAll()
            //           .Include(u => u.ReportTemplate)
            //           .Include(u => u.UserGroup)
            //              // _reportTemplateRepository.GetAll().AsNoTracking()

            //              .WhereIf(input.UserGroupId != null && input.UserGroupId.Count > 0,
            //                   t => input.UserGroupId.Contains(t.UserGroupId))
            //            .WhereIf(input.MemberId != null && input.MemberId.Count > 0,
            //                   t => input.MemberId.Contains(t.MemberUserId))
            //            .Where(t => t.ReportTemplate.TemplateType == TemplateType.Gloable ||
            //                   (t.ReportTemplate.TemplateType == TemplateType.OnlyMe ||
            //                   t.ReportTemplate.TemplateType == TemplateType.Group ||
            //                   t.ReportTemplate.TemplateType == TemplateType.User && t.CreatorUserId == userId))
            //            .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.ReportTemplate.TemplateName.ToLower().Contains(input.Filter.ToLower()))
            //            .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportTemplate.ReportCategory))
            //            .WhereIf(input.ReportType != null, p => input.ReportType.Contains(p.ReportTemplate.ReportType))
            //            .WhereIf(input.Status != 0, p => p.ReportTemplate.TemplateType == input.Status)
            //            .Join(template,
            //                r => r.ReportTemplate.ReportCategory,
            //                t => t.ReportCategory,
            //                (r, t) => new { report = r, template = t })
            //            .Select(x => new GetReportTemplateOutput
            //            {
            //                Id = x.report.ReportTemplate.Id,
            //                ReportCategory = x.template.ReportCategory,
            //                TemplateName = x.report.ReportTemplate.TemplateName,
            //                TemplateType = x.report.ReportTemplate.TemplateType,
            //                Icon = x.template.Items.Where(t => t.Type == x.report.ReportTemplate.ReportType).Select(t => t.Icon).FirstOrDefault(),
            //                RoutePath = x.template.Items.Where(t => t.Type == x.report.ReportTemplate.ReportType).Select(t => t.RoutePath).FirstOrDefault(),
            //                BackGroundColor = x.template.BackGroundColor
            //            });
            #endregion
            #region list code 
            //var userId = AbpSession.GetUserId();
            //var template = await GetReportTemplate();

            //var userGroups = await (from groupusers in _groupMemberItemTamplateRepository.GetAll().AsNoTracking()
            //                            .Include(u => u.UserGroup)
            //                        join userGroupMember in _userGroupMemberRepository.GetAll()
            //                        on groupusers.UserGroupId equals userGroupMember.UserGroupId
            //                        where (userGroupMember.MemberId == userId || groupusers.CreatorUserId == userId)
            //                        select new
            //                        {
            //                            ReportTemplateId = groupusers.ReportTemplateId,
            //                            UserMemberId = userGroupMember.MemberId,
            //                        }).ToListAsync();


            //var userMemberGroups = await _groupMemberItemTamplateRepository.GetAll().AsNoTracking()
            //                  .Where(u => u.MemberUserId == userId || u.CreatorUserId == userId)
            //                  .Select(t => new
            //                  {

            //                      ReportTemplateId = t.ReportTemplateId,
            //                      UserMemberId = t.MemberUserId
            //                  }).ToListAsync();

            //var query = _reportTemplateRepository.GetAll().AsNoTracking()
            //            .Where(t => t.TemplateType == TemplateType.Gloable
            //                        || t.TemplateType == TemplateType.Group && userGroups.Any(x => x.ReportTemplateId == t.Id && x.UserMemberId == userId)
            //                        || (t.TemplateType == TemplateType.User && userMemberGroups.Any(x => x.ReportTemplateId == t.Id && x.UserMemberId.Value == userId))
            //                        || (t.TemplateType == TemplateType.OnlyMe && t.CreatorUserId == userId)
            //                        || t.CreatorUserId == userId)                         
            //            .WhereIf(!input.Filter.IsNullOrEmpty(), p => p.TemplateName.ToLower().Contains(input.Filter.ToLower()))
            //            .WhereIf(input.ReportCategory != null, p => input.ReportCategory.Contains(p.ReportCategory))
            //            .WhereIf(input.Status != 0, p => p.TemplateType == input.Status)
            //            .Join(template,
            //                r => r.ReportCategory,
            //                t => t.ReportCategory,
            //                (r, t) => new { report = r, template = t })
            //            .Select(x => new GetReportTemplateOutput
            //            {

            //                PermissionReadWrite = x.report.PermissionReadWrite,
            //                Id = x.report.Id,
            //                ReportCategory = x.template.ReportCategory,
            //                TemplateName = x.report.TemplateName,
            //                TemplateType = x.report.TemplateType,
            //                Icon = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.Icon).FirstOrDefault(),
            //                RoutePath = x.template.Items.Where(t => t.Type == x.report.ReportType).Select(t => t.RoutePath).FirstOrDefault(),
            //                BackGroundColor = x.template.BackGroundColor
            //            }).Where(t => t.RoutePath != null);
            #endregion

            var resultCount = await query.CountAsync();
            var @entities = await query.PageBy(input).ToListAsync();
            return new PagedResultDto<GetReportTemplateOutput>(resultCount, ObjectMapper.Map<List<GetReportTemplateOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_Template_Delete)]
        public async Task Delete(EntityDto<long> input)
        {
            var @entity = await _reportTemplateRepository.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
            //delete member group item 
            var memberGroupItems = await _groupMemberItemTamplateRepository.GetAll()
                                   .Where(i => i.ReportTemplateId == input.Id).ToListAsync();
            foreach (var i in memberGroupItems)
            {
                CheckErrors(await _groupMemberItemTamplateManager.RemoveAsync(i));
            }

            CheckErrors(await _reportTemplateManager.RemoveAsync(@entity));

        }

    }

}
