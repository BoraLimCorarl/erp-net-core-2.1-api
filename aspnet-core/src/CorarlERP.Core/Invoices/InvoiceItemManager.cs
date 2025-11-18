using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Invoices
{
    public class InvoiceItemManager : CorarlERPDomainServiceBase, IInvoiceItemManager
    {
        private readonly IRepository<InvoiceItem, Guid> _invoiceItemRepository;

        public InvoiceItemManager(IRepository<InvoiceItem, Guid> invoiceItemRepository)
        {
            _invoiceItemRepository = invoiceItemRepository;
        }

        public async Task<IdentityResult> CreateAsync(InvoiceItem entity)
        {
            await _invoiceItemRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<InvoiceItem> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _invoiceItemRepository.GetAll() :
                _invoiceItemRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(InvoiceItem entity)
        {
            await _invoiceItemRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(InvoiceItem entity)
        {
            await _invoiceItemRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
