using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ItemIssues
{
    public class ItemIssueManager : CorarlERPDomainServiceBase, IItemIssueManager
    {

        private readonly IRepository<ItemIssue, Guid> _itemIssueRepository;

        public ItemIssueManager(IRepository<ItemIssue, Guid> itemIssueRepository)
        {
            _itemIssueRepository = itemIssueRepository;
        }
        public async Task<IdentityResult> CreateAsync(ItemIssue entity)
        {
            await _itemIssueRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ItemIssue> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _itemIssueRepository.GetAll()
               .Include(u => u.Customer)              
               .Include(u => u.TransactionTypeSale)
               :
               _itemIssueRepository.GetAll()
               .Include(u => u.Customer)              
               .Include(u => u.TransactionTypeSale)
               .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(ItemIssue entity)
        {
            await _itemIssueRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ItemIssue entity)
        {
            await _itemIssueRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
