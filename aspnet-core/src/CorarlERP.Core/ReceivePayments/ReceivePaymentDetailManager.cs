using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ReceivePayments
{
    public class ReceivePaymentDetailManager : CorarlERPDomainServiceBase, IReceivePaymentDetailManager
    {
        private readonly IRepository<ReceivePaymentDetail, Guid> _receivePaymentDetailRepository;

        public ReceivePaymentDetailManager(IRepository<ReceivePaymentDetail, Guid> receivePaymentDetailRepository)
        {
            _receivePaymentDetailRepository = receivePaymentDetailRepository;
        }

        public async Task<IdentityResult> CreateAsync(ReceivePaymentDetail entity)
        {
            await _receivePaymentDetailRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ReceivePaymentDetail> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _receivePaymentDetailRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.Invoice)
                :
                _receivePaymentDetailRepository.GetAll()
                .Include(u => u.Customer)
                .Include(u => u.Invoice)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(ReceivePaymentDetail entity)
        {
            await _receivePaymentDetailRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ReceivePaymentDetail entity)
        {
            await _receivePaymentDetailRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
