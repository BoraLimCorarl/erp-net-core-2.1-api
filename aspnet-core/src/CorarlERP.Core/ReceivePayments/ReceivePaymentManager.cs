using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.ReceivePayments
{
    public class ReceivePaymentManager : CorarlERPDomainServiceBase, IReceivePaymentManager
    {

        private readonly IRepository<ReceivePayment, Guid> _receivePaymentRepository;

        public ReceivePaymentManager(IRepository<ReceivePayment, Guid> receivePaymentRepository)
        {
            _receivePaymentRepository = receivePaymentRepository;
        }

        public async Task<IdentityResult> CreateAsync(ReceivePayment entity)
        {
            await _receivePaymentRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<ReceivePayment> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _receivePaymentRepository.GetAll() :
                _receivePaymentRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(ReceivePayment entity)
        {
            await _receivePaymentRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ReceivePayment entity)
        {
            await _receivePaymentRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
