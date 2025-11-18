using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PaymentMethods
{
    public class PaymentMethodManager : CorarlERPDomainServiceBase, IPaymentMethodManager
    {
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;

        public PaymentMethodManager(IRepository<PaymentMethod, Guid> PaymentMethod)
        {
            _paymentMethodRepository = PaymentMethod;
        }

        public async Task<IdentityResult> CreateAsync(PaymentMethod entity)
        {
            
            await _paymentMethodRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PaymentMethod> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _paymentMethodRepository.GetAll().Include(u => u.Account).Include(s => s.PaymentMethodBase) :
              _paymentMethodRepository.GetAll().Include(u => u.Account).Include(s => s.PaymentMethodBase).AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PaymentMethod entity)
        {
            await _paymentMethodRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PaymentMethod entity)
        {

            await _paymentMethodRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(PaymentMethod entity)
        {
            @entity.Enable(false);
            await _paymentMethodRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(PaymentMethod entity)
        {
            @entity.Enable(true);
            await _paymentMethodRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }


}
