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
    public class InvoiceTemplateMapManager : CorarlERPDomainServiceBase, IInvoiceTemplateMapManager
    {
        private readonly IRepository<InvoiceTemplateMap, Guid> _repository;

        public InvoiceTemplateMapManager(IRepository<InvoiceTemplateMap, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IdentityResult> CreateAsync(InvoiceTemplateMap entity)
        {
            await CheckDuplicate(@entity);
            await _repository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<InvoiceTemplateMap> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _repository.GetAll() : _repository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> UpdateAsync(InvoiceTemplateMap entity)
        {
            await CheckDuplicate(@entity);

            await _repository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(InvoiceTemplateMap entity)
        {
            await _repository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicate(InvoiceTemplateMap @entity)
        {
            var find = await _repository.GetAll()                                
                             .Where(u => u.Id != entity.Id && u.TemplateType == entity.TemplateType && u.SaleTypeId == entity.SaleTypeId)
                             .AsNoTracking()
                             .FirstOrDefaultAsync();

            if (find != null)
            {
                throw new UserFriendlyException(L("DuplicateTemplateMap"));
            }
        }

    }
}
