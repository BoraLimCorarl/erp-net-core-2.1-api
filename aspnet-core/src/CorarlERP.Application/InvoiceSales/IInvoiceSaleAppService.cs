using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Bills.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Invoices.Dto;
using CorarlERP.Journals.Dto;
using CorarlERP.SaleOrders.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.InvoiceSales
{
    public interface IInvoiceSaleAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateInvoiceInput input);
        Task<FileDto> CreateAndPrint(CreateInvoiceInput input);

       // Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);

        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);

        Task<InvoiceDetailOutput> GetDetail(EntityDto<Guid> input);

        Task Delete(CarlEntityDto input);

        Task<PagedResultDto<InvoiceHeader>> GetList(GetListInvoiceInput input);

        Task<PagedResultDto<getInvoiceListOutput>> GetListInvoiceForReceivePayment(GetListInvoiceForPaybillInput input);

        Task<PagedResultDto<GetListInvoiceOutput>> Find(GetListInvoiceInput input);

        Task<NullableIdDto<Guid>> Update(UpdateInvoiceInput input);

        Task<PagedResultDto<InvoiceSummaryOutput>> GetInvoices(GetInvoiceListInput input);
        
        Task<InvoiceSummaryOutputForGetInvoiceItem> GetInvoiceItems(SaleOrderGetlistInputForIssue input);

        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
        Task<PagedResultDto<GetListInvoiceOutput>> GetListSaleReturnUpaid(GetListInvoiceInput input);

        Task<FileDto> ExportExcelItemsTamplate();
        Task ImportExcelItems(FileDto input);
        Task MultiDelete(GetListDeleteInput input);
    }
}
