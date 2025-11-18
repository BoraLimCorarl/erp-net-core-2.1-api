using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CorarlERP.Common.Dto;
using CorarlERP.Dto;
using CorarlERP.Journals.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CorarlERP.Journals
{
    public interface IGeneralJournalAppService : IApplicationService
    {
        Task<NullableIdDto<Guid>> Create(CreateJournalInput input);
        Task<PagedResultDto<JournalGetListOutput>> GetList(GetListJournalInput input);
        Task<PagedResultDto<JournalGetListOutput>> Find(GetListJournalInput input);
        Task<JournalDetailOutput> GetDetail(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> Update(UpdateJournalInput input);
        Task Delete(CarlEntityDto input);
        Task Enable(EntityDto<Guid> input);
        Task Disable(EntityDto<Guid> input);
        Task<NullableIdDto<Guid>> UpdateStatusToPublish(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToVoid(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateStatusToDraft(UpdateStatus input);
        Task<NullableIdDto<Guid>> UpdateAccount(UpdateAccount input);
        Task<NullableIdDto<Guid>> UpdateJournalCreationTimeIndex(UpdateJournalCreationTimeIndex input);
        Task UpdateJournalDate(UpdateJournalDateInput input);
        Task ChangeJournalCurrency(ChangeJournalCurrencyInput input);
        Task ChangeJournalLocation(ChangeJournalLocationInput input);
        Task ChangeJournalClass(ChangeJournalClassInput input);
        Task<FileDto> ExportExcelTamplate();
        Task ImportExcel(FileDto input);
    }
}
