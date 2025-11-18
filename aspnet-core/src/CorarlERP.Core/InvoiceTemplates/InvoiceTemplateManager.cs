using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Abp.UI;

namespace CorarlERP.InvoiceTemplates
{
    public class InvoiceTemplateManager : CorarlERPDomainServiceBase, IInvoiceTemplateManager
    {
        private readonly IRepository<InvoiceTemplate, Guid> _repository;

        public InvoiceTemplateManager(IRepository<InvoiceTemplate, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IdentityResult> CreateAsync(InvoiceTemplate entity)
        {
            await CheckDuplicate(@entity);
            await _repository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<InvoiceTemplate> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _repository.GetAll() : _repository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(InvoiceTemplate entity)
        {
            await CheckDuplicate(@entity);

            await _repository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(InvoiceTemplate entity)
        {
            await _repository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(InvoiceTemplate entity)
        {
            @entity.SetIsActive(true);
            await _repository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(InvoiceTemplate entity)
        {
            @entity.SetIsActive(false);
            await _repository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicate(InvoiceTemplate @entity)
        {
            var find = await _repository.GetAll()                                
                             .Where(u => u.Id != entity.Id && u.Name.ToLower() == entity.Name.ToLower())
                             .AsNoTracking()
                             .FirstOrDefaultAsync();

            if (find != null)
            {
                throw new UserFriendlyException(L("DuplicateTemplate", entity.Name));
            }
        }
    }
}
