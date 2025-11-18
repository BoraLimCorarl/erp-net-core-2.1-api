using Abp.Application.Services.Dto;
using CorarlERP.Addresses;
using CorarlERP.Bills;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Journals.Dto;
using CorarlERP.Locations;
using CorarlERP.PayBills;
using CorarlERP.PayBills.Dto;
using CorarlERP.Taxes;
using CorarlERP.Vendors;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;
using static CorarlERP.Journals.Journal;

namespace CorarlERP.Tests.PayBills
{

    public class PayBillAppService_Test : AppTestBase
    {
        public readonly Class _class;
        private readonly IBillAppService _billAppService;
        private readonly IPayBillAppService _payBillAppService;
        private readonly Currency _currency;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly ChartOfAccount _chartOfAccount1;
        public readonly Tax _tax;
        private readonly Item _item;
        public readonly Location _location;
        public readonly AccountType _accountType;
        public readonly Bills.Bill _bill;
        public readonly BillItem _billItem;
        private readonly ItemType _itemType;
        public readonly Vendor _vendor;

        public PayBillAppService_Test() {
            _billAppService = Resolve<IBillAppService>();
            _payBillAppService = Resolve<IPayBillAppService>();
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _class = CreateClass(null, null, "Default", false, null);
            _accountType = CreateAccountType("Bank", "", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                       displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _tax = CreateTax("Tax Exempt", 0);
            _chartOfAccount = CreateChartOfAccount("111524", "Cash", "cash description", _accountType.Id, null, _tax.Id);
            _chartOfAccount1 = CreateChartOfAccount("155222", "Pretty Cash", "Pretty cash description", _accountType.Id, null, _tax.Id);
            _vendor = CreateVendor("Nika Phone Shop", "001", "nika@gmail.com", "070 955 102", true, "www.nika.com", "", new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), _chartOfAccount.Id, vendorType: null);

            _item = CreateItem(ItemName: "Iphone X", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: null,
                   purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", 
                   barcode:"", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);

            _bill = CreateBillCreate(null, null, Bills.Bill.ReceiveFromStatus.None, DateTime.Now, _vendor.Id, _location.Id, true, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), 200,0,200, null,DateTime.Now,false,DateTime.Now);
            _billItem = CreateBillItem(null, null, _bill.Id, _tax.Id, _item.Id, "Item Description", 2, 100, 0, 200);
        }


        public CreatePayBillInput CreateHelper() {

            //var bill = CreateBillCreate(null, null, Bills.Bill.ReceiveFromStatus.None, DateTime.Now, _vendor.Id, _location.Id, true, new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), new CAddress("Khan Chamkarmourn", "KH", "12104", "Phnom Penh", "#18E0 Monivong Blvd"), 200, 0, 200, null);
            //var billItem = CreateBillItem(null, null, bill.Id, _tax.Id, _item.Id, "Item Description", 2, 100, 0, 200);

            //UsingDbContext(context =>
            //{
            //    var b = context.Bills.FirstOrDefault(u => u.Id == bill.Id);
            //    b.ShouldNotBeNull();
            //});

            //var input = new GetListBillForPaybillInput() {
            //    FromDate = DateTime.Now.AddDays(-10),
            //    ToDate = DateTime.Now.AddDays(10),
            //    Filter = null,
            //    Sorting = null,
            //    MaxResultCount = 10,
            //    SkipCount = 0,
            //};

            //var billOutput = await _billAppService.GetListBillForPayBill(input);

            //var payBillList = new List<CorarlERP.PayBills.Dto.PayBillDetail>();

            //foreach (var i in billOutput.Items) {
            //    payBillList.Add(new CorarlERP.PayBills.Dto.PayBillDetail()
            //    {
            //        accountId = _chartOfAccount.Id,
            //        billId = _bill.Id,
            //        DueDate = _bill.DueDate,
            //        OpenBalance = _bill.OpenBalance,
            //        Payment = 200,
            //        TotalAmount = 200,
            //        vendorId = _bill.VendorId
            //    });
            //}

            var result = new CreatePayBillInput()
            {
                Memo = "Pay bill",
                ClassId = _class.Id,
                CurrencyId = _currency.Id,
                FiFo = false,
                paymentDate = DateTime.Now,
                PaymentNo = "PB-001",
                Reference = "PB-001",
                Status = TransactionStatus.Publish,
                PaymentAccountId = _chartOfAccount.Id,
                TotalOpenBalance = 0,
                TotalPayment = 200,
                TotalPaymentDue = 0,
                PayBillDetail = new List<CorarlERP.PayBills.Dto.PayBillDetail>() {
                    new CorarlERP.PayBills.Dto.PayBillDetail()
                    {
                        AccountId = _chartOfAccount.Id,
                        BillId = _bill.Id,
                        DueDate = _bill.DueDate,
                        OpenBalance = _bill.OpenBalance,
                        Payment = 200,
                        TotalAmount = 200,
                        VendorId = _bill.VendorId
                    }
                }
            };
            return result;

        }

        [Fact]
        public async Task Test_viewBill()
        {

            var PaybillInput = CreateHelper();
            var result = await _payBillAppService.Create(PaybillInput);
            result.Id.ShouldNotBeNull();
            var output = await _payBillAppService.ViewBillHistory(new GetListViewHistoryInput
            {
                Id = _bill.Id,
            });

         
        }

        [Fact]
        public async Task Test_CreatePayBill()
        {
            var paybill = CreateHelper();
            var result = await _payBillAppService.Create(paybill);
            result.ShouldNotBeNull();
            CheckPayBill(paybill, result.Id);
        }


        public void CheckPayBill(CreatePayBillInput input, Guid? Id = null) {
            UsingDbContext(context =>
            {
                var JournalEntity = context.Journals.FirstOrDefault(p => p.JournalNo == input.PaymentNo);
                JournalEntity.ShouldNotBeNull();
                JournalEntity.Credit.ShouldBe(input.TotalPayment);
                JournalEntity.Debit.ShouldBe(0);
                JournalEntity.CurrencyId.ShouldBe(input.CurrencyId);
                JournalEntity.Date.ShouldBe(input.paymentDate);
                JournalEntity.Memo.ShouldBe(input.Memo);

                var journalItems = context.JournalItems.Where(u => u.JournalId == JournalEntity.Id && u.Debit != 0).OrderBy(u => u.AccountId).ToList();
                var InputJournalItems = input.PayBillDetail.OrderBy(u => u.AccountId).ToList();
                journalItems.ShouldNotBeNull();
                journalItems.Count().ShouldBe(InputJournalItems.Count());
                for (var i = 0; i < journalItems.Count; i++)
                {
                    var JournalItem = journalItems[i];
                    var inJournalItem = InputJournalItems[i];
                    ShouldBeTestExtensions.ShouldBe(JournalItem.JournalId, JournalEntity.Id);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.AccountId, inJournalItem.AccountId);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Credit, 0);
                    ShouldBeTestExtensions.ShouldBe(JournalItem.Debit, inJournalItem.TotalAmount);
                };
                var payBill = context.PayBill.FirstOrDefault(p => p.Id == Id);
                var jItemId = context.JournalItems.FirstOrDefault(u => u.Debit == 0);
                // if (Id != null) JournalEntity.Id.ShouldBe(Id.Value);
                payBill.TotalPayment.ShouldBe(input.TotalPayment);

                var payBillItems = context.PayBillDetail.Where(u => u.PayBillId == payBill.Id).ToList();
                payBillItems.ShouldNotBeNull();
                payBillItems.Count().ShouldBe(InputJournalItems.Count());
                
            });
        }

        //[Fact]
        //public async Task Test_GetList()
        //{
        //    var paybill = CreateHelper();
        //    var result = await _payBillAppService.Create(paybill);

        //    result.Id.ShouldNotBeNull();
        //    var output = await _payBillAppService.GetList(new GetListPayBillInput()
        //    {
        //        FromDate = DateTime.Now.AddDays(-10),
        //        ToDate = DateTime.Now.AddDays(10),
        //        Filter = null,
        //        Sorting = null,
        //        MaxResultCount = 10,
        //        SkipCount = 0
        //    });

        //    foreach (var v in output.Items)
        //    {
        //        v.Id.ShouldNotBeNull();
        //        v.Id.ShouldBe(result.Id.Value);
        //        v.JournalNo.ShouldBe(paybill.PaymentNo);
        //        v.PaymentDate.ShouldBe(paybill.paymentDate);
        //        v.Status.ShouldBe(paybill.Status);
        //        v.Memo.ShouldBe(paybill.Memo);
        //        v.TotalPayment.ShouldBe(paybill.TotalPayment);
        //        v.TotalAmount.ShouldBe(paybill.TotalOpenBalance);
        //    }

        //}



        [Fact]
        public async Task Test_UpdateStatusToVoid()
        {
            Guid PayBillId = Guid.Empty;
            var PayBillInput = CreateHelper();
            var result = await _payBillAppService.Create(PayBillInput);
            result.ShouldNotBeNull();
            CheckPayBill(PayBillInput, result.Id);

            UsingDbContext(context =>
            {
                var paybill = context.PayBill.FirstOrDefault(u => u.Id == result.Id.Value);
                paybill.ShouldNotBeNull();
                PayBillId = paybill.Id;
            });
            PayBillId.ShouldNotBe(Guid.Empty);

            var UpdateStatusInput = new UpdateStatus()
            {
                Id = PayBillId,
            };
            var updatedResult = await _payBillAppService.UpdateStatusToVoid(UpdateStatusInput);
            
        }

        [Fact]
        public async Task Test_GetDetail()
        {

            var PaybillInput = CreateHelper();
            var result = await _payBillAppService.Create(PaybillInput);
            result.Id.ShouldNotBeNull();
            var output = await _payBillAppService.GetDetail(new EntityDto<Guid>()
            {
                Id = result.Id.Value,
            });

            output.Id.ShouldNotBeNull();
            output.Id.ShouldBe(result.Id.Value);
            output.PayBillNo.ShouldBe(PaybillInput.PaymentNo);
            output.StatusCode.ShouldBe(PaybillInput.Status);
            output.PaymentDate.ShouldBe(PaybillInput.paymentDate);
            output.Memo.ShouldBe(PaybillInput.Memo);
            output.FiFo.ShouldBe(PaybillInput.FiFo);
            output.Reference.ShouldBe(PaybillInput.Reference);

        }

        [Fact]
        public async Task Test_Update()
        {
            Guid ItemReceiptId = Guid.Empty;
            var paybillInput = CreateHelper();
            var result = await _payBillAppService.Create(paybillInput);
            result.ShouldNotBeNull();
            CheckPayBill(paybillInput, result.Id);

            var UpdateInput = new UpdatePayBillInput()
            {
                Id = result.Id.Value,
                CurrencyId = _currency.Id,
                ClassId = _class.Id,
                FiFo = false,
                Memo = "change memo",
                PayBillDetail = new List<CorarlERP.PayBills.Dto.PayBillDetail>() {
                    new CorarlERP.PayBills.Dto.PayBillDetail(){
                        AccountId = _chartOfAccount1.Id,
                        DueDate = new DateTime().AddDays(2),
                        BillId = _bill.Id,
                        OpenBalance = 10,
                        Payment = 10,
                        TotalAmount = 10,
                        VendorId = _vendor.Id
                    }
                },
                PaymentAccountId = _chartOfAccount1.Id,
                paymentDate = new DateTime().AddDays(1),
                PaymentNo = "PNO-001",
                Reference = "New Reference",
                Status = TransactionStatus.Draft,
                TotalOpenBalance = 100,
                TotalPayment = 100,
                TotalPaymentDue = 0
            };

            var updatedResult = await _payBillAppService.Update(UpdateInput);

            updatedResult.ShouldNotBeNull();

        }

    }


}
