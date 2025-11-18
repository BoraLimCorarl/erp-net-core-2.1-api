using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using CorarlERP.AccountCycles;
using CorarlERP.AutoSequences;
using CorarlERP.Bills;
using CorarlERP.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Journals
{
    public class JournalManager : CorarlERPDomainServiceBase, IJournalManager
    {
        private readonly IRepository<Journal, Guid> _journalRepository;
        private JournalType? _journalType;
        private readonly IRepository<Tenant, int> _tenantRepository;
        private readonly IAutoSequenceManager _autoSequenceManager;
        private readonly IRepository<AccountCycle, long> _accountCycleRepository;
        public JournalManager(IRepository<Journal, Guid> journalRepository,
            IRepository<Tenant, int> tenantRepository,
            IRepository<AccountCycle, long> accountCycleRepository,
            IAutoSequenceManager autoSequenceManager
        ) : base(accountCycleRepository)
        {
            _journalRepository = journalRepository;
            _tenantRepository = tenantRepository;
            _autoSequenceManager = autoSequenceManager;
            _accountCycleRepository = accountCycleRepository;
        }

        public void SetJournalType(JournalType journalType)
        {
            _journalType = journalType;
        }

        public async Task<IdentityResult> CreateAsync(Journal entity, bool noVlidate = false, bool checkDupliateReference = false)
        {
            ValidateJournal(entity);
            await CheckClosePeriod(entity.Date);
            if (!noVlidate) await CheckDuplicateGeneralJournal(entity);

            if (checkDupliateReference) await CheckDuplicateReferenceNo(entity);

            await _journalRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> DisableAsync(Journal entity)
        {
            ValidateJournal(entity);
            await CheckClosePeriod(entity.Date);
            @entity.Enable(false);
            await _journalRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> EnableAsync(Journal entity)
        {
            ValidateJournal(entity);
            await CheckClosePeriod(entity.Date);
            @entity.Enable(true);
            await _journalRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<Journal> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ?
                _journalRepository.GetAll()
                .Include(u => u.Class)
                .Include(u => u.Currency)
                .Include(u => u.GeneralJournal)
                .Include(u => u.Bill)
                .Include(u => u.PayBill)
                .Include(u => u.ItemReceipt)
                .Include(u => u.ItemReceiptCustomerCredit)
                .Include(u => u.withdraw)
                .Include(u=>u.Location)
                .WhereIf(_journalType != null, u => u.JournalType == _journalType)
                :
                _journalRepository.GetAll()
                .Include(u => u.Location)
                .Include(u => u.Class)
                .Include(u => u.withdraw)
                .Include(u => u.Currency)
                .Include(u => u.GeneralJournal)
                .Include(u => u.ItemReceipt)
                .Include(u => u.ItemReceiptCustomerCredit)
                .Include(u => u.Bill)
                .Include(u => u.PayBill)
                .WhereIf(_journalType != null, u => u.JournalType == _journalType)
                .AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async virtual Task<IdentityResult> RemoveAsync(Journal entity)
        {
            ValidateJournal(entity);
            await CheckClosePeriod(entity.Date);
            await _journalRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public async virtual Task<IdentityResult> UpdateAsync(Journal entity, DocumentType? documentType , bool IsDuplicate = true)
        {
            ValidateJournal(entity);
            if (IsDuplicate == true)
            {
                await CheckDuplicateGeneralJournal(entity);
            }
            await CheckClosePeriod(entity.Date);

            if (documentType != null && entity.Status == TransactionStatus.Publish)
            {
                var auto = await _autoSequenceManager.GetAutoSequenceAsync(documentType.Value);
                if (auto.RequireReference) await CheckDuplicateReferenceNo(entity);
            }
            await _journalRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        private async Task CheckDuplicateGeneralJournal(Journal @entity)
        {
            var @old = await _journalRepository.GetAll().AsNoTracking()
                           .WhereIf(entity?.JournalType != null, u => u.JournalType == entity.JournalType)
                           .Where(u => u.IsActive &&
                                       u.JournalNo.ToLower() == entity.JournalNo.ToLower() &&
                                       u.Id != entity.Id
                                       )
                           .FirstOrDefaultAsync();

            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.GeneralJournal))
            {
                throw new UserFriendlyException(L("DuplicateJournalNo", entity.JournalNo));
                
            }

            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.Bill))
            {
                throw new UserFriendlyException(L("DuplicateBillNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptPurchase))
            {
                throw new UserFriendlyException(L("DuplicateItemReceiptNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.PayBill))
            {
                throw new UserFriendlyException(L("DuplicatePaymentNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.VendorCredit))
            {
                throw new UserFriendlyException(L("DuplicateVendorCreditNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.Invoice))
            {
                Logger.Info($"Duplicate Invoice Error Line1: New.JournalNo = {entity.JournalNo}, Old.JournalNo = {old.JournalNo}, Old.TenantId = {old.TenantId}, New.TenantId = {entity.TenantId}");
                Logger.Info($"Duplicate Invoice Error Line2: New.Status = {entity.Status}, Old.Status = {old.Status}, Old.Reference = {old.Reference}, New.Reference = {entity.Reference}");
                Logger.Info($"Duplicate Invoice Error Line3: New.Id = {entity.Id}, Old.Id = {old.Id}, New.JouranlType = {entity.JournalType.ToString()},  Old.JouranlType = {old.JournalType.ToString()}");
                Logger.Info($"Duplicate Invoice Error Line4: NewInvoice.Id = {entity.InvoiceId}, OldInvoice.Id = {old.InvoiceId}");

                throw new UserFriendlyException(L("DuplicateInvoiceNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemIssueSale))
            {
                throw new UserFriendlyException(L("DuplicateItemIssueNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.CustomerCredit))
            {
                throw new UserFriendlyException(L("DuplicateCustomerCreditNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ReceivePayment))
            {
                throw new UserFriendlyException(L("DuplicateReceivePaymentNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptCustomerCredit))
            {
                throw new UserFriendlyException(L("DuplicateItemReceiptCustomerCreditNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemIssueVendorCredit))
            {
                throw new UserFriendlyException(L("DuplicateItemIssueVendorIssueNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemIssueAdjustment))
            {
                throw new UserFriendlyException(L("DuplicateItemIssueNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemIssueOther))
            {
                throw new UserFriendlyException(L("DuplicateItemIssueNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemIssueTransfer))
            {
                throw new UserFriendlyException(L("DuplicateItemIssueNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptAdjustment))
            {
                throw new UserFriendlyException(L("DuplicateItemReceiptNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptOther))
            {
                throw new UserFriendlyException(L("DuplicateItemReceiptNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptTransfer))
            {
                throw new UserFriendlyException(L("DuplicateItemReceiptNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.Deposit))
            {
                throw new UserFriendlyException(L("DuplicateDepositNo", entity.JournalNo));
            }
            if (old != null && old.JournalNo.ToLower() == entity.JournalNo.ToLower() && old.JournalType.Equals(JournalType.Withdraw))
            {
                throw new UserFriendlyException(L("DuplicateWithdrawNo", entity.JournalNo));
            }

        }

        private async Task CheckDuplicateReferenceNo(Journal @entity)
        {
            if (entity.Reference.IsNullOrEmpty())
            {
                throw new UserFriendlyException(L("RequireReferenceNo"));
            }

            if (entity.JournalType == JournalType.VendorCredit)
            {
                var vendorCredit = await _journalRepository.GetAll().AsNoTracking().Include(t => t.VendorCredit)
                                        .Where(t => t.JournalType == JournalType.VendorCredit)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.VendorCredit.VendorId == entity.VendorCredit.VendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (vendorCredit != null && vendorCredit.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }

            }
            else if (entity.JournalType == JournalType.ItemIssueVendorCredit)
            {
                var vendorCreditItemIssue = await _journalRepository.GetAll().AsNoTracking().Include(t => t.ItemIssueVendorCredit)
                                        .Where(t => t.JournalType == JournalType.ItemIssueVendorCredit)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.ItemIssueVendorCredit.VendorId == entity.ItemIssueVendorCredit.VendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (vendorCreditItemIssue != null && vendorCreditItemIssue.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.Bill)
            {
                var bill = await _journalRepository.GetAll().AsNoTracking().Include(t => t.Bill)
                                        .Where(t => t.JournalType == JournalType.Bill)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.Bill.VendorId == entity.Bill.VendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (bill != null && bill.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.Invoice)
            {
                var invoice = await _journalRepository.GetAll().AsNoTracking().Include(t => t.Invoice)
                                        .Where(t => t.JournalType == JournalType.Invoice)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.Invoice.CustomerId == entity.Invoice.CustomerId
                                        )
                                        .FirstOrDefaultAsync();

                if (invoice != null && invoice.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.ItemIssueSale)
            {
                var itemIssueSale = await _journalRepository.GetAll().AsNoTracking().Include(t => t.ItemIssue)
                                        .Where(t => t.JournalType == JournalType.ItemIssueSale)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.ItemIssue.CustomerId == entity.ItemIssue.CustomerId
                                        )
                                        .FirstOrDefaultAsync();

                if (itemIssueSale != null && itemIssueSale.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptPurchase)
            {
                var itemReceipt = await _journalRepository.GetAll().AsNoTracking().Include(t => t.ItemReceipt)
                                        .Where(t => t.JournalType == JournalType.ItemReceiptPurchase)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.ItemReceipt.VendorId == entity.ItemReceipt.VendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (itemReceipt != null && itemReceipt.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.ItemReceiptCustomerCredit)
            {
                var itemReceiptCustomerCredit = await _journalRepository.GetAll().AsNoTracking().Include(t => t.ItemReceiptCustomerCredit)
                                        .Where(t => t.JournalType == JournalType.ItemReceiptCustomerCredit)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.ItemReceiptCustomerCredit.CustomerId == entity.ItemReceiptCustomerCredit.CustomerId
                                        )
                                        .FirstOrDefaultAsync();

                if (itemReceiptCustomerCredit != null && itemReceiptCustomerCredit.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.CustomerCredit)
            {
                var customerCredit = await _journalRepository.GetAll().AsNoTracking().Include(t => t.CustomerCredit)
                                        .Where(t => t.JournalType == JournalType.CustomerCredit)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.CustomerCredit.CustomerId == entity.CustomerCredit.CustomerId
                                        )
                                        .FirstOrDefaultAsync();

                if (customerCredit != null && customerCredit.Reference.ToLower() == entity.Reference.ToLower() && customerCredit.JournalType.Equals(JournalType.CustomerCredit))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.Withdraw)
            {
                var withdraw = await _journalRepository.GetAll().AsNoTracking().Include(t => t.withdraw)
                                        .Where(t => t.JournalType == JournalType.Withdraw)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.withdraw.VendorId == entity.withdraw.VendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (withdraw != null && withdraw.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }
            else if (entity.JournalType == JournalType.Deposit)
            {
                var deposit = await _journalRepository.GetAll().AsNoTracking().Include(t => t.Deposit)
                                        .Where(t => t.JournalType == JournalType.Deposit)
                                        .Where(u => u.IsActive && u.Reference.ToLower() == entity.Reference.ToLower() &&
                                                    u.Id != entity.Id &&
                                                    u.Deposit.ReceiveFromVendorId == entity.Deposit.ReceiveFromVendorId
                                        )
                                        .FirstOrDefaultAsync();

                if (deposit != null && deposit.Reference.ToLower() == entity.Reference.ToLower())
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
            }

            else
            {
                var @old = await _journalRepository.GetAll().AsNoTracking()
                                .WhereIf(entity?.JournalType != null, u => u.JournalType == entity.JournalType)
                                .Where(u => u.IsActive &&
                                            u.Reference.ToLower() == entity.Reference.ToLower() &&
                                            u.Id != entity.Id
                                            )
                                .FirstOrDefaultAsync();

                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.GeneralJournal))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.Bill))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptPurchase))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.PayBill))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.Invoice))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemIssueSale))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.CustomerCredit))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ReceivePayment))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptCustomerCredit))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemIssueVendorCredit))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemIssueAdjustment))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemIssueOther))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemIssueTransfer))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptAdjustment))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptOther))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.ItemReceiptTransfer))
                {
                    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                }
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.Deposit))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
                //if (old != null && old.Reference.ToLower() == entity.Reference.ToLower() && old.JournalType.Equals(JournalType.Withdraw))
                //{
                //    throw new UserFriendlyException(L("DuplicateReferenceNo"));
                //}
            }
        }

        private void ValidateJournal(Journal @entity)
        {
            if (entity == null
                /*|| (_journalType != null && entity.JournalType != _journalType.Value)*/)
            {
                throw new UserFriendlyException(L("RecordNotFound"));
            }
        }

    }
}
