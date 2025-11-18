using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using CorarlERP.Customers.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using CorarlERP.Vendors.Dto;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Dto;
using CorarlERP.ChartOfAccounts;
using OfficeOpenXml;
using CorarlERP.Reports;
using System.IO;
using CorarlERP.Addresses;
using CorarlERP.CustomerTypes;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.UserGroups;
using CorarlERP.MultiTenancy;
using Abp.Dependency;
using CorarlERP.Items;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.FileStorages;
using CorarlERP.Features;
using Abp.Application.Features;

namespace CorarlERP.Customers
{

    public class CustomerAppService : ReportBaseClass, ICustomerAppService
    {
        private readonly ICustomerManager _customerManager;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly ICustomerContactPersonManager _customerContactPersonManager;
        private readonly IRepository<CustomerContactPerson, Guid> _customerContactPersonRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly AppFolders _appFolders;
        private readonly IRepository<CustomerType, long> _customerTypeRepository;

        private readonly ICustomerGroupManager _customerGroupManager;
        private readonly IRepository<CustomerGroup, Guid> _customerGroupRepository;

        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly ITenantManager _tenantManager;
        private readonly IRepository<UserGroup, Guid> _userGroupRepository;
        private readonly IRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IFeatureChecker _featureChecker;
        public CustomerAppService(ICustomerManager customerManager, ICustomerContactPersonManager contactPersonManager,
                           IRepository<Customer, Guid> customerRepository,
                           IRepository<CustomerContactPerson, Guid> customerContactPersonRepository,
                           AppFolders appFolders,
                           IFileStorageManager fileStorageManager,
                           IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
                           IRepository<CustomerType, long> customerTypeRepository,
                           ICustomerGroupManager customerGroupManager,
                           IRepository<CustomerGroup, Guid> customerGroupRepository,
                           IRepository<UserGroupMember, Guid> userGroupMemberRepository,
                           IRepository<UserGroup, Guid> userGroupRepository,
                           IFeatureChecker featureChecker,
                           IRepository<ItemsUserGroup, Guid> itemUserGroupRepository)
            : base(null, appFolders, userGroupMemberRepository, null)
        {
            _userGroupMemberRepository = userGroupMemberRepository;
            _customerGroupManager = customerGroupManager;
            _customerGroupRepository = customerGroupRepository;
            _customerManager = customerManager;
            _customerRepository = customerRepository;
            _customerContactPersonManager = contactPersonManager;
            _customerContactPersonRepository = customerContactPersonRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _appFolders = appFolders;
            _customerTypeRepository = customerTypeRepository;
            _tenantManager = IocManager.Instance.Resolve<ITenantManager>();
            _userGroupRepository = userGroupRepository;
            _itemUserGroupRepository = itemUserGroupRepository;
            _fileStorageManager = fileStorageManager;
            _featureChecker = featureChecker;
        }

        private IQueryable<CustomerSummaryOutput> GetCustomerGroup(long userId)
        {
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToList();

            var @query = _customerGroupRepository.GetAll()
                            .Include(u => u.Customer)
                            .Where(t => t.Customer.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            .AsNoTracking()
                            .Select(t => t.Customer);

            var @queryAll = _customerRepository.GetAll()
                            .Where(t => t.Member == Member.All)
                            .AsNoTracking();

            var result = @queryAll.Concat(query)
                        .Select(t => new CustomerSummaryOutput
                        {
                            Id = t.Id,
                            CustomerName = t.CustomerName,
                            CustomerCode = t.CustomerCode
                        });
            return result;
        }

        private async Task CheckMaxCustomerCountAsync(int tenantId)
        {
            var maxCustomerCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxCustomerCount)).To<int>();
            if (maxCustomerCount <= 0)
            {
                return;
            }

            var currentCustomerCount = await _customerRepository.CountAsync();
            if (currentCustomerCount >= maxCustomerCount)
            {
                throw new UserFriendlyException(L("MaximumCustomerCount_Error_Detail", maxCustomerCount));
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Create, AppPermissions.Pages_Tenant_Customer_CreatePOS,
           AppPermissions.Pages_Tenant_SetUp_Customer_Create)]
        public async Task<ValidationCountOutput> CheckMaxCustomerCount()
        {
            var maxCustomerCount = (await _featureChecker.GetValueAsync(AbpSession.TenantId.Value, AppFeatures.MaxCustomerCount)).To<int>();
           

            var currentCustomerCount = await _customerRepository.CountAsync();
            return new ValidationCountOutput { MaxCount = maxCustomerCount, MaxCurrentCount = currentCustomerCount };
        }



        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Create, AppPermissions.Pages_Tenant_Customer_CreatePOS,
             AppPermissions.Pages_Tenant_SetUp_Customer_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateCustomerInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await this.CheckMaxCustomerCountAsync(AbpSession.TenantId.Value);
            }
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();
            var @entity = Customer.Create(tenantId, userId, input.CustomerCode, input.CustomerName, input.Email, input.Website, input.Remark,
                input.BillingAddress, input.ShippingAddress, input.SameAsShippngAddress, input.PhoneNumber, input.AccountId, input.CustomerTypeId, input.IsWalkIn);

            @entity.UpdateMember(input.Member);
            #region contactperson
            if (input.ContactPersons.Where(s => s.IsPrimary).Select(s => s.IsPrimary).Take(2).ToList().Count() == 2)
            {
                throw new UserFriendlyException(L("DuplicatePrimary"));
            }
            foreach (var contactPerson in input.ContactPersons)
            {
                var @contactPersons = CustomerContactPerson.CreateContactPerson(tenantId, userId, contactPerson.Title, contactPerson.FirstName, contactPerson.LastName, contactPerson.DisplayNameAs, @entity, contactPerson.IsPrimary, contactPerson.PhoneNumber, contactPerson.Email);
                CheckErrors(await _customerContactPersonManager.CreateAsync(@contactPersons));
            }
            #endregion

            if (input.IsPOS && input.LocationId.HasValue)
            {
                var userGroup = await _userGroupRepository.GetAll().Where(t => t.LocationId == input.LocationId).FirstOrDefaultAsync();
                if (userGroup != null)
                {
                    input.UserGroups = new List<GroupItems> {
                        new GroupItems { UserGroupId = userGroup.Id }
                    };

                    @entity.UpdateMember(Member.UserGroup);
                }
            }


            #region customer Group
            if (input.UserGroups != null && input.UserGroups.Count > 0)
            {
                foreach (var c in input.UserGroups)
                {
                    var customerGroup = CustomerGroup.Create(tenantId, userId, entity.Id, c.UserGroupId);
                    CheckErrors(await _customerGroupManager.CreateAsync(customerGroup));
                }
            }
            #endregion

            CheckErrors(await _customerManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Delete, AppPermissions.Pages_Tenant_Customer_DeletePOS, AppPermissions.Pages_Tenant_SetUp_Customer_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _customerManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var contactPersons = await _customerContactPersonRepository.GetAll().Where(u => u.CustomerId == entity.Id).ToListAsync();

            foreach (var s in contactPersons)
            {
                CheckErrors(await _customerContactPersonManager.RemoveAsync(s));
            }

            CheckErrors(await _customerManager.RemoveAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Disable,
            AppPermissions.Pages_Tenant_Customer_DisablePOS,
            AppPermissions.Pages_Tenant_SetUp_Customer_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _customerManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _customerManager.DisableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Enable, AppPermissions.Pages_Tenant_Customer_EnablePOS, AppPermissions.Pages_Tenant_SetUp_Customer_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _customerManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _customerManager.EnableAsync(@entity));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Find, AppPermissions.Pages_Tenant_SetUp_Customer)]
        public async Task<PagedResultDto<GetListFindOutput>> Find(GetCustomerListInput input)
        {
            var userId = AbpSession.GetUserId();
            // get user by group member
            var userGroups = await _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == userId)
                            .Where(x => x.UserGroup.LocationId != null)
                            .WhereIf(input.Locations != null && input.Locations.Count > 0, t => input.Locations.Contains(t.UserGroup.LocationId))
                            .AsNoTracking()
                            .Select(x => x.UserGroupId)
                            .ToListAsync();

            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var @query = _customerGroupRepository.GetAll()
                            .Include(u => u.Customer.Account)
                            .Where(t => t.Customer.Member == Member.UserGroup && (!input.IsWalkIn || t.Customer.IsWalkIn))
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            .WhereIf(input.IsActive != null, p => p.Customer.IsActive == input.IsActive.Value)
                            .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Any(), p => input.CustomerTypes.Contains(p.Customer.CustomerTypeId.Value))
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.Customer.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.Customer.CustomerCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(t => t.Customer);

            var @queryAll = _customerRepository
                          .GetAll()
                          .Include(u => u.Account)
                          .AsNoTracking()
                          .Where(t => t.Member == Member.All && (!input.IsWalkIn || t.IsWalkIn))
                          .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                          .WhereIf(input.CustomerTypes != null && input.CustomerTypes.Any(), p => input.CustomerTypes.Contains(p.CustomerTypeId.Value))
                          .WhereIf(
                              !input.Filter.IsNullOrEmpty(),
                              p => p.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                                   p.CustomerCode.ToLower().Contains(input.Filter.ToLower())
                          );

            var findQuery = @queryAll.Union(query).WhereIf(customerTypeMemberIds.Any(), c => customerTypeMemberIds.Contains(c.CustomerTypeId.Value));

            var resultCount = await findQuery.CountAsync();
            var @entities = await findQuery
                                .OrderBy(input.Sorting)
                                .PageBy(input)
                                .ToListAsync();
            return new PagedResultDto<GetListFindOutput>(resultCount, ObjectMapper.Map<List<GetListFindOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Find, AppPermissions.Pages_Tenant_SetUp_Customer)]
        public async Task<CustomerDetailOutput> FindDefault(long locationId)
        {
            var @entity = await _customerGroupRepository.GetAll()
                            .Include(s => s.Customer.Account)
                            .Include(s => s.UserGroup)
                            .Where(s => s.UserGroup.LocationId == locationId && s.Customer.IsWalkIn == true)
                            .Select(s => s.Customer)
                            .FirstOrDefaultAsync();
            if (entity == null)
            {
                @entity = await _customerRepository.GetAll()
                            .Include(s => s.Account)
                            .Where(s => s.Member == Member.All && s.IsWalkIn == true)
                            .Select(s => s)
                            .FirstOrDefaultAsync();
            }
            if (entity == null)
            {
                throw new UserFriendlyException(L("Customer") + " " + L("RecordNotFound"));
            }

            var result = ObjectMapper.Map<CustomerDetailOutput>(@entity);

            return result;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Find, AppPermissions.Pages_Tenant_SetUp_Customer)]
        public async Task<bool> CheckExist(CheckExistCustomerInput input)
        {
            var result = await _customerGroupRepository.GetAll()
                          .AsNoTracking()
                          .Where(s => s.UserGroup.LocationId == input.LocationId)
                          .Where(s => s.CustomerId == input.CustomerId)
                          .AnyAsync();

            if (result) return true;

            result = await _customerRepository.GetAll()
                        .AsNoTracking()
                        .Where(s => s.Member == Member.All)
                        .Where(s => s.Id == input.CustomerId)
                        .AnyAsync();

            return result;
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_GetDetail, AppPermissions.Pages_Tenant_Customer_GetDetailPOS, AppPermissions.Pages_Tenant_SetUp_Customer_GetDetail)]
        public async Task<CustomerDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _customerManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var @contactPersons = await _customerContactPersonRepository.GetAll().Where(u => u.CustomerId == entity.Id).ToListAsync();

            var result = ObjectMapper.Map<CustomerDetailOutput>(@entity);

            result.CustomerContactPersons = ObjectMapper.Map<List<CustomerContactPersonDetailOutput>>(@contactPersons);

            result.UserGroups = await _customerGroupRepository.GetAll().Include(t => t.UserGroup)
                            .Where(u => u.CustomerId == entity.Id).AsNoTracking()
                            .Select(t => new GroupItems
                            {
                                Id = t.Id,
                                UserGroupId = t.UserGroupId,
                                UserGroupName = t.UserGroup.Name
                            })
                            .ToListAsync();

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_GetList, AppPermissions.Pages_Tenant_SetUp_Customer_GetList)]
        public async Task<PagedResultDto<CustomerGetListOutput>> GetList(GetCustomerListInput input)
        {
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var @query = _customerRepository
               .GetAll()
               .Include(u => u.Account)
               .AsNoTracking()
               .WhereIf(input.CustomerTypes != null, p => input.CustomerTypes.Contains(p.CustomerTypeId))
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(customerTypeMemberIds.Any(), c => customerTypeMemberIds.Contains(c.CustomerTypeId.Value))
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                        p.CustomerCode.ToLower().Contains(input.Filter.ToLower())
               );

            var resultCount = await query.CountAsync();
            var @entities = await query.ToListAsync();
            if (input.UsePagination)
            {
                entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                entities = await query.ToListAsync();
            }
            return new PagedResultDto<CustomerGetListOutput>(resultCount, ObjectMapper.Map<List<CustomerGetListOutput>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_GetListPOS)]
        public async Task<PagedResultDto<CustomerGetListOutput>> GetListPOS(GetCustomerListInput input)
        {
            var userId = AbpSession.GetUserId();
            var customerUserGroup = GetCustomerGroup(AbpSession.GetUserId());
            var customerTypeMemberIds = await GetCustomerTypeMembers().Select(s => s.CustomerTypeId).ToListAsync();

            var @query = from cus in _customerRepository
               .GetAll()
               .Include(u => u.Account)
               .AsNoTracking()
               .WhereIf(input.CustomerTypes != null, p => input.CustomerTypes.Contains(p.CustomerTypeId))
               .WhereIf(customerTypeMemberIds.Any(), p => customerTypeMemberIds.Contains(p.CustomerTypeId.Value))
               .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
               .WhereIf(
                   !input.Filter.IsNullOrEmpty(),
                   p => p.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                        p.CustomerCode.ToLower().Contains(input.Filter.ToLower()))
                         join cg in customerUserGroup
                                      on cus.Id equals cg.Id
                         select new CustomerGetListOutput
                         {
                             Id = cus.Id,
                             IsActive = cus.IsActive,
                             CustomerCode = cus.CustomerCode,
                             CustomerName = cus.CustomerName,
                             PhoneNumber = cus.PhoneNumber,
                             Email = cus.Email,
                             AccountId = cus.AccountId,
                             Account = ObjectMapper.Map<ChartAccountSummaryOutput>(cus.Account),

                         };

            var resultCount = await query.CountAsync();
            var @entities = await query.ToListAsync();
            if (input.UsePagination)
            {
                entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            }
            else
            {
                entities = await query.ToListAsync();
            }
            return new PagedResultDto<CustomerGetListOutput>(resultCount, ObjectMapper.Map<List<CustomerGetListOutput>>(@entities));
        }


        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_Update, AppPermissions.Pages_Tenant_Customer_UpdatePOS, AppPermissions.Pages_Tenant_SetUp_Customer_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateCustomerInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _customerManager.GetAsync(input.Id, true); //this is vendor

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(
                        userId,
                        input.CustomerCode,
                        input.CustomerName,
                        input.Email,
                        input.Website,
                        input.Remark,
                        input.BillingAddress,
                        input.ShippingAddress,
                        input.SameAsShippngAddress,
                        input.PhoneNumber,
                        input.AccountId,
                        input.CustomerTypeId,
                        input.IsWalkIn);
            @entity.UpdateMember(input.Member);
            #region update contactPerson
            if (input.ContactPersons.Where(s => s.IsPrimary).Select(s => s.IsPrimary).Take(2).ToList().Count() == 2)
            {
                throw new UserFriendlyException(L("DuplicatePrimary"));
            }
            var contactPersons = await _customerContactPersonRepository.GetAll().Where(u => u.CustomerId == entity.Id).ToListAsync();
            foreach (var c in input.ContactPersons)
            {
                if (c.Id != null)
                {
                    var @contactPerson = contactPersons.FirstOrDefault(u => u.Id == c.Id);
                    if (contactPerson != null)
                    {
                        //here is in only same vendor so no need to update vendor
                        contactPerson.UpdateContactPerson(userId, c.Title, c.FirstName, c.LastName, c.DisplayNameAs, c.IsPrimary, c.PhoneNumber, c.Email);
                        CheckErrors(await _customerContactPersonManager.UpdateAsync(contactPerson));
                    }
                }
                else if (c.Id == null)
                {
                    //@entity.Id is vendorId or input.Id is also vendor Id so no need to pass vendorId from outside
                    var @contactPerson = CustomerContactPerson.CreateContactPerson(tenantId, userId, c.Title, c.FirstName, c.LastName, c.DisplayNameAs, entity.Id, c.IsPrimary, c.PhoneNumber, c.Email);
                    CheckErrors(await _customerContactPersonManager.CreateAsync(@contactPerson));

                }
            }

            var @toDeleteContactPerson = contactPersons.Where(u => !input.ContactPersons.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteContactPerson)
            {
                CheckErrors(await _customerContactPersonManager.RemoveAsync(t));
            }


            #endregion
            #region update userGroup

            var customerGroupEntity = await _customerGroupRepository.GetAll().Where(u => u.CustomerId == entity.Id).ToListAsync();
            if (input.Member == Member.UserGroup)
            {
                foreach (var c in input.UserGroups)
                {
                    if (c.Id != null)
                    {
                        var customerGroup = customerGroupEntity.FirstOrDefault(u => u.Id == c.Id);
                        if (customerGroup != null)
                        {
                            customerGroup.Update(userId, entity.Id, c.UserGroupId);
                            CheckErrors(await _customerGroupManager.UpdateAsync(customerGroup));
                        }
                    }
                    else if (c.Id == null)
                    {
                        var customerGroup = CustomerGroup.Create(tenantId, userId, entity.Id, c.UserGroupId);
                        CheckErrors(await _customerGroupManager.CreateAsync(customerGroup));

                    }
                }

                var @toDeleteCustomerGroup = customerGroupEntity.Where(u => !input.UserGroups.Any(i => i.Id != null && i.Id == u.Id)).ToList();

                foreach (var t in @toDeleteCustomerGroup)
                {
                    CheckErrors(await _customerGroupManager.RemoveAsync(t));
                }
            }
            else
            {
                foreach (var t in customerGroupEntity)
                {
                    CheckErrors(await _customerGroupManager.RemoveAsync(t));
                }
            }

            #endregion
            CheckErrors(await _customerManager.UpdateAsync(@entity));

            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }


        #region import and export to Excel
        private ReportOutput GetReportTemplateCustomer(bool hasAccountFeature)
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter

                      new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "CustomerType",
                        ColumnLength = 100,
                        ColumnTitle = "Customer Type",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsRequired = true,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "CustomerCode",
                        ColumnLength = 100,
                        ColumnTitle = "Customer Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsRequired = true,
                        MoreFunction = null,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "CustomerName",
                        ColumnLength = 120,
                        ColumnTitle = "Customer Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true,
                        IsRequired = true,
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 200,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = hasAccountFeature,
                        IsRequired = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Email",
                        ColumnLength = 120,
                        ColumnTitle = "Email",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Phone",
                        ColumnLength = 120,
                        ColumnTitle = "Phone Number",
                        ColumnType = ColumnType.String,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Website",
                        ColumnLength = 120,
                        ColumnTitle = "Website",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BillingAddress_Street",
                        ColumnLength = 120,
                        ColumnTitle = "BillingAddress Street",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BillingAddress_CityTown",
                        ColumnLength = 120,
                        ColumnTitle = "BillingAddress CityTown",
                        ColumnType = ColumnType.String,
                        SortOrder = 9,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BillingAddress_Province",
                        ColumnLength = 120,
                        ColumnTitle = "BillingAddress Province",
                        ColumnType = ColumnType.String,
                        SortOrder = 10,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BillingAddress_PostalCode",
                        ColumnLength = 120,
                        ColumnTitle = "BillingAddress PostalCode",
                        ColumnType = ColumnType.String,
                        SortOrder = 11,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "BillingAddress_Country",
                        ColumnLength = 120,
                        ColumnTitle = "BillingAddress Country",
                        ColumnType = ColumnType.String,
                        SortOrder = 12,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShippingAddress_Street",
                        ColumnLength = 120,
                        ColumnTitle = "ShippingAddress Street",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 13,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShippingAddress_CityTown",
                        ColumnLength = 120,
                        ColumnTitle = "ShippingAddress CityTown",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 14,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShippingAddress_Province",
                        ColumnLength = 120,
                        ColumnTitle = "ShippingAddress Province",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 15,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShippingAddress_PostalCode",
                        ColumnLength = 120,
                        ColumnTitle = "ShippingAddress PostalCode",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 16,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "ShippingAddress_Country",
                        ColumnLength = 120,
                        ColumnTitle = "ShippingAddress Country",
                        ColumnType = ColumnType.String,
                        SortOrder = 17,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Remark",
                        ColumnLength = 120,
                        ColumnTitle = "Remark",
                        ColumnType = ColumnType.String,
                        SortOrder = 18,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "SameAsShippngAddress",
                        ColumnLength = 120,
                        ColumnTitle = "SameAsShippngAddress",
                        ColumnType = ColumnType.String,
                        SortOrder = 19,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsActive",
                        ColumnLength = 120,
                        ColumnTitle = "Status",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 30,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    }
                },
                Groupby = "",
                HeaderTitle = "Customers",
                Sortby = "",
            };
            return result;
        }

        private ReportOutput GetReportTemplateSubCustomer()
        {
            ReportOutput result = new ReportOutput()
            {
                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter                    
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Title",
                        ColumnLength = 120,
                        ColumnTitle = "Title",
                        ColumnType = ColumnType.String,
                        SortOrder = 1,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "FirstName",
                        ColumnLength = 120,
                        ColumnTitle = "First Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },

                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "LastName",
                        ColumnLength = 120,
                        ColumnTitle = "Last Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "DisplayNameAs",
                        ColumnLength = 120,
                        ColumnTitle = "DisplayNameAs",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "PhoneNumber",
                        ColumnLength = 120,
                        ColumnTitle = "Phone Number",
                        ColumnType = ColumnType.String,
                        SortOrder = 5,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },

                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "IsPrimary",
                        ColumnLength = 120,
                        ColumnTitle = "IsPrimary",
                        ColumnType = ColumnType.Bool,
                        SortOrder = 6,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Email",
                        ColumnLength = 120,
                        ColumnTitle = "Email",
                        ColumnType = ColumnType.String,
                        SortOrder = 7,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                    new CollumnOutput {
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "CustomerName",
                        ColumnLength = 120,
                        ColumnTitle = "Customer Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "CustomerContactPerson",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_ExportExcel, AppPermissions.Pages_Tenant_SetUp_Customer_ExportExcel)]
        public async Task<FileDto> ExportExcel()
        {
            var inputItem = new GetCustomerListInput()
            {
                UsePagination = false
            };
            //var chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).ToListAsync();
            //var customerTypes = await _customerTypeRepository.GetAll().Select(t => t.CustomerTypeName).ToListAsync();
            var @itemData = await _customerRepository.GetAll()
                   .Include(u => u.Account)
                   .Include(u => u.CustomerType)
                   .ToListAsync();
            var subitems = (from sub in _customerContactPersonRepository.GetAll()
                            .Include(u => u.Customer)
                            .Where(i => itemData.Any(t => t.Id == i.CustomerId))
                            select new CreateOrUpdateCustomerContactPersonExprotInput
                            {
                                DisplayNameAs = sub.DisplayNameAs,
                                Id = sub.Id,
                                Email = sub.Email,
                                FirstName = sub.FirstName,
                                LastName = sub.LastName,
                                IsPrimary = sub.IsPrimary,
                                PhoneNumber = sub.PhoneNumber,
                                Title = sub.Title,
                                Customer = ObjectMapper.Map<CustomerDetailOutput>(sub.Customer)

                            }).ToList();

            var result = new FileDto();
            var sheetName = "Customer";
            var seetNameSubItem = "SubCustomers";
            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.

            using (var p = new ExcelPackage())
            {
                //A workbook must have at least on cell, so lets add one... 
                var ws = p.Workbook.Worksheets.Add(sheetName);
                ws.PrinterSettings.Orientation = eOrientation.Landscape;
                ws.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                ws.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                ws.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowTableHeader = 1;
                int colHeaderTable = 1;//start from row 1 of spreadsheet
                                       // write header collumn table

                var hasAccountFeature = IsEnabled(AppFeatures.AccountingFeature);
                var headerList = GetReportTemplateCustomer(hasAccountFeature);
                foreach (var i in headerList.ColumnInfo.Where(t => t.Visible).ToList())
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true,0,i.IsRequired);
                    ws.Column(colHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colHeaderTable += 1;
                }
                int colHeaderPropertyTable = colHeaderTable;
                #endregion Row 1

                #region Row Body 
                int rowBody = rowTableHeader + 1;//start from row header of spreadsheet
                int count = 1;
                // write body
                foreach (var i in itemData)
                {
                    int collumnCellBody = 1;
                    foreach (var h in headerList.ColumnInfo.Where(t => t.Visible).ToList())
                    {
                        if (h.ColumnName == "SameAsShippngAddress")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(ws, rowBody, collumnCellBody, strList, i.SameAsShippngAddress.ToString());
                        }
                        else
                        {
                            WriteBodyCustomers(ws, rowBody, collumnCellBody, h, i, count);
                        }

                        collumnCellBody += 1;
                    }
                    rowBody += 1;
                    count += 1;
                }

                #endregion Row Body             
                //A workbook must have at least on cell, so lets add one... 
                var subitem = p.Workbook.Worksheets.Add(seetNameSubItem);
                subitem.PrinterSettings.Orientation = eOrientation.Landscape;
                subitem.PrinterSettings.FitToPage = true;
                //ws.PrinterSettings.PaperSize = ePaperSize.A3; //set default format paper size 
                subitem.Cells.Style.Font.Size = DefaultFontSize; //Default font size for whole sheet
                subitem.Cells.Style.Font.Name = DefaultFontName; //Default Font name for whole sheet

                #region Row 1 Header Table
                int rowSubTableHeader = 1;
                int colSubHeaderTable = 1;//start from row 1 of spreadsheet
                // write header collumn table
                var headerSubList = GetReportTemplateSubCustomer();

                foreach (var i in headerSubList.ColumnInfo)
                {
                    AddTextToCell(subitem, rowSubTableHeader, colSubHeaderTable, i.ColumnTitle, true);

                    subitem.Column(colSubHeaderTable).Width = ConvertPixelToInches(i.ColumnLength);
                    colSubHeaderTable += 1;
                }
                #endregion Row 1

                #region Row Body 
                int rowSubBody = rowSubTableHeader + 1;//start from row header of spreadsheet
                int countsub = 1;
                // write body
                foreach (var i in subitems)
                {
                    int collumnSubCellBody = 1;
                    foreach (var h in headerSubList.ColumnInfo)
                    {
                        if (h.ColumnName == "IsPrimary")
                        {
                            List<string> strList = new List<string>();
                            strList.Add("True");
                            strList.Add("False");
                            AddDropdownList(subitem, rowSubBody, collumnSubCellBody, strList, i.IsPrimary.ToString());
                        }
                        else
                        {
                            WriteBodySubCustomers(subitem, rowSubBody, collumnSubCellBody, h, i, count);
                        }

                        collumnSubCellBody += 1;
                    }
                    rowSubBody += 1;
                    countsub += 1;
                }

                #endregion Row Body    

                result.FileName = $"Customer_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";

                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Customer_ImportExcel, AppPermissions.Pages_Tenant_SetUp_Customer_ImportExcel)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            // Open and read the XlSX file.
            var @items = _customerRepository.GetAll();
            // var @subitems = _customerContactPersonRepository.GetAll();            
            //var chartOfAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
            var customerTypes = await _customerTypeRepository.GetAll().AsNoTracking().ToListAsync();
            var hasAccountFeature = IsEnabled(AppFeatures.AccountingFeature);
            // var headerList = GetReportTemplateCustomer(hasAccountFeature);
            var indexHasAccountFeature = hasAccountFeature ? 0 : -1;

            var chartOfAccounts = hasAccountFeature ? await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync() : new List<ChartOfAccount>();
            var tenant = !hasAccountFeature ? await GetCurrentTenantAsync() : null;

            // var colIndex = headerList.ColumnInfo.Count();
            // List<createImport> lst = new List<createImport>();

            if (excelPackage != null)
            {
                // Get the work book in the file
                var workBook = excelPackage.Workbook;
                if (workBook != null)
                {
                    // retrive first worksheets
                    var worksheet = excelPackage.Workbook.Worksheets[0];

                    //loop all rows
                    for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                    {
                        Guid? accountId = Guid.Empty;
                        var CustomerType = worksheet.Cells[i, 1].Value?.ToString();
                        var CustomerCode = worksheet.Cells[i, 2].Value?.ToString();
                        var CustomerName = worksheet.Cells[i, 3].Value?.ToString();
                        var Account = worksheet.Cells[i, 4].Value?.ToString();
                        var Email = worksheet.Cells[i, 5 + indexHasAccountFeature].Value?.ToString();
                        var Phone = worksheet.Cells[i, 6 + indexHasAccountFeature].Value?.ToString();
                        var Website = worksheet.Cells[i, 7 + indexHasAccountFeature].Value?.ToString();
                        var BillingAddress_Street = worksheet.Cells[i, 8 + indexHasAccountFeature].Value?.ToString();
                        var BillingAddress_CityTown = worksheet.Cells[i, 9 + indexHasAccountFeature].Value?.ToString();
                        var BillingAddress_Province = worksheet.Cells[i, 10 + indexHasAccountFeature].Value?.ToString();
                        var BillingAddress_PostalCode = worksheet.Cells[i, 11 + indexHasAccountFeature].Value?.ToString();
                        var BillingAddress_Country = worksheet.Cells[i, 12 + indexHasAccountFeature].Value?.ToString();
                        var ShippingAddress_Street = worksheet.Cells[i, 13 + indexHasAccountFeature].Value?.ToString();
                        var ShippingAddress_CityTown = worksheet.Cells[i, 14 + indexHasAccountFeature].Value?.ToString();
                        var ShippingAddress_Province = worksheet.Cells[i, 15 + indexHasAccountFeature].Value?.ToString();
                        var ShippingAddress_PostalCode = worksheet.Cells[i, 16 + indexHasAccountFeature].Value?.ToString();
                        var ShippingAddress_Country = worksheet.Cells[i, 17 + indexHasAccountFeature].Value?.ToString();
                        var Remark = worksheet.Cells[i, 18 + indexHasAccountFeature].Value?.ToString();
                        var SameAsShippngAddress = worksheet.Cells[i, 19 + indexHasAccountFeature].Value?.ToString();

                        if (i > 1)
                        {
                            accountId = hasAccountFeature ? chartOfAccounts.Where(s => s.AccountName == Account.ToString()).Select(s => s.Id).FirstOrDefault() : tenant.CustomerAccountId;
                            long? customerTypeId = customerTypes.Where(s => s.CustomerTypeName == CustomerType).Select(s => s.Id).FirstOrDefault();
                            var subWorkSeet = excelPackage.Workbook.Worksheets[1];
                            List<CreateOrUpdateCustomerContactPersonInput> subitems = new List<CreateOrUpdateCustomerContactPersonInput>();
                            for (int s = subWorkSeet.Dimension.Start.Row; s <= subWorkSeet.Dimension.End.Row; s++)
                            {
                                if (s > 1)
                                {
                                    var Title = subWorkSeet.Cells[s, 1].Value?.ToString();
                                    var First_Name = subWorkSeet.Cells[s, 2].Value?.ToString();
                                    var Last_Name = subWorkSeet.Cells[s, 3].Value?.ToString();
                                    var DisplayNameAs = subWorkSeet.Cells[s, 4].Value?.ToString();
                                    var Phone_Number = subWorkSeet.Cells[s, 5].Value?.ToString();
                                    var IsPrimary = subWorkSeet.Cells[s, 6].Value;
                                    var S_Email = subWorkSeet.Cells[s, 7].Value?.ToString();
                                    var Customer_Name = subWorkSeet.Cells[s, 8].Value?.ToString();

                                    if (Customer_Name == CustomerName)
                                    {
                                        var subitem = new CreateOrUpdateCustomerContactPersonInput
                                        {
                                            DisplayNameAs = DisplayNameAs,
                                            FirstName = First_Name,
                                            LastName = Last_Name,
                                            Email = Email,
                                            IsPrimary = Convert.ToBoolean(IsPrimary),
                                            PhoneNumber = Phone_Number,
                                            Title = Title,
                                        };
                                        subitems.Add(subitem);
                                    }

                                }
                            }

                            var B_Address = new CAddress(

                                BillingAddress_CityTown,
                                BillingAddress_Country,
                                BillingAddress_PostalCode,
                                BillingAddress_Province,
                                BillingAddress_Street

                               );
                            var S_Address = new CAddress(
                                 ShippingAddress_CityTown,
                                 ShippingAddress_Country,
                                 ShippingAddress_PostalCode,
                                 ShippingAddress_Province,
                                 ShippingAddress_Street
                              );
                            var createInput = new CreateCustomerInput()
                            {
                                AccountId = accountId == Guid.Empty ? null : accountId,
                                CustomerTypeId = customerTypeId == 0 ? null : customerTypeId,
                                CustomerCode = CustomerCode,
                                CustomerName = CustomerName,
                                SameAsShippngAddress = Convert.ToBoolean(SameAsShippngAddress),
                                Email = Email,
                                PhoneNumber = Phone,
                                Remark = Remark,
                                Website = Website,
                                ContactPersons = subitems,
                                BillingAddress = B_Address,
                                ShippingAddress = Convert.ToBoolean(SameAsShippngAddress) ? B_Address : S_Address,
                                Member = Member.All
                            };

                            if (items.Where(t => t.CustomerCode == createInput.CustomerCode).Count() == 0)
                            {
                                await Create(createInput);
                            }
                        }
                    }
                }
            }
            //RemoveFile(input, _appFolders);
        }
        #endregion
    }
}
