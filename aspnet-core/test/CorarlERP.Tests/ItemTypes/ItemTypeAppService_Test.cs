using CorarlERP.ItemTypes;
using CorarlERP.ItemTypes.Dto;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CorarlERP.Tests.ItemTypes
{
    public class ItemTypeAppService_Test : AppTestBase
    {
        private readonly IItemTypeAppService _ItemTypeAppService;
        private readonly IDefaultValues _defaultValues;
        public ItemTypeAppService_Test()
        {
            _ItemTypeAppService = Resolve<IItemTypeAppService>();
            _defaultValues = Resolve<IDefaultValues>();
            base.LoginAsHostAdmin();
        }

        [Fact]
        public async Task Test_Sync()
        {
            await _ItemTypeAppService.Sync();
            var defaultItemTypes = _defaultValues.ItemTypes.OrderBy(u => u.Name).ToList();

            // Assert
            UsingDbContext(context =>
            {
                var itemTypeEntities = context.ItemTypes.OrderBy(u => u.Name).ToList();
                itemTypeEntities.Count.ShouldBe(defaultItemTypes.Count());

                for (var i = 0; i < itemTypeEntities.Count; i++)
                {
                    itemTypeEntities[i].Name.ShouldBe(defaultItemTypes[i].Name);
                    itemTypeEntities[i].DisplayInventoryAccount.CompareTo(defaultItemTypes[i].DisplayInventoryAccount);
                    itemTypeEntities[i].DisplayPurchase.CompareTo(defaultItemTypes[i].DisplayPurchase);
                    itemTypeEntities[i].DisplayReorderPoint.CompareTo(defaultItemTypes[i].DisplayReorderPoint);
                    itemTypeEntities[i].DisplaySale.CompareTo(defaultItemTypes[i].DisplaySale);
                    itemTypeEntities[i].DisplaySubItem.CompareTo(defaultItemTypes[i].DisplaySubItem);
                }

            });

            var Test = _defaultValues.ItemTypes.OrderBy(u => u.Name).FirstOrDefault(u => u.Name == "Item");
            Test.Name = "Change item";

            await _ItemTypeAppService.Sync();

            UsingDbContext(context =>
            {
                var khrEntity = context.ItemTypes.FirstOrDefault(u => u.Name == Test.Name);
                khrEntity.Name.ShouldBe(Test.Name);
            });
        }
#if GetList
        [Fact]
        public async Task Test_GetItemType()
        {
            await _ItemTypeAppService.Sync();
            var defaultItemTypes = _defaultValues.ItemTypes.OrderBy(u => u.Name).ToList();
            var output = await _ItemTypeAppService.GetList(new GetItemTypeListInput()
            {
                Filter = ""
            });
            UsingDbContext(context =>
            {
                var ItemTypeEntities = context.ItemTypes.OrderBy(u => u.Name).ToList();
                ItemTypeEntities.Count.ShouldBe(defaultItemTypes.Count());

                for (var i = 0; i < ItemTypeEntities.Count; i++)
                {
                    output.Items[i].Name.ShouldBe(defaultItemTypes[i].Name);
                    output.Items[i].DisplayInventoryAccount.CompareTo(defaultItemTypes[i].DisplayInventoryAccount);
                    output.Items[i].DisplayItemCategory.CompareTo(defaultItemTypes[i].DisplayItemCategory);
                    output.Items[i].DisplayPurchase.CompareTo(defaultItemTypes[i].DisplayPurchase);
                    output.Items[i].DisplayReorderPoint.CompareTo(defaultItemTypes[i].DisplayReorderPoint);
                    output.Items[i].DisplaySale.CompareTo(defaultItemTypes[i].DisplaySale);
                    output.Items[i].DisplaySubItem.CompareTo(defaultItemTypes[i].DisplaySubItem);
                    output.Items[i].DisplayUOM.CompareTo(defaultItemTypes[i].DisplayUOM);

                }

            });
        }
 
        
#endif
    }
}
