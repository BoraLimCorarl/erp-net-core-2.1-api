using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Invoices
{
    public class InvoiceManager : CorarlERPDomainServiceBase, IInvoiceManager
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;

        public InvoiceManager(IRepository<Invoice, Guid> invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }


        public async Task<IdentityResult> CreateAsync(Invoice entity)
        {
            await _invoiceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Invoice> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _invoiceRepository.GetAll()              
                .Include(u => u.Customer)
                .Include(u=>u.TransactionTypeSale)
                :
                _invoiceRepository.GetAll()               
                .Include(u => u.Customer)
                .Include(u => u.TransactionTypeSale)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Invoice entity)
        {
            await _invoiceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Invoice entity)
        {
            await _invoiceRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
