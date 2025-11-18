using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;

using CorarlERP.Vendors.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Abp.Authorization;
using CorarlERP.Authorization;
using CorarlERP.Reports.Dto;
using CorarlERP.ReportTemplates;
using CorarlERP.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Reports;
using OfficeOpenXml;
using System.IO;
using CorarlERP.Addresses;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.UserGroups;
using CorarlERP.VendorTypes;
using CorarlERP.FileStorages;
using CorarlERP.Features;
using CorarlERP.MultiTenancy;
using Abp.Application.Features;

namespace CorarlERP.Vendors
{
    public class VendorAppService : ReportBaseClass, IVendorAppService
    {
        private readonly IVendorManager _vendorManager;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IContactPersonManager _contactPersonManager;
        private readonly IRepository<ContactPreson, Guid> _contactPersonRepository;
        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly AppFolders _appFolders;
        private readonly IRepository<VendorType, long> _vendorTypeRepository;

        private readonly IVendorGroupManager _vendorGroupManager;
        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;

        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        private readonly IFileStorageManager _fileStorageManager;
        private readonly IFeatureChecker _featureChecker;
        public VendorAppService(IVendorManager VendorManager, IContactPersonManager contactPersonManager,
                           AppFolders appFolders,
                           IFileStorageManager fileStorageManager,
                           IRepository<ChartOfAccount, Guid> chartOfAccountRepository,
                           IRepository<Vendor, Guid> VendorRepository,
                           IRepository<ContactPreson, Guid> contactPersonRepository,
                           IVendorGroupManager vendorGroupManager,
                           IRepository<VendorGroup, Guid> vendorGroupRepository,
                           IRepository<VendorType, long> vendorTypeRepository,
                           IFeatureChecker featureChecker,
                           IRepository<UserGroupMember, Guid> userGroupMemberRepository
            ) : base(null, appFolders, userGroupMemberRepository, null)
        {
            _vendorTypeRepository = vendorTypeRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _vendorGroupManager = vendorGroupManager;
            _vendorGroupRepository = vendorGroupRepository;
            _vendorManager = VendorManager;
            _vendorRepository = VendorRepository;
            _contactPersonManager = contactPersonManager;
            _contactPersonRepository = contactPersonRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _appFolders = appFolders;
            _fileStorageManager = fileStorageManager;
            _featureChecker = featureChecker;
        }


        private async Task CheckMaxVendorCountAsync(int tenantId)
        {
            var maxVendorCount = (await _featureChecker.GetValueAsync(tenantId, AppFeatures.MaxVendorCount)).To<int>();
            if (maxVendorCount <= 0)
            {
                return;
            }

            var currentVendorCount = await _vendorRepository.CountAsync();
            if (currentVendorCount >= maxVendorCount)
            {
                throw new UserFriendlyException(L("MaximumVendorCount_Error_Detail", maxVendorCount));
            }
        }

        public async Task<ValidationCountOutput> CheckMaxVendorCount()
        {
            var maxVendorCount = (await _featureChecker.GetValueAsync(AbpSession.TenantId.Value, AppFeatures.MaxVendorCount)).To<int>();           
            var currentVendorCount = await _vendorRepository.CountAsync();
            return new ValidationCountOutput { MaxCount = maxVendorCount, MaxCurrentCount = currentVendorCount };
           
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Create, AppPermissions.Pages_Tenant_SetUp_Vendor_Create)]
        public async Task<NullableIdDto<Guid>> Create(CreateVendorInput input)
        {
            var tenantId = AbpSession.TenantId;
            var userId = AbpSession.GetUserId();
            if (tenantId != null)
            {
                await this.CheckMaxVendorCountAsync(tenantId.Value);
            }
            var @entity = Vendor.Create(tenantId, userId, input.VendorCode, input.VendorName, 
                                        input.Email, input.Website, input.Remark,
                                        input.BillingAddress, input.ShippingAddress, input.SameAsShippngAddress,
                                        input.PhoneNumber, input.AccountId, input.VendorTypeId);
            @entity.UpdateMember(input.Member);

            #region contactperson
            if (input.ContactPersons.Where(s => s.IsPrimary).Select(s => s.IsPrimary).Take(2).ToList().Count() == 2)
            {
                throw new UserFriendlyException(L("DuplicatePrimary"));
            }
            foreach (var contactPerson in input.ContactPersons)
            {
                var @contactPersons = ContactPreson.CreateContactPerson(tenantId, userId, contactPerson.Title, contactPerson.FirstName, contactPerson.LastName, contactPerson.DisplayNameAs, @entity, contactPerson.IsPrimary, contactPerson.PhoneNumber, contactPerson.Email);
                CheckErrors(await _contactPersonManager.CreateAsync(@contactPersons));
            }
            #endregion


            #region vendor Group
            if (input.UserGroups != null && input.UserGroups.Count > 0)
            {
                foreach (var c in input.UserGroups)
                {
                    var vendorGroup = VendorGroup.Create(tenantId.Value, userId, entity.Id, c.UserGroupId);
                    CheckErrors(await _vendorGroupManager.CreateAsync(vendorGroup));
                }
            }
            #endregion

            CheckErrors(await _vendorManager.CreateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Delete, AppPermissions.Pages_Tenant_SetUp_Vendor_Delete)]
        public async Task Delete(EntityDto<Guid> input)
        {
            var @entity = await _vendorManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var contactPersons = await _contactPersonRepository.GetAll().Where(u => u.VenderId == entity.Id).ToListAsync();

            foreach (var s in contactPersons)
            {
                CheckErrors(await _contactPersonManager.RemoveAsync(s));
            }

            CheckErrors(await _vendorManager.RemoveAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Disable, AppPermissions.Pages_Tenant_SetUp_Vendor_Disable)]
        public async Task Disable(EntityDto<Guid> input)
        {
            var @entity = await _vendorManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _vendorManager.DisableAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Enable, AppPermissions.Pages_Tenant_SetUp_Vendor_Enable)]
        public async Task Enable(EntityDto<Guid> input)
        {
            var @entity = await _vendorManager.GetAsync(input.Id, true);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            CheckErrors(await _vendorManager.EnableAsync(@entity));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Find, AppPermissions.Pages_Tenant_SetUp_Vendor)]
        public async Task<PagedResultDto<VendorGetListOutPut>> Find(GetVendorListInput input)
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

            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();


            var @query = _vendorGroupRepository.GetAll()
                            .Include(u => u.Vendor)
                            .Include(u => u.Vendor.ChartOfAccount)
                            .Where(t => t.Vendor.Member == Member.UserGroup)
                            .Where(t => userGroups.Contains(t.UserGroupId))
                            .WhereIf(input.IsActive != null, p => p.Vendor.IsActive == input.IsActive.Value)
                            .WhereIf(input.VendorTypes != null && input.VendorTypes.Any(), p => input.VendorTypes.Contains(p.Vendor.VendorTypeId.Value))
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.Vendor.VendorName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.Vendor.VendorCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(t => t.Vendor);

            var @queryAll = _vendorRepository.GetAll()
                            .Include(u => u.ChartOfAccount)
                            .Where(t => t.Member == Member.All)
                            .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                            .WhereIf(input.VendorTypes != null && input.VendorTypes.Any(), p => input.VendorTypes.Contains(p.VendorTypeId.Value))
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.VendorName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.VendorCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking();

            var resultCount = await @queryAll.Union(query).CountAsync();
            var @entities = await @queryAll.Union(query)
                .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value))
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();
            return new PagedResultDto<VendorGetListOutPut>(resultCount, ObjectMapper.Map<List<VendorGetListOutPut>>(@entities));
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_GetDetail, AppPermissions.Pages_Tenant_SetUp_Vendor_GetDetail)]
        public async Task<VendorDetailOutput> GetDetail(EntityDto<Guid> input)
        {
            var @entity = await _vendorManager.GetAsync(input.Id);

            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            var @contactPersons = await _contactPersonRepository.GetAll()
                                .Where(u => u.VenderId == entity.Id).AsNoTracking().ToListAsync();

            var result = ObjectMapper.Map<VendorDetailOutput>(@entity);

            result.ContactPersons = ObjectMapper.Map<List<ContactPersonDetailOut>>(@contactPersons);
            result.UserGroups = await _vendorGroupRepository.GetAll().Include(t => t.UserGroup)
                            .Where(u => u.VendorId == entity.Id).AsNoTracking()
                            .Select(t => new GroupItems
                            {
                                Id = t.Id,
                                UserGroupId = t.UserGroupId,
                                UserGroupName = t.UserGroup.Name
                            })
                            .ToListAsync();

            return result;
        }
        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_GetList, AppPermissions.Pages_Tenant_SetUp_Vendor_GetList)]
        public async Task<PagedResultDto<VendorGetListOutPut>> GetList(GetVendorListInput input)
        {
            var vendoTypeMemberIds = await GetVendorTypeMembers().Select(s => s.VendorTypeId).ToListAsync();

            var @query = _vendorRepository.GetAll()
                            .Include(u => u.ChartOfAccount)
                            .AsNoTracking()
                            .WhereIf(input.VendorTypes != null, p => input.VendorTypes.Contains(p.VendorTypeId.Value))
                            .WhereIf(input.IsActive != null, p => p.IsActive == input.IsActive.Value)
                            .WhereIf(vendoTypeMemberIds.Any(), s => vendoTypeMemberIds.Contains(s.VendorTypeId.Value))
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.VendorName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.VendorCode.ToLower().Contains(input.Filter.ToLower())
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
            return new PagedResultDto<VendorGetListOutPut>(resultCount, ObjectMapper.Map<List<VendorGetListOutPut>>(@entities));
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_Update, AppPermissions.Pages_Tenant_SetUp_Vendor_Update)]
        public async Task<NullableIdDto<Guid>> Update(UpdateVendorInput input)
        {
            var tenantId = AbpSession.GetTenantId();
            var userId = AbpSession.GetUserId();

            var @entity = await _vendorManager.GetAsync(input.Id, true); //this is vendor
            if (entity == null)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }

            entity.Update(
                        userId,
                        input.VendorCode,
                        input.VendorName,
                        input.Email,
                        input.Website,
                        input.Remark,
                        input.BillingAddress,
                        input.ShippingAddress,
                        input.SameAsShippngAddress,
                        input.PhoneNumber,
                        input.AccountId,
                        input.VendorTypeId);
            @entity.UpdateMember(input.Member);

            #region update contactPerson
            if (input.ContactPersons.Where(s => s.IsPrimary).Select(s => s.IsPrimary).Take(2).ToList().Count() == 2)
            {
                throw new UserFriendlyException(L("DuplicatePrimary"));
            }
            var contactPersons = await _contactPersonRepository.GetAll().Where(u => u.VenderId == entity.Id).ToListAsync();
            foreach (var c in input.ContactPersons)
            {
                if (c.Id != null)
                {
                    var @contactPerson = contactPersons.FirstOrDefault(u => u.Id == c.Id);
                    if (contactPerson != null)
                    {
                        //here is in only same vendor so no need to update vendor
                        contactPerson.UpdateContactPerson(userId, c.Title, c.FirstName, c.LastName, c.DisplayNameAs, c.IsPrimary, c.PhoneNumber, c.Email);
                        CheckErrors(await _contactPersonManager.UpdateAsync(contactPerson));
                    }
                }
                else if (c.Id == null)
                {
                    //@entity.Id is vendorId or input.Id is also vendor Id so no need to pass vendorId from outside
                    var @contactPerson = ContactPreson.CreateContactPerson(tenantId, userId, c.Title, c.FirstName, c.LastName, c.DisplayNameAs, entity.Id, c.IsPrimary, c.PhoneNumber, c.Email);
                    CheckErrors(await _contactPersonManager.CreateAsync(@contactPerson));

                }
            }

            var @toDeleteContactPerson = contactPersons.Where(u => !input.ContactPersons.Any(i => i.Id != null && i.Id == u.Id)).ToList();

            foreach (var t in toDeleteContactPerson)
            {
                CheckErrors(await _contactPersonManager.RemoveAsync(t));
            }


            #endregion

            #region update userGroup
            
            var vendorGroupEntity = await _vendorGroupRepository.GetAll().Where(u => u.VendorId == entity.Id).ToListAsync();
            if (input.Member == Member.UserGroup)
            {
                foreach (var c in input.UserGroups)
                {
                    if (c.Id != null)
                    {
                        var @vendorGroup = vendorGroupEntity.FirstOrDefault(u => u.Id == c.Id);
                        if (@vendorGroup != null)
                        {
                            @vendorGroup.Update(userId, entity.Id, c.UserGroupId);
                            CheckErrors(await _vendorGroupManager.UpdateAsync(@vendorGroup));
                        }
                    }
                    else if (c.Id == null)
                    {
                        var @vendorGroup = VendorGroup.Create(tenantId, userId, entity.Id, c.UserGroupId);
                        CheckErrors(await _vendorGroupManager.CreateAsync(@vendorGroup));

                    }
                }

                var @toDeleteVendorGroup = vendorGroupEntity.Where(u => !input.UserGroups.Any(i => i.Id != null && i.Id == u.Id)).ToList();

                foreach (var t in @toDeleteVendorGroup)
                {
                    CheckErrors(await _vendorGroupManager.RemoveAsync(t));
                }
            }
            else
            {
                foreach (var t in vendorGroupEntity)
                {
                    CheckErrors(await _vendorGroupManager.RemoveAsync(t));
                }
            }
            #endregion

            CheckErrors(await _vendorManager.UpdateAsync(@entity));
            await CurrentUnitOfWork.SaveChangesAsync();
            
            return new NullableIdDto<Guid>() { Id = entity.Id };
        }

        #region import and export to Excel
        private ReportOutput GetReportTemplateVendor(bool hasAccountFeature)
        {
            ReportOutput result = new ReportOutput()
            {

                ColumnInfo = new List<CollumnOutput>() {
                     // start properties with can filter

                     new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = true,
                        ColumnName = "VendorType",
                        ColumnLength = 100,
                        ColumnTitle = "Vendor Type",
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
                        ColumnName = "VendorCode",
                        ColumnLength = 100,
                        ColumnTitle = "Vendor Code",
                        ColumnType = ColumnType.String,
                        SortOrder = 2,
                        Visible = true,
                        AllowFunction = null,
                        MoreFunction = null,
                        IsRequired = true,
                        IsDisplay = false//no need to show in col check it belong to filter option
                    },

                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "VendorName",
                        ColumnLength = 120,
                        ColumnTitle = "Vendor Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 3,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        IsRequired = true,
                        DisableDefault = true
                    },
                    new CollumnOutput{
                        AllowGroupby = false,
                        AllowFilter = false,
                        ColumnName = "Account",
                        ColumnLength = 200,
                        ColumnTitle = "Account",
                        ColumnType = ColumnType.String,
                        SortOrder = 4,
                        IsRequired = true,
                        Visible = hasAccountFeature,
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
                HeaderTitle = "Vendors",
                Sortby = "",
            };
            return result;
        }

        private ReportOutput GetReportTemplateSubVendor()
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
                        ColumnName = "VendorName",
                        ColumnLength = 120,
                        ColumnTitle = "Vendor Name",
                        ColumnType = ColumnType.String,
                        SortOrder = 8,
                        Visible = true,
                        AllowFunction = null,
                        IsDisplay = true,
                        DisableDefault = false
                    },
                },
                Groupby = "",
                HeaderTitle = "VendorContactPerson",
                Sortby = "",
            };
            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_ExportExcel, AppPermissions.Pages_Tenant_SetUp_Vendor_ExportExcel)]
        public async Task<FileDto> ExportExcel()
        {
            var inputItem = new GetVendorListInput()
            {
                UsePagination = false
            };
            var chartOfAccounts = await _chartOfAccountRepository.GetAll().Select(t => t.AccountName).ToListAsync();
            var @itemData = await _vendorRepository.GetAll()
                   .Include(u => u.ChartOfAccount)
                   .Include(u => u.VendorType)
                   .ToListAsync();
            var subitems = (from sub in _contactPersonRepository.GetAll().Include(u => u.Vendor)
                            .Where(i => itemData.Any(t => t.Id == i.VenderId))
                            select new CreateOrUpdateContactPersonExcelInput
                            {
                                DisplayNameAs = sub.DisplayNameAs,
                                Id = sub.Id,
                                Email = sub.Email,
                                FirstName = sub.FirstName,
                                LastName = sub.LastName,
                                IsPrimary = sub.IsPrimary,
                                PhoneNumber = sub.PhoneNumber,
                                Title = sub.Title,
                                Vendors = ObjectMapper.Map<VendorSummaryOutput>(sub.Vendor)
                            }).ToList();

            var result = new FileDto();
            var sheetName = "Vendors";
            var seetNameSubItem = "SubVendors";
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
                var headerList = GetReportTemplateVendor(hasAccountFeature);              
                foreach (var i in headerList.ColumnInfo.Where(t=>t.Visible).ToList())
                {
                    AddTextToCell(ws, rowTableHeader, colHeaderTable, i.ColumnTitle, true,0 ,i.IsRequired);
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
                            WriteBodyVendors(ws, rowBody, collumnCellBody , h, i, count);
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
                var headerSubList = GetReportTemplateSubVendor();

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
                            WriteBodySubVendors(subitem, rowSubBody, collumnSubCellBody, h, i, count);
                        }

                        collumnSubCellBody += 1;
                    }
                    rowSubBody += 1;
                    countsub += 1;
                }

                #endregion Row Body    

                result.FileName = $"Vendor_Report.xlsx";
                result.FileToken = $"{Guid.NewGuid()}.xlsx";
                result.FileUrl = $"{_appFolders.DownloadBaseUrl}?fileName={result.FileName}&fileToken={result.FileToken}";
                
                await _fileStorageManager.UploadTempFile(result.FileToken, p);
            }

            return result;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_ImportExcel, AppPermissions.Pages_Tenant_SetUp_Vendor_ImportExcel)]
        public async Task ImportExcel(FileDto input)
        {
            //var excelPackage = Read(input, _appFolders);
            var excelPackage = await Read(input);

            // Open and read the XlSX file.
            var @items = await _vendorRepository.GetAll().ToListAsync();
            // var @subitems = _customerContactPersonRepository.GetAll();            
          
            var vendorTypes = await _vendorTypeRepository.GetAll().AsNoTracking().ToListAsync();
            var hasAccountFeature = IsEnabled(AppFeatures.AccountingFeature);
            var indexHasAccountFeature = hasAccountFeature ? 0 : -1;
            var chartOfAccounts = hasAccountFeature ? await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync() :new List<ChartOfAccount>();
            var tenant = !hasAccountFeature ? await GetCurrentTenantAsync() :null;
            // var headerList = GetReportTemplateVendor(hasAccountFeature);
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
                        var VendorType = worksheet.Cells[i, 1].Value;
                        var VendorCode = worksheet.Cells[i, 2].Value?.ToString();
                        var VendorName = worksheet.Cells[i, 3].Value?.ToString();
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
                        dynamic v;
                        if (i > 1)
                        {

                            //if (VendorType.ToString() == "Employee")
                            //{
                            //  v =  enumStatus.EnumStatus.VendorType.Employee;
                            //}
                            //else
                            //{
                            //    v = enumStatus.EnumStatus.VendorType.Supplier;
                            //}
                            var vendorTypeId = vendorTypes.Where(s => s.VendorTypeName == VendorType.ToString()).Select(s => s.Id).FirstOrDefault();

                            if(vendorTypeId == null || vendorTypeId == 0)
                            {
                                throw new UserFriendlyException(L("CanNotFindVendorTypeName"));
                            }
                            accountId = hasAccountFeature ? chartOfAccounts.Where(s => s.AccountName == Account.ToString()).Select(s => s.Id).FirstOrDefault() : tenant.VendorAccountId;
                            var subWorkSeet = excelPackage.Workbook.Worksheets[1];
                            List<CreateOrUpdateContactPersonInput> subitems = new List<CreateOrUpdateContactPersonInput>();
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

                                    if (Customer_Name == VendorName)
                                    {
                                        var subitem = new CreateOrUpdateContactPersonInput
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
                            
                            var createInput = new CreateVendorInput()
                            {
                                AccountId = accountId == Guid.Empty ? null : accountId,
                                VendorTypeId = vendorTypeId,
                                VendorCode = VendorCode,
                                VendorName = VendorName,
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

                            if (items.Where(t => t.VendorCode == createInput.VendorCode).Count() == 0)
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
