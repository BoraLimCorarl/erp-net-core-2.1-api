using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Items;
using CorarlERP.ItemTypes;
using CorarlERP.Locations;
using CorarlERP.PhysicalCounts;
using CorarlERP.PhysicalCounts.Dto;
using CorarlERP.Taxes;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Tests.PhysicalCounts
{
    public class PhysicalCountAppService_Test : AppTestBase
    {
        private readonly IPhysicalCountAppService _physicalCountAppService;
        private readonly Currency _currency;
        private readonly Location _location;
        private readonly Class _class;
        private readonly Item _item;
        private readonly ItemType _itemType;
        public readonly ChartOfAccount _chartOfAccount;
        public readonly Tax _tax;
        public readonly AccountType _accountType;

        public PhysicalCountAppService_Test()
        {
            _physicalCountAppService = Resolve<IPhysicalCountAppService>();
            _tax = CreateTax("Tax Exempt", 0);
            _accountType = CreateAccountType("AccountType", "AccountDescription", TypeOfAccount.COGS);
            _itemType = CreateItemType(null, "Service", displayInventoryAccount: false, displayPurchase: true, displaySale: true, displayReorderPoint: false, displayTrackSerialNumber: false,
                        displaySubItem: false, displayUOM: false, displayItemCategory: false);
            _chartOfAccount = CreateChartOfAccount("accountCode", "accountName", "accountdescrition", _accountType.Id, null, _tax.Id);
            _item = CreateItem(ItemName: "Computer", ItemCode: "0001", salePrice: 0, purchaseCost: 0, reorderPoint: 0, trackSerial: true, saleCurrenyId: null, purchaseAccountId: _chartOfAccount.Id,
                    purchaseTaxId: null, saleTaxId: null, saleAccountId: null, purchaseCurrencyId: null, inventoryAccountId: _chartOfAccount.Id, itemTypeId: _itemType.Id, description: "", 
                    barcode:"", useBatchNo: false, autoBatchNo: false, batchNoFormulaId: null, trackExpiration: false);
            _location = CreateLocation(null, null, "KPC", false, Member.All, null);
            _currency = CreateCurrency("USA", "$", "USA", "The UAS");
            _class = CreateClass(null, null, "Default", false, null);

        }


        private CreatePhysicalCountInput CreateHelper()
        {
            var physicalInput = new CreatePhysicalCountInput()
            {
                Status = TransactionStatus.Draft,
                Memo = "Product from UAS",
                PhysicalCountDate = DateTime.Now,
                PhysicalCountNo = "T001",
                Reference = "T001",
                LocationId = _location.Id,
                ClassId = _class.Id,
                //PhysicalCountItems = new List<CreateOrUpdatePhysicalItemInput>()
                //{
                //      new CreateOrUpdatePhysicalItemInput() {
                //          ItemId = _item.Id,
                //          QtyChange = 10,
                //          QtyOnHand = 10
                //      },
                //      new CreateOrUpdatePhysicalItemInput() {
                //          ItemId =_item.Id,
                //          QtyChange = 10,
                //          QtyOnHand = 10
                //      }
                //}
            };
            return physicalInput;

        }

        [Fact]
        public async Task Test_Create()
        {
            var physicalInput = CreateHelper();
            var result = await _physicalCountAppService.Create(physicalInput);
            result.ShouldNotBeNull();

            CheckPhysicalCount(physicalInput);

        }

        private void CheckPhysicalCount(CreatePhysicalCountInput physicalInput, Guid? Id = null)
        {
            UsingDbContext(context =>
            {
                var physicalEntity = context.PhysicalCounts.FirstOrDefault(p => p.PhysicalCountNo == physicalInput.PhysicalCountNo);
                physicalEntity.ShouldNotBeNull();
                if (Id != null) physicalEntity.Id.ShouldBe(Id.Value);

                physicalEntity.PhysicalCountDate.ShouldBe(physicalInput.PhysicalCountDate);
                physicalEntity.PhysicalCountNo.ShouldBe(physicalInput.PhysicalCountNo);
                physicalEntity.Memo.ShouldBe(physicalInput.Memo);

                var physicalItems = context.PhysicalCountItems.Where(u => u.PhysicalCountId == physicalEntity.Id).OrderBy(u => u.ItemId).ToList();
                var InputPhysicalItems = physicalInput.PhysicalCountItems.OrderBy(u => u.ItemId).ToList();
                physicalItems.ShouldNotBeNull();
                physicalItems.Count().ShouldBe(InputPhysicalItems.Count());
                for (var i = 0; i < physicalItems.Count; i++)
                {
                    var item = physicalItems[i];
                    var inputItem = InputPhysicalItems[i];
                    ShouldBeTestExtensions.ShouldBe(item.ItemId, inputItem.ItemId);
                    //ShouldBeTestExtensions.ShouldBe(item.QtyChange, inputItem.QtyChange);
                    ShouldBeTestExtensions.ShouldBe(item.QtyOnHand, inputItem.QtyOnHand);
                }
            });

        }


        [Fact]
        public async Task Test_GetList()
        {
            Guid test = Guid.NewGuid();
            var input = CreateHelper();
            var result = await _physicalCountAppService.Create(input);

            result.Id.ShouldNotBeNull();
            var output = await _physicalCountAppService.GetList(new GetPhysicalCountListInput()
            {
                // Items = new List<Guid> {_item.Id },
                FromDate = Convert.ToDateTime("2017-01-25T09:00:14.508+07:00"),
                ToDate = Convert.ToDateTime("2019-12-25T09:00:14.508+07:00"),
                //FromDate = DateTime.Now.AddDays(-10),
                //ToDate = DateTime.Now.AddDays(10),
                Filter = null,
                Sorting = null,
                MaxResultCount = 10,
                SkipCount = 0,


            });

            foreach (var v in output.Items)
            {
                v.Id.ShouldNotBeNull();
                v.Id.ShouldBe(result.Id.Value);
            }
        }
    }
}
