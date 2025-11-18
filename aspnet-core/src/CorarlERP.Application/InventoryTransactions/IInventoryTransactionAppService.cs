using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.InventoryTransactions.Dto;
using System.Threading.Tasks;

namespace CorarlERP.InventoryTransactions
{
    public interface IInventoryTransactionAppService : IApplicationService
    {
        Task<PagedResultDto<GetListInventoryOutPut>> GetListInventoryTransaction(ListInventoryInput input);
      
    }
}
