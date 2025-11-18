using System.Threading.Tasks;
using CorarlERP.Common;
using CorarlERP.Common.Dto;
using Shouldly;

namespace CorarlERP.Tests.CommonLookup
{
    public class CommonLookupAppService_Tests : AppTestBase
    {
        private readonly ICommonLookupAppService _commonLookupAppService;

        public CommonLookupAppService_Tests()
        {
            LoginAsHostAdmin();
            _commonLookupAppService = Resolve<ICommonLookupAppService>();
        }

        [MultiTenantFact]
        public async Task Should_Get_Editions()
        {
            var paidEditions = await _commonLookupAppService.GetEditionsForCombobox();
            paidEditions.Items.Count.ShouldBe(7);

            var freeEditions = await _commonLookupAppService.GetEditionsForCombobox(true);
            freeEditions.Items.Count.ShouldBe(4);
        }

        [Xunit.Fact]
        public void TestFindUser()
        {
            var findUser = _commonLookupAppService.FindUserForUserGroup(new FindUsersInput()
            {
                Filter = "",
            });

        }
    }
}
