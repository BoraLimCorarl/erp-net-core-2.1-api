using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.LayaltyAndMemberships
{
    public class CardManager : CorarlERPDomainServiceBase, ICardManager
    {
        private readonly IRepository<Card, Guid> _cardRepository;

        public CardManager(IRepository<Card, Guid> cardRepository)
        {
            _cardRepository = cardRepository;
        }
        private async Task CheckDuplicate(Card @entity)
        {
            var cards = await _cardRepository.GetAll().AsNoTracking()
                           .Where(u => u.CardStatus == CardStatus.Enable &&
                                       u.Id != entity.Id)
                           .ToListAsync();
            var @oldTagId = cards.Where(u => u.CardStatus == CardStatus.Enable && u.CardId.ToLower() == entity.CardId.ToLower() && u.Id != entity.Id).FirstOrDefault();
            var @oldCardNumber = cards.Where(u => u.CardStatus == CardStatus.Enable && u.CardNumber.ToLower() == entity.CardNumber.ToLower() && u.Id != entity.Id).FirstOrDefault();
            var @oldSerialNumber = cards.Where(u => u.CardStatus == CardStatus.Enable && u.SerialNumber.ToLower() == entity.SerialNumber.ToLower() && u.Id != entity.Id).FirstOrDefault();


            if (@oldTagId != null)
            {
                throw new UserFriendlyException(L("DuplicateCardId", entity.CardId));
            }
            else if(@oldCardNumber != null)
            {
                throw new UserFriendlyException(L("DuplicateCardNumber", entity.CardNumber));
            }
            else if (@oldSerialNumber != null)
            {
                throw new UserFriendlyException(L("DuplicateSerialNumber", entity.SerialNumber));
            }         
        }
        public async Task<IdentityResult> CreateAsync(Card entity)
        {
            await CheckDuplicate(entity);

            await _cardRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DisableAsync(Card entity)
        {
            @entity.Disable();
            await _cardRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

       public async Task<IdentityResult> DeactivateAsync(Card @entity)
        {
            entity.Deactivate();
            await _cardRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> EnableAsync(Card entity)
        {
            @entity.Enable();
            await _cardRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<Card> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _cardRepository.GetAll() :
                _cardRepository.GetAll().AsNoTracking();

            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IdentityResult> RemoveAsync(Card entity)
        {
            await _cardRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Card entity)
        {
            await CheckDuplicate(entity);
            await _cardRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }
    }
}
