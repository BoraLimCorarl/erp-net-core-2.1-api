using Abp.Application.Services.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Common.Dto;
using CorarlERP.Currencies;
using CorarlERP.Journals;
using CorarlERP.Journals.Dto;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.Journals
{
   public class JournalAppService_Test : AppTestBase
    {
        private readonly IGeneralJournalAppService _JournalAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly ChartOfAccount _chartOfAccount1;
        public readonly Tax _tax;
        public readonly AccountType _accountType;
        public JournalAppService_Test()
        {
            _JournalAppService = Resolve<IGeneralJournalAppService>();
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _tax = CreateTax("Tax Exempt", 0);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _chartOfAccount1 = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
        }

        private CreateJournalInput CreateHelper()
        {
            var JournalExemptInput = new CreateJournalInput()
            {
                CurrencyId = _currency.Id,
                Memo = "Product from UAS",
                Date = DateTime.Now,
                IsActive = true,
               
                ClassId = null,
                Credit = 10,
                Debit = 10,               
                JournalNo = "P001",
                Reference = "From japen",
                Status = TransactionStatus.Publish,
                
                JournalItems = new List<CreateOrUpdateJournalItemInput>()
                {
                       new CreateOrUpdateJournalItemInput() {AccountId = _chartOfAccount.Id,Credit= 5 ,Debit =5 ,Description = "Description"},
                       new CreateOrUpdateJournalItemInput() {AccountId = _chartOfAccount1.Id,Credit= 10 ,Debit =10 ,Description = "Description"}
                }
            };
            return JournalExemptInput;

        }

        [Fact]
        public async Task Test_CreateJournal()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);
            result.ShouldNotBeNull();
            CheckJournal(JournalExemptInput);

        }
        [Fact]
        public async Task Test_DeleteJournal()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);
            result.Id.ShouldNotBeNull();
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.JournalNo);
            });
            await _JournalAppService.Delete(new CarlEntityDto { Id = result.Id.Value });
            UsingDbContext(context =>
            {
                context.Journals.Count().ShouldBe(0);
            });
        }

        [Fact]
        public async Task Test_GetListJournal()
        {            
            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _JournalAppService.GetList(new GetListJournalInput()
            {               
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(10),

               // IsActive = null,
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,


            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(JournalExemptInput.JournalNo);
                v.Memo.ShouldBe(JournalExemptInput.Memo);
                v.Credit.ShouldBe(JournalExemptInput.Credit);
                v.Debit.ShouldBe(JournalExemptInput.Debit);
                v.Date.ShouldBe(JournalExemptInput.Date);
              //  v.IsActive.ShouldBe(JournalExemptInput.IsActive);
               
            }
        }

        [Fact]
        public async Task Test_FindJournal()
        {
            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _JournalAppService.Find(new GetListJournalInput()
            {
                

               // IsActive = null,
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,


            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
                v.JournalNo.ShouldBe(JournalExemptInput.JournalNo);
                v.Memo.ShouldBe(JournalExemptInput.Memo);
                v.Credit.ShouldBe(JournalExemptInput.Credit);
                v.Debit.ShouldBe(JournalExemptInput.Debit);
                v.Date.ShouldBe(JournalExemptInput.Date);
                //v.IsActive.ShouldBe(JournalExemptInput.IsActive);

            }
        }

        [Fact]
        public async Task Test_GetDetailJournal()
        {

            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);

            result.Id.ShouldNotBeNull();
            var output = await _JournalAppService.GetDetail(new EntityDto<Guid>()
            {

                Id = result.Id.Value,

            });
            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.JournalNo.ShouldBe(JournalExemptInput.JournalNo);
            output.Reference.ShouldBe(JournalExemptInput.Reference);
            output.Date.ShouldBe(JournalExemptInput.Date);
            output.Debit.ShouldBe(JournalExemptInput.Debit);
            output.ClassId.ShouldBe(JournalExemptInput.ClassId);
            CheckJournal(JournalExemptInput);
        }

        [Fact]
        public async Task Test_UpdateJournal()
        {
            Guid JournalId = Guid.Empty;
            var JournalExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(JournalExemptInput);
            result.ShouldNotBeNull();
            CheckJournal(JournalExemptInput);

            UsingDbContext(context =>
            {
                var journalItem = context.JournalItems.FirstOrDefault(u => u.AccountId == JournalExemptInput.JournalItems[0].AccountId);
                journalItem.ShouldNotBeNull();
                JournalId = journalItem.Id;
            });
            JournalId.ShouldNotBe(Guid.Empty);

            var UpdateJournalExemptInput = new UpdateJournalInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,              
                Memo = "Product from England",
                Date = DateTime.Now,
                IsActive = true,                
                ClassId = null,
                Credit = 20,
                Debit = 20,               
                JournalNo = "P001",
                Reference = "From japen",
                Status = TransactionStatus.Draft,
                
                JournalItems = new List<CreateOrUpdateJournalItemInput>()
                {
                     new CreateOrUpdateJournalItemInput() {AccountId = _chartOfAccount.Id,Credit= 10 ,Debit =10 ,Description = "Description"},
                     new CreateOrUpdateJournalItemInput() {AccountId = _chartOfAccount.Id,Credit= 100 ,Debit =100 ,Description = "Description2"}
                }

            };
            var updatedResult = await _JournalAppService.Update(UpdateJournalExemptInput);
            updatedResult.ShouldNotBeNull();

            CheckJournal(UpdateJournalExemptInput, result.Id.Value);


        }

        private void CheckJournal(CreateJournalInput JournalExemptInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == JournalExemptInput.JournalNo);
                JournalEntity.ShouldNotBeNull();
                if (Id != null) JournalEntity.Id.ShouldBe(Id.Value);              
                JournalEntity.Credit.ShouldBe(JournalExemptInput.Credit);
                JournalEntity.Debit.ShouldBe(JournalExemptInput.Debit);
                JournalEntity.CurrencyId.ShouldBe(JournalExemptInput.CurrencyId);
                JournalEntity.Date.ShouldBe(JournalExemptInput.Date);
                JournalEntity.Memo.ShouldBe(JournalExemptInput.Memo);
                JournalEntity.IsActive.ShouldBe(true);

                var journalItems = context.JournalItems.Where(u => u.JournalId == JournalEntity.Id).OrderBy(u => u.Description).ToList();
                var InputPurchaseOrderItems = JournalExemptInput.JournalItems.OrderBy(u => u.Description).ToList();
                journalItems.ShouldNotBeNull();
                journalItems.Count().ShouldBe(InputPurchaseOrderItems.Count());
                for (var i = 0; i < journalItems.Count; i++)
                {
                    var JournalItem = journalItems[i];
                    var inJournalItem = InputPurchaseOrderItems[i];               
                    ShouldBeTestExtensions.ShouldBe(JournalItem.AccountId, inJournalItem.AccountId);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Credit, inJournalItem.Credit);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Debit, inJournalItem.Debit);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Description, inJournalItem.Description);                    
                }
            });
        }
            
        [Fact]
        public async Task Test_UpdateStatus()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckJournal(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.JournalItems.FirstOrDefault(u => u.AccountId == ItemReceiptExemptInput.JournalItems[0].AccountId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _JournalAppService.UpdateStatusToPublish(UpdateStatusInput);

        }

        [Fact]
        public async Task Test_UpdateStatusToDraft()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckJournal(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.JournalItems.FirstOrDefault(u => u.AccountId == ItemReceiptExemptInput.JournalItems[0].AccountId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _JournalAppService.UpdateStatusToDraft(UpdateStatusInput);

        }


        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            Guid ItemReceiptId = Guid.Empty;
            var ItemReceiptExemptInput = CreateHelper();
            var result = await _JournalAppService.Create(ItemReceiptExemptInput);
            result.ShouldNotBeNull();
            CheckJournal(ItemReceiptExemptInput);

            UsingDbContext(context =>
            {

                var ItemReceipt = context.JournalItems.FirstOrDefault(u => u.AccountId == ItemReceiptExemptInput.JournalItems[0].AccountId);
                ItemReceipt.ShouldNotBeNull();
                ItemReceiptId = result.Id.Value;
            });
            ItemReceiptId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = ItemReceiptId,
            };
            var updatedResult = await _JournalAppService.UpdateStatusToVoid(UpdateStatusInput);

        }
    }
}
