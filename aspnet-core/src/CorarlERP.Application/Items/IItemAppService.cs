using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Dto;
using CorarlERP.Items.Dto;
using CorarlERP.PropertyFormulas.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CorarlERP.Items
{
    public interface IItemAppService: IApplicationService
    {
        Task<ItemGetListOutput> Create(CreateItemInput input);

        [HttpPost]
        Task<PagedResultDto<ItemGetListOutput>> GetList(GetItemListInput input);
        Task<PagedResultDto<ItemGetListOutput>> Find(GetItemListInputFind input);
        Task<PagedResultDto<ItemGetListOutput>> POSFind(GetItemListInputPOSFind input);
        Task<List<ItemListIdOutput>> CheckItemExist(ItemIdsListInput input);
        Task<ItemDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> UpdateAsync(UpdateItemInput input);
        Task Delete(EntityDto<Guid> input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        //Task<List<ItemGetListBalanceOutput>> GetBalanceQty(GetItemBalanceQtyInput input);
        Task ImportExcel(FileDto input);
        Task<FileDto> ExportExcel(GetItemListInput input);
        Task<FileDto> ExportExcelTemplate(GetItemListInput input);
        Task<FileDto> ExportExcelHasDefaultAccountTemplate(GetItemListInput input);
        Task<FileDto> ExportPDF();
        Task<PagedResultDto<ItemGetListOutput>> SaleReturnFind(GetItemListInputPOSFind input);
        Task<FileDto> ExportExcelHasDefaultAccount(GetItemListInput input);
        Task ImportExcelHasDefaultAccount(FileDto input);
        Task<ValidationCountOutput> CheckMaxItemCount();
        Task<PagedResultDto<ItemGetListOutput>> FindBarcode(GetItemListInputFind input);
        Task<FileDto> PrintBarcode(GetListPrintBarcodeInput input);
        Task<GetCustomItemCodeSetting> CustomItemCode(CustomItemCodeInput input);
        Task<NullableIdDto<Guid>> UpdateAndResetItemCode(UpdateItemInput input);
        
        Task<FileDto> ExportExcelEditTemplate(GetItemListInput input);
        Task ImportExcelEditItem(FileDto input);
        Task<FileDto> ExportExcelEditPropertyTemplate(GetItemListInput input);
        Task ImportExcelEditPropertyItem(FileDto input);
    }
}
