using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.UI;
using CorarlERP.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using System;
using System.IO;
using System.Text;
using Abp.Timing;
using Abp.Domain.Uow;
using CorarlERP.CashFlowTemplates.Dto;
using Abp.Extensions;
using CorarlERP.ChartOfAccounts;
using CorarlERP.AccountTypes;
using Abp.Collections.Extensions;
using CorarlERP.ReportTemplates;
using Amazon.Scheduler.Model;

namespace CorarlERP.CashFlowTemplates
{
    [AbpAuthorize]
    public class CashFlowTemplateAppService : CorarlERPAppServiceBase, ICashFlowTemplateAppService
    {
        private readonly ICorarlRepository<CashFlowTemplate, Guid> _cashFlowTemplateRepository;
        private readonly ICorarlRepository<CashFlowTemplateCategory, Guid> _cashFlowTemplateCategoryRepository;
        private readonly ICorarlRepository<CashFlowTemplateAccount, Guid> _cashFlowTemplateAccountRepository;
        private readonly ICorarlRepository<CashFlowAccountGroup, Guid> _cashFlowAccountGroupRepository;
        private readonly ICorarlRepository<CashFlowCategory, Guid> _cashFlowCategoryRepository;
        private readonly ICorarlRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly ICorarlRepository<ReportTemplate, long> _reportTemplateRepository;
    

        public CashFlowTemplateAppService(
            ICorarlRepository<ReportTemplate, long> reportTemplateRepository,
            ICorarlRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            ICorarlRepository<CashFlowAccountGroup, Guid> cashFlowAccountGroupRepository,
            ICorarlRepository<CashFlowCategory, Guid> cashFlowCategoryRepository,
            ICorarlRepository<CashFlowTemplate, Guid> cashFlowTemplateRepository,
            ICorarlRepository<CashFlowTemplateCategory, Guid> cashFlowTemplateCategoryRepository,
            ICorarlRepository<CashFlowTemplateAccount, Guid> cashFlowTemplateAccountRepository)
        {
            _cashFlowTemplateRepository = cashFlowTemplateRepository;
            _cashFlowTemplateAccountRepository = cashFlowTemplateAccountRepository;
            _cashFlowAccountGroupRepository = cashFlowAccountGroupRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _reportTemplateRepository = reportTemplateRepository;
            _cashFlowCategoryRepository = cashFlowCategoryRepository;
            _cashFlowTemplateCategoryRepository = cashFlowTemplateCategoryRepository;
        }

        private async Task CheckDuplidate(CreateOrUpdateCashFlowTemplateDto input)
        {
            var find = await _cashFlowTemplateRepository.GetAll().AsNoTracking().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            if (find) throw new UserFriendlyException(L("DuplicateTemplate"));
        }

        private async Task ValidateCreateUpdate(CreateOrUpdateCashFlowTemplateDto input)
        {
            if (input.Name.IsNullOrWhiteSpace()) throw new UserFriendlyException(L("IsRequired", L("TemplateName")));
            if (input.Categories.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("CashFlowCategory")));
            if (input.Accounts.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("Account")));

            var uncategory = input.Accounts.Any(s => s.CategoryId == Guid.Empty);
            if (uncategory) throw new UserFriendlyException(L("IsRequired", L("CashFlowCategory")));


            var inGroups = input.Accounts.Where(s => s.InAccountGroupId.HasValue).GroupBy(s => s.InAccountGroupId).Select(s => s.Key.Value).ToList();
            var findGroupIn = await _cashFlowAccountGroupRepository.GetAll().AsNoTracking()
                             .Where(s => inGroups.Contains(s.Id)).CountAsync() == inGroups.Count();
            if (!findGroupIn) throw new UserFriendlyException("RecordNotFound");

            var outGroups = input.Accounts.Where(s => s.InAccountGroupId.HasValue).GroupBy(s => s.InAccountGroupId).Select(s => s.Key.Value).ToList();
            var findGroupOut = await _cashFlowAccountGroupRepository.GetAll().AsNoTracking()
                             .Where(s => outGroups.Contains(s.Id)).CountAsync() == outGroups.Count();
            if (!findGroupOut) throw new UserFriendlyException("RecordNotFound");
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task<CreateOrUpdateCashFlowTemplateDto> Create(CreateOrUpdateCashFlowTemplateDto input)
        {
            await ValidateCreateUpdate(input);
            await CheckDuplidate(input);

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var entity = CashFlowTemplate.Create(tenantId, userId, input.Name, input.Description, input.SplitCashInAndCashOutFlow);

            var categories = new List<CashFlowTemplateCategory>();
            foreach (var i in input.Categories)
            {
                var category = CashFlowTemplateCategory.Create(tenantId, userId, entity.Id, i.Id.Value);
                categories.Add(category);
            }

            var accounts = new List<CashFlowTemplateAccount>();
            foreach(var i in input.Accounts)
            {
                var account = CashFlowTemplateAccount.Create(tenantId, userId, entity.Id, i.CategoryId, i.AccountId, i.InAccountGroupId, i.OutAccountGroupId);
                accounts.Add(account);
            }

            await _cashFlowTemplateRepository.BulkInsertAsync(new List<CashFlowTemplate> { entity });
            await _cashFlowTemplateCategoryRepository.BulkInsertAsync(categories);
            await _cashFlowTemplateAccountRepository.BulkInsertAsync(accounts);

            input.Id = entity.Id;

            return input;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public async Task<PagedResultDto<CashFlowTemplateListDto>> GetList(GetListCashFlowTemplateInput input)
        {
            var query = _cashFlowTemplateRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(input.IsActive.HasValue, s => input.IsActive.Value == s.IsActive)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowTemplateListDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Description = s.Description,
                            IsActive = s.IsActive,
                            IsDefault = s.IsDefault,
                            SplitCashInAndCashOutFlow = s.SplitCashInAndCashOutFlow,
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowTemplateListDto>(0, new List<CashFlowTemplateListDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowTemplateListDto>(count, result);

        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find)]
        public async Task<PagedResultDto<CashFlowTemplateSummaryDto>> Find(GetListCashFlowTemplateInput input)
        {
            var query = _cashFlowTemplateRepository.GetAll()
                        .AsNoTracking()
                        .WhereIf(input.IsActive.HasValue, s => input.IsActive.Value == s.IsActive)
                        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), s => s.Name.ToLower().Contains(input.Filter.ToLower()))
                        .Select(s => new CashFlowTemplateSummaryDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            IsDefault = s.IsDefault
                        });

            var count = await query.CountAsync();

            if (count == 0) return new PagedResultDto<CashFlowTemplateSummaryDto>(0, new List<CashFlowTemplateSummaryDto>());

            if (input.UsePagination)
            {
                query = query.OrderBy(input.Sorting).PageBy(input);
            }
            else
            {
                query = query.OrderBy(input.Sorting);
            }

            var result = await query.ToListAsync();

            return new PagedResultDto<CashFlowTemplateSummaryDto>(count, result);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate)]
        public  async Task<CashFlowTemplateDetailDto> GetDetail(EntityDto<Guid> input)
        {
             var result = await _cashFlowTemplateRepository.GetAll()
                               .AsNoTracking()
                               .Select(s => new CashFlowTemplateDetailDto
                               {
                                   Id = s.Id,
                                   Name = s.Name,
                                   Description = s.Description,
                                   IsActive = s.IsActive,
                                   IsDefault = s.IsDefault,
                                   SplitCashInAndCashOutFlow = s.SplitCashInAndCashOutFlow,
                               })
                               .FirstOrDefaultAsync(s => s.Id == input.Id);

            if(result == null) throw new UserFriendlyException("RecordNotFound");

            var getCategoryAccount = await GetAccountWithDefaultCategoryHelper(input.Id);
            result.Accounts = getCategoryAccount.Accounts;
            result.Categories = getCategoryAccount.Categories;

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update)]
        public async Task<CreateOrUpdateCashFlowTemplateDto> Update(CreateOrUpdateCashFlowTemplateDto input)
        {
            await ValidateCreateUpdate(input);
            await CheckDuplidate(input);
            

            var entity = await _cashFlowTemplateRepository.FirstOrDefaultAsync(s => s.Id == input.Id);
            if(entity == null) throw new UserFriendlyException("RecordNotFound");

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            entity.Update(userId, input.Name, input.Description, input.SplitCashInAndCashOutFlow);

            var categoryEntities = await _cashFlowTemplateCategoryRepository.GetAll().AsNoTracking().Where(s => s.TemplateId == input.Id).ToListAsync();
            var addCategorys = input.Categories.Where(s => !categoryEntities.Any(r => r.CategoryId == s.Id)).ToList();
            var deleteCategorys = categoryEntities.Where(s => !input.Categories.Any(r => r.Id == s.CategoryId)).ToList();

            if (addCategorys.Any())
            {
                var categories = new List<CashFlowTemplateCategory>();
                foreach (var i in addCategorys)
                {
                    var category = CashFlowTemplateCategory.Create(tenantId, userId, entity.Id, i.Id.Value);
                    categories.Add(category);
                }
                await _cashFlowTemplateCategoryRepository.BulkInsertAsync(categories);
            }

            if (deleteCategorys.Any()) await _cashFlowTemplateCategoryRepository.BulkDeleteAsync(deleteCategorys);           

            var accountEntities = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking().Where(s => s.TemplateId == input.Id).ToListAsync();
            var addAccounts = input.Accounts.Where(s => !s.Id.HasValue).ToList();
            var updateAccounts = input.Accounts.Where(s => s.Id.HasValue).ToList();
            var deleteAccounts = accountEntities.Where(s => !updateAccounts.Any(r => s.Id == r.Id)).ToList();

            if (addAccounts.Any())
            {
                var accounts = new List<CashFlowTemplateAccount>();
                foreach (var i in addAccounts)
                {
                    var account = CashFlowTemplateAccount.Create(tenantId, userId, entity.Id, i.CategoryId, i.AccountId, i.InAccountGroupId, i.OutAccountGroupId);
                    accounts.Add(account);
                }
                await _cashFlowTemplateAccountRepository.BulkInsertAsync(accounts);
            }

            if (updateAccounts.Any())
            {
                var accounts = new List<CashFlowTemplateAccount>();
                foreach (var i in updateAccounts)
                {
                    var account = accountEntities.FirstOrDefault(s => s.Id == i.Id);
                    if(account == null) throw new UserFriendlyException("RecordNotFound");
                    account.Update(userId, entity.Id, i.CategoryId, i.AccountId, i.InAccountGroupId, i.OutAccountGroupId);
                    accounts.Add(account);
                }
                await _cashFlowTemplateAccountRepository.BulkUpdateAsync(accounts);
            }

            if (deleteAccounts.Any()) await _cashFlowTemplateAccountRepository.BulkDeleteAsync(deleteAccounts);

            await _cashFlowTemplateRepository.UpdateAsync(entity);           

            input.Id = entity.Id;

            return input;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var find = await _reportTemplateRepository.GetAll().AnyAsync(s => s.DefaultTemplateReport == input.Id.ToString());
            if (find) throw new UserFriendlyException(L("CannotDeleteTemplateIsInUse"));

            var entity = await _cashFlowTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            var templateCategories = await _cashFlowTemplateCategoryRepository.GetAll().AsNoTracking().Where(s => s.TemplateId == input.Id).ToListAsync();
            if (templateCategories.Any()) await _cashFlowTemplateCategoryRepository.BulkDeleteAsync(templateCategories);

            var templateAccounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking().Where(s => s.TemplateId == input.Id).ToListAsync();
            if (templateAccounts.Any()) await _cashFlowTemplateAccountRepository.BulkDeleteAsync(templateAccounts);

            await _cashFlowTemplateRepository.DeleteAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var entity = await _cashFlowTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(true);
            entity.LastModificationTime = Clock.Now;
            entity.LastModifierUserId = AbpSession.UserId;

            await _cashFlowTemplateRepository.UpdateAsync(entity);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var entity = await _cashFlowTemplateRepository.GetAll().FirstOrDefaultAsync(s => s.Id == input.Id);
            if (entity == null) throw new UserFriendlyException(L("RecordNotFound"));

            entity.Enable(false);
            entity.LastModificationTime = Clock.Now;
            entity.LastModifierUserId = AbpSession.UserId;

            await _cashFlowTemplateRepository.UpdateAsync(entity);

        }

        private async Task<Dictionary<CashFlowCategoryType, CashFlowCategory>> SyncAndGetDefaultCategories()
        {
            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var defaultTypes = new List<CashFlowCategoryType> { CashFlowCategoryType.Operation, CashFlowCategoryType.Investment, CashFlowCategoryType.Finance };

            var cateogries = await _cashFlowCategoryRepository.GetAll()
                                   .AsNoTracking()
                                   .Where(s => defaultTypes.Contains(s.Type) && s.IsDefault)
                                   .ToListAsync();

            var addDefaultCategories = new List<CashFlowCategory>();

            var operationCategory = cateogries.FirstOrDefault(s => s.Type == CashFlowCategoryType.Operation);
            if (operationCategory == null)
            {
                operationCategory = CashFlowCategory.Create(tenantId, userId, CashFlowCategoryType.Operation, CashFlowCategoryType.Operation.GetName(), 1, "");
                operationCategory.SetDefault(true);
                addDefaultCategories.Add(operationCategory);
            }

            var investmentCategory = cateogries.FirstOrDefault(s => s.Type == CashFlowCategoryType.Investment);
            if (investmentCategory == null)
            {
                investmentCategory = CashFlowCategory.Create(tenantId, userId, CashFlowCategoryType.Investment, CashFlowCategoryType.Investment.GetName(), 2, "");
                investmentCategory.SetDefault(true);
                addDefaultCategories.Add(investmentCategory);
            }

            var financeCategory = cateogries.FirstOrDefault(s => s.Type == CashFlowCategoryType.Finance);
            if (financeCategory == null)
            {
                financeCategory = CashFlowCategory.Create(tenantId, userId, CashFlowCategoryType.Finance, CashFlowCategoryType.Finance.GetName(), 3, "");
                financeCategory.SetDefault(true);
                addDefaultCategories.Add(financeCategory);
            }

            if (addDefaultCategories.Any()) await _cashFlowCategoryRepository.BulkInsertAsync(addDefaultCategories);


            return new Dictionary<CashFlowCategoryType, CashFlowCategory>
            {
                { CashFlowCategoryType.Operation, operationCategory },
                { CashFlowCategoryType.Investment, investmentCategory },
                { CashFlowCategoryType.Finance, financeCategory },
            };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task SyncDefaultTemplate()
        {
            var find = await _cashFlowTemplateRepository.GetAll().AsNoTracking().AnyAsync(s => s.IsDefault);
            if (find) return;

            var chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking()
                                        .Include(s => s.AccountType)
                                        .ToListAsync();

            if (chartOfAccounts.IsNullOrEmpty()) throw new UserFriendlyException(L("IsRequired", L("ChartOfAccount")));

            var tenantId = AbpSession.TenantId.Value;
            var userId = AbpSession.UserId.Value;

            var entity = CashFlowTemplate.Create(tenantId, userId, "Default", "Default Template", false);
            entity.SetDefault(true);
            await _cashFlowTemplateRepository.BulkInsertAsync(new List<CashFlowTemplate> { entity });

            var equityAccountType = AccountTypeEnums.Equity.GetName();
            var fixedAssetAccountType = AccountTypeEnums.FixedAssets.GetName();
            var longTermLiabilityAccountType = AccountTypeEnums.LongTermLiability.GetName();

            var defaultCategoryDic = await SyncAndGetDefaultCategories();

            var categories = new List<CashFlowTemplateCategory>();
            foreach (var i in defaultCategoryDic)
            {
                var category = CashFlowTemplateCategory.Create(tenantId, userId, entity.Id, i.Value.Id);
                categories.Add(category);
            }
            await _cashFlowTemplateCategoryRepository.BulkInsertAsync(categories);

            var accounts = new List<CashFlowTemplateAccount>();
            foreach (var i in chartOfAccounts)
            {
                var category = i.AccountType.AccountTypeName == fixedAssetAccountType ? defaultCategoryDic[CashFlowCategoryType.Investment] :
                               i.AccountType.AccountTypeName == equityAccountType || i.AccountType.AccountTypeName == longTermLiabilityAccountType ? defaultCategoryDic[CashFlowCategoryType.Finance] : defaultCategoryDic[CashFlowCategoryType.Operation];
                var account = CashFlowTemplateAccount.Create(tenantId, userId, entity.Id, category.Id, i.Id, null, null);
                accounts.Add(account);
            }
                       
            await _cashFlowTemplateAccountRepository.BulkInsertAsync(accounts);
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate, AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task<GetCashFlowCategoryWithAccountOutput> GetAccountWithDefaultCategory()
        {
            return await GetAccountWithDefaultCategoryHelper(null);
        }


        private async Task<GetCashFlowCategoryWithAccountOutput> GetAccountWithDefaultCategoryHelper(Guid? id)
        {
            var defaultCategories = new List<CashFlowCategoryDto>();
            var accounts = new List<CashFlowTemplateAccountDetailDto>();

            if (id.HasValue)
            {
                accounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking()
                                .Where(s => s.TemplateId == id.Value)
                                .Select(s => new CashFlowTemplateAccountDetailDto
                                {
                                    Id = s.Id,
                                    TemplateId = s.TemplateId,
                                    InAccountGroupId = s.AccountGroupId,
                                    InAccountGroupName = s.AccountGroupId.HasValue ? s.AccontGroup.Name : "",
                                    InAccountGroupSortOrder = s.AccountGroupId.HasValue ? s.AccontGroup.SortOrder : 0,
                                    OutAccountGroupId = s.OutAccountGroupId,
                                    OutAccountGroupName = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.Name : "",
                                    OutAccountGroupSortOrder = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.SortOrder : 0,
                                    AccountId = s.AccountId,
                                    AccountName = s.Account.AccountName,
                                    AccountCode = s.Account.AccountCode,
                                    CategoryId = s.CategoryId,
                                    CategoryName = s.Category.Name,
                                    CashTransfer = s.Category.Type == CashFlowCategoryType.CashTransfer,
                                    CategorySortOrder = s.Category.SortOrder,                                                                    
                                })
                                .OrderBy(s => s.CategorySortOrder)
                                .ThenBy(s => s.InAccountGroupSortOrder)
                                .ThenBy(s => s.OutAccountGroupSortOrder)
                                .ThenBy(s => s.AccountCode)
                                .ToListAsync();

                defaultCategories = await _cashFlowTemplateCategoryRepository.GetAll()
                                    .AsNoTracking()
                                    .Where(s => s.TemplateId == id.Value)
                                    .Select(s => 
                                    new CashFlowCategoryDto
                                    {
                                        Id = s.CategoryId,
                                        Name = s.Category.Name,
                                        SortOrder = s.Category.SortOrder,
                                        Type = s.Category.Type,                                        
                                    })
                                    .OrderBy(s => s.SortOrder)
                                    .ToListAsync();
            }
            else
            {
               
                var chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking()
                                            .Include(s => s.AccountType)
                                            .OrderBy(s => s.AccountCode)
                                            .ToListAsync();

                var equityAccountType = AccountTypeEnums.Equity.GetName();
                var fixedAssetAccountType = AccountTypeEnums.FixedAssets.GetName();
                var longTermLiabilityAccountType = AccountTypeEnums.LongTermLiability.GetName();


                var defaultCategoryDic = await SyncAndGetDefaultCategories();

                var operationCategory = defaultCategoryDic[CashFlowCategoryType.Operation];
                var investmentCategory = defaultCategoryDic[CashFlowCategoryType.Investment];
                var financeCategory = defaultCategoryDic[CashFlowCategoryType.Finance];

                foreach (var i in chartOfAccounts)
                {
                    var category = i.AccountType.AccountTypeName == fixedAssetAccountType ? investmentCategory :
                                   i.AccountType.AccountTypeName == equityAccountType || i.AccountType.AccountTypeName == longTermLiabilityAccountType ? financeCategory : operationCategory;
                    var account = new CashFlowTemplateAccountDetailDto
                    {
                        AccountId = i.Id,
                        AccountName = i.AccountName,
                        AccountCode = i.AccountCode,
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        CategorySortOrder = category.SortOrder,
                    };
                    accounts.Add(account);
                }

                defaultCategories = defaultCategoryDic.Values
                                   .Select(s =>
                                   new CashFlowCategoryDto
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       SortOrder = s.SortOrder,
                                       Type = s.Type,
                                   })
                                   .OrderBy(s => s.SortOrder)
                                   .ToList();
            }


            return new GetCashFlowCategoryWithAccountOutput
            {
                Categories = defaultCategories,
                Accounts = accounts
            };
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Update, AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Create)]
        public async Task<List<CashFlowTemplateAccountDetailDto>> SyncAccounts(EntityDto<Guid?> input)
        {

            var accounts = new List<CashFlowTemplateAccountDetailDto>();
            if (input.Id.HasValue)
            {
                accounts = await _cashFlowTemplateAccountRepository.GetAll().AsNoTracking()
                                .Where(s => s.TemplateId == input.Id.Value)
                                .Select(s => new CashFlowTemplateAccountDetailDto
                                {
                                    Id = s.Id,
                                    TemplateId = s.TemplateId,
                                    InAccountGroupId = s.AccountGroupId,
                                    InAccountGroupName = s.AccountGroupId.HasValue ? s.AccontGroup.Name : "",
                                    InAccountGroupSortOrder = s.AccountGroupId.HasValue ? s.AccontGroup.SortOrder : 0,
                                    OutAccountGroupId = s.OutAccountGroupId,
                                    OutAccountGroupName = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.Name : "",
                                    OutAccountGroupSortOrder = s.OutAccountGroupId.HasValue ? s.OutAccontGroup.SortOrder : 0,
                                    AccountId = s.AccountId,
                                    AccountName = s.Account.AccountName,
                                    AccountCode = s.Account.AccountCode,
                                    CategoryId = s.CategoryId,
                                    CategoryName = s.Category.Name,
                                    CashTransfer = s.Category.Type == CashFlowCategoryType.CashTransfer,
                                    CategorySortOrder = s.Category.SortOrder,
                                })
                                .ToListAsync();
            }
           

            var chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking()
                                        .Include(s => s.AccountType)
                                        .WhereIf(!accounts.IsNullOrEmpty(), s => !accounts.Any(a => a.AccountId == s.Id))
                                        .ToListAsync();

            if (!chartOfAccounts.IsNullOrEmpty()) 
            {
                var equityAccountType = AccountTypeEnums.Equity.GetName();
                var fixedAssetAccountType = AccountTypeEnums.FixedAssets.GetName();
                var longTermLiabilityAccountType = AccountTypeEnums.LongTermLiability.GetName();

                var defaultCategoryDic = await SyncAndGetDefaultCategories();

                var operationCategory = defaultCategoryDic[CashFlowCategoryType.Operation];
                var investmentCategory = defaultCategoryDic[CashFlowCategoryType.Investment];
                var financeCategory = defaultCategoryDic[CashFlowCategoryType.Finance];

                foreach (var i in chartOfAccounts)
                {
                    var category = i.AccountType.AccountTypeName == fixedAssetAccountType ? investmentCategory :
                                   i.AccountType.AccountTypeName == equityAccountType || i.AccountType.AccountTypeName == longTermLiabilityAccountType ? financeCategory : operationCategory;
                    var account = new CashFlowTemplateAccountDetailDto
                    {
                        AccountId = i.Id,
                        AccountName = i.AccountName,
                        AccountCode = i.AccountCode,
                        CategoryId = category.Id,
                        CategoryName = category.Name,
                        CategorySortOrder = category.SortOrder,
                    };
                    accounts.Add(account);
                }
            }


            return accounts
                   .OrderBy(s => s.CategorySortOrder)
                   .ThenBy(s => s.InAccountGroupSortOrder)
                   .ThenBy(s => s.OutAccountGroupSortOrder)
                   .ThenBy(s => s.AccountCode)
                   .ToList();
          
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Report_CashFlowTemplate_Find)]
        public async Task<CashFlowTemplateSummaryDto> GetDefaultTemplate()
        {
            var template = await _cashFlowTemplateRepository.GetAll().AsNoTracking()
                                .Select(s => new CashFlowTemplateSummaryDto
                                {
                                    Id = s.Id,
                                    Name = s.Name,                                    
                                })
                                .FirstOrDefaultAsync();

            return template;
        }
    }
}
