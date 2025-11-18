using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PayBills
{
    public class PayBillDetailManager : CorarlERPDomainServiceBase, IPayBillDetailManager
    {

        private readonly IRepository<PayBillDetail, Guid> _payBillDetailRepository;

        public PayBillDetailManager(IRepository<PayBillDetail, Guid> payBillDetailRepository)
        {
            _payBillDetailRepository = payBillDetailRepository;
        }

        public async Task<IdentityResult> CreateAsync(PayBillDetail entity)
        {
            await _payBillDetailRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PayBillDetail> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _payBillDetailRepository.GetAll()
                .Include(u => u.Vendor)
                .Include(u=>u.Bill)
                .Include(u=>u.VendorCredit)
                :
                _payBillDetailRepository.GetAll()
                .Include(u => u.Vendor)
                .Include(u => u.VendorCredit)
                .Include(u => u.Bill)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PayBillDetail entity)
        {
            await _payBillDetailRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PayBillDetail entity)
        {
            await _payBillDetailRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
