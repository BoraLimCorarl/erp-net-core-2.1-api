using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using CorarlERP.Common.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using CorarlERP.MultiTenancy;
using System.Transactions;
using Abp.BackgroundJobs;
using Hangfire;
using Abp.Authorization;
using CorarlERP.Authorization;
using System.Collections.Generic;
using CorarlERP.Storage;
using CorarlERP.Galleries;
using System;
using CorarlERP.FileUploads;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.Authorization.Users;

namespace CorarlERP.HostMaintances
{
    public class HostMaintanceAppService : CorarlERPAppServiceBase, IHostMaintanceAppService
    {


        private readonly IHostMaintanceManager _maintenanceManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IFileUploadManager _fileUploadManager;
        private readonly ICorarlRepository<Tenant> _tenantRepository;
        private readonly ICorarlRepository<Gallery, Guid> _galleryRepository;
        private readonly ICorarlRepository<BinaryObject, Guid> _binaryObjectRepository;
        private readonly ICorarlRepository<JournalTransactionType, Guid> _transactionType;
        private readonly ICorarlRepository<User, long> _userRepository;
        public HostMaintanceAppService(
            IHostMaintanceManager maintenanceManager,
            IUnitOfWorkManager unitOfWorkManager,
            IBinaryObjectManager binaryObjectManager,
            IFileUploadManager fileUploadManager,
            ICorarlRepository<Tenant> tenantRepository,
            ICorarlRepository<Gallery, Guid> galleryRepository,
            ICorarlRepository<BinaryObject, Guid> binaryObjectRepository,
            ICorarlRepository<JournalTransactionType, Guid> transactionType,
            ICorarlRepository<User, long> userRepository
            )
        {
            _maintenanceManager = maintenanceManager;
            _unitOfWorkManager = unitOfWorkManager;
            _binaryObjectManager = binaryObjectManager;
            _fileUploadManager = fileUploadManager;
            _tenantRepository = tenantRepository;
            _galleryRepository = galleryRepository;
            _binaryObjectRepository = binaryObjectRepository;
            _transactionType = transactionType;
            _userRepository = userRepository;
        }


        /// <summary>
        /// Assign static permissions to all roles 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
        [UnitOfWork(IsDisabled = false)]
        public async Task AssignPermissionsToStaticRoles(HostTenantListInput input)
        {
            // var tenants = new List<GetTenantEditionInput>();
            if (input.Tenants.Count == 0)
            {
                input.Tenants = await GetAllTenantsAsync();
            }
            foreach (var tenant in input.Tenants)
            {
                /*
                 * Below code use hangefire so plese ask b.visal if need to use it or not
                 */
                BackgroundJob.Enqueue(() => _maintenanceManager.AssignPermissionsToTenant(tenant.Id, tenant.EditionId.Value));
                //await _maintenanceManager.AssignPermissionsToTenant(id);
            }
        }

        private async Task<List<GetTenantEditionInput>> GetAllTenantsAsync()
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                                                   AbpDataFilters.MustHaveTenant))
                {
                    var tenants = await TenantManager.Tenants
                              .AsNoTracking()
                              .Where(t => t.IsActive)
                              .Select(u => new GetTenantEditionInput { Id = u.Id, EditionId = u.EditionId }).ToListAsync();

                    return tenants;


                }

            }

        }
        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
        [UnitOfWork(IsDisabled = false)]
        public async Task<string> AsyncTransactionTypeKitchenOrder()
        {
            var message = "";
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                                                   AbpDataFilters.MustHaveTenant))
                {
                    var JournalTransactionTypes = new List<JournalTransactionType>();
                    var tenantIds = await _tenantRepository.GetAll().Select(s => s.Id).ToListAsync();
                    var transactionTypes = await _transactionType.GetAll().Where(s => s.InventoryTransactionType == enumStatus.EnumStatus.InventoryTransactionType.ItemIssueKitchenOrder).Select(s => s.TenantId).ToListAsync();
                    var tenantIdForCreate = tenantIds.Where(s => !transactionTypes.Contains(s)).ToList();
                    var userIds = await _userRepository.GetAll().AsNoTracking().Select(s => new { userId = s.Id, tenantId = s.TenantId }).ToListAsync();
                    foreach (var i in tenantIdForCreate)
                    {
                        var userId = userIds.Where(s => s.tenantId == i).Select(s => s.userId).FirstOrDefault();
                        var j = JournalTransactionType.Create(i, userId, "Kitchen Order", true, true, enumStatus.EnumStatus.InventoryTransactionType.ItemIssueKitchenOrder);
                        JournalTransactionTypes.Add(j);
                    }
                    if(JournalTransactionTypes.Count > 0) await _transactionType.BulkInsertAsync(JournalTransactionTypes);
                    message = $"Tenant to sync {JournalTransactionTypes.Count}";
                }
                await uow.CompleteAsync();
                return message;
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
        [UnitOfWork(IsDisabled = false)]
        public async Task AssignPermissionsToStaticRole(HostTenantListInput input)
        {
            if (input.Tenants.Count == 0)
            {
                input.Tenants = await GetAllTenantsAsync();
            }

            foreach (var id in input.Tenants)
            {

                /*
                 * Below code use hangefire so plese ask b.visal if need to use it or not
                 */
                BackgroundJob.Enqueue(() => _maintenanceManager.AssignPermissionForEachRole(id.Id, input.RoleName));

            }
        }




        [AbpAuthorize(AppPermissions.Pages_Administration_Host_Maintenance)]
        [UnitOfWork(IsDisabled = false)]
        public async Task<string> SysncLogFromBinaryToGallery()
        {

            var galleryIds = new List<Guid>();
            var tenants = new List<Tenant>();
            var binaryObjects = new List<BinaryObject>();

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                                                  AbpDataFilters.MustHaveTenant))
                {
                    galleryIds = await _galleryRepository.GetAll().AsNoTracking().Select(s => s.Id).ToListAsync();
                    tenants = await _tenantRepository.GetAll()
                                        .Where(s => s.LogoId.HasValue)
                                        .WhereIf(!galleryIds.IsNullOrEmpty(), s => !galleryIds.Contains(s.LogoId.Value))
                                        .ToListAsync();
                    var tenantIds = tenants.Select(s => s.Id).ToList();
                    binaryObjects = await _binaryObjectRepository.GetAll().Where(s => s.TenantId != null && tenantIds.Contains(s.TenantId.Value)).AsNoTracking().ToListAsync();
                }
            }


            if (tenants.Count == 0) return "No tenant to sync";

            var updateTenants = new List<Tenant>();
            var deleteLogos = new List<BinaryObject>();

            foreach (var tenant in tenants)
            {
                var logoObject = binaryObjects.Where(s => s.Id == tenant.LogoId).FirstOrDefault();
                if (logoObject != null)
                {
                    var types = tenant.LogoFileType?.Split("/");
                    var extension = types.IsNullOrEmpty() ? "png" : types.Last().ToLower();
                    var fileName = $"{tenant.TenancyName}-logo.{extension}";

                    var gallery = await _fileUploadManager.Upload(tenant.Id, tenant.LastModifierUserId ?? tenant.CreatorUserId, UploadFrom.Logo, logoObject.Bytes, tenant.LogoFileType, fileName);
                    tenant.SetLog(gallery.GalleryId);
                    updateTenants.Add(tenant);
                    deleteLogos.Add(logoObject);

                }

            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                                                  AbpDataFilters.MustHaveTenant))
                {
                    await _tenantRepository.BulkUpdateAsync(updateTenants);
                    await _binaryObjectRepository.BulkDeleteAsync(deleteLogos);
                }
                await uow.CompleteAsync();
            }
            return $"{tenants.Count} tenants were sync successful";
        }


    }
}
