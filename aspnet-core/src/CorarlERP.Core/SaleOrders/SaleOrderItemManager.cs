using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.SaleOrders
{
    public class SaleOrderItemManager : CorarlERPDomainServiceBase, ISaleOrderItemManager
    {
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        public SaleOrderItemManager(IRepository<SaleOrderItem, Guid> saleOrderItemRepository)
        {
            _saleOrderItemRepository = saleOrderItemRepository;
        }
        public async Task<IdentityResult> CreateAsync(SaleOrderItem pEntity)
        {
            await _saleOrderItemRepository.InsertAsync(pEntity);
            return IdentityResult.Success;
        }

        public async Task<SaleOrderItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _saleOrderItemRepository.GetAll() : _saleOrderItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(SaleOrderItem entity)
        {
            await _saleOrderItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(SaleOrderItem entity)
        {
            await _saleOrderItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

    }
}
