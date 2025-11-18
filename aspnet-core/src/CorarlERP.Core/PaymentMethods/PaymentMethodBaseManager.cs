using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CorarlERP.PaymentMethods
{
   
    public class PaymentMethodBaseManager : CorarlERPDomainServiceBase, IPaymentMethodBaseManager
    {
        private readonly IRepository<PaymentMethodBase, Guid> _paymentMethodBaseRepository;

        public PaymentMethodBaseManager()
        {
            _paymentMethodBaseRepository = IocManager.Instance.Resolve<IRepository < PaymentMethodBase, Guid >>();
        }

        private async Task CheckDuplicate(PaymentMethodBase @entity)
        {
            var @old = await _paymentMethodBaseRepository.GetAll().AsNoTracking()
                           .Where(u =>
                                       u.Name.ToLower() == entity.Name.ToLower() &&
                                       u.Id != entity.Id)
                           .FirstOrDefaultAsync();

            if (old != null)
            {
                throw new UserFriendlyException(L("DuplicateName", entity.Name));
            }
        }


        public async Task<IdentityResult> CreateAsync(PaymentMethodBase entity)
        {
            await CheckDuplicate(entity);

            await _paymentMethodBaseRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<PaymentMethodBase> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _paymentMethodBaseRepository.GetAll() :
              _paymentMethodBaseRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(PaymentMethodBase entity)
        {
            await _paymentMethodBaseRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(PaymentMethodBase entity)
        {
            await _paymentMethodBaseRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(PaymentMethodBase entity)
        {
            @entity.Enable(false);
            await _paymentMethodBaseRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(PaymentMethodBase entity)
        {
            @entity.Enable(true);
            await _paymentMethodBaseRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }


}
