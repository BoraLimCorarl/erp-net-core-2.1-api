using Abp.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CorarlERP.AutoSequences.AutoSequenceManager;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AutoSequences
{
    public interface IAutoSequenceManager : IDomainService
    {
        Task<AutoSequence> GetAsync(Guid id, bool tracking = false);
        Task<IdentityResult> CreateAsync(AutoSequence @entity);
        Task<IdentityResult> RemoveAsync(AutoSequence @entity);
        List<DocumentTypeData> GetDocumentTypes();
        Task<IdentityResult> UpdateAsync(AutoSequence @entity);
        Task<AutoSequence> GetAutoSequenceAsync(DocumentType documentType);
        string GetNewReferenceNumber(string defaultPrefix,
                                            YearFormat yearFormat,
                                            string symbolFormat,
                                            string numberFormat,
                                            string lastAutoSequenceNumber,
                                            DateTime currentDate);
    }
}
