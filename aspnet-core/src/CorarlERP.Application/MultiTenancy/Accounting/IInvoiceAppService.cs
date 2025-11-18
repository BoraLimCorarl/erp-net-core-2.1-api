using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using CorarlERP.MultiTenancy.Accounting.Dto;

namespace CorarlERP.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
