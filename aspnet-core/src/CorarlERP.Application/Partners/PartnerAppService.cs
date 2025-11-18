using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using CorarlERP.Authorization;
using CorarlERP.Customers;
using CorarlERP.Partners.Dto;
using CorarlERP.UserGroups;
using CorarlERP.Vendors;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Linq;
using Abp.Runtime.Session;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Partners
{
    [AbpAuthorize]
    public class PartnerAppService : CorarlERPAppServiceBase, IPartnerAppService
    {
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;

        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;
        private readonly IRepository<CustomerGroup, Guid> _customerGroupRepository;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;
        public PartnerAppService(
           IRepository<Vendor, Guid> vendorRepository,
           IRepository<Locations.Location, long> locationRepository,
           IRepository<Customer, Guid> customerRepository,
           IRepository<UserGroupMember, Guid> userGroupMemberRepository,
           IRepository<VendorGroup, Guid> vendorGroupRepository,
           IRepository<CustomerGroup, Guid> customerGroupRepository
       ) : base(userGroupMemberRepository, locationRepository)
        {
            _customerGroupRepository = customerGroupRepository;
            _vendorGroupRepository = vendorGroupRepository;
            _customerRepository = customerRepository;
            _userGroupMemberRepository = userGroupMemberRepository;
            _vendorRepository = vendorRepository;
        }

        [AbpAuthorize(AppPermissions.Pages_Tenant_Vendor_GetList,
                     AppPermissions.Pages_Tenant_Customer_GetList)]
        public async Task<PagedResultDto<GetListPartnerOutPut>> GetList(GetPartnerListInput input)
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
            //vendor repos
            var @queryVendorGroup = _vendorGroupRepository.GetAll()
                            .Include(u => u.Vendor)
                            .Where(t => t.Vendor.Member == Member.UserGroup && userGroups.Contains(t.UserGroupId) && t.Vendor.IsActive == true)
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.Vendor.VendorName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.Vendor.VendorCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(t => t.Vendor);

            var @queryVendorAll = _vendorRepository.GetAll()
                            .Where(t => t.Member == Member.All && t.IsActive == true)
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.VendorName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.VendorCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking();

            var @queryVendor = @queryVendorAll.Union(@queryVendorGroup)
                            .Select( t => new GetListPartnerOutPut {
                                Id = t.Id,
                                PartnerCode = t.VendorCode,
                                PartnerName = t.VendorName,
                                PartnerType = PartnerType.Vendor
                            });

            //Customer repos
            var @queryCustomerGroup = _customerGroupRepository.GetAll()
                            .Include(u => u.Customer)
                            .Where(t => t.Customer.Member == Member.UserGroup && userGroups.Contains(t.UserGroupId) && t.Customer.IsActive == true)
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.Customer.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.Customer.CustomerCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking()
                            .Select(t => t.Customer);

            var @queryCustomerAll = _customerRepository.GetAll()
                            .Where(t => t.Member == Member.All && t.IsActive == true)
                            .WhereIf(
                                !input.Filter.IsNullOrEmpty(),
                                p => p.CustomerName.ToLower().Contains(input.Filter.ToLower()) ||
                                     p.CustomerCode.ToLower().Contains(input.Filter.ToLower()))
                            .AsNoTracking();
            var @queryCustomer = @queryCustomerAll.Union(@queryCustomerGroup)
                            .Select(t => new GetListPartnerOutPut
                            {
                                Id = t.Id,
                                PartnerCode = t.CustomerCode,
                                PartnerName = t.CustomerName,
                                PartnerType = PartnerType.Customer
                            });

            var query = @queryVendor.Union(@queryCustomer);
            var resultCount = await query.CountAsync();            
            var entities = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            
            return new PagedResultDto<GetListPartnerOutPut>(resultCount, ObjectMapper.Map<List<GetListPartnerOutPut>>(@entities));
        }
    }
}
