using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AutoSequences
{
    [Table("CarlErpAutoSequences")]
    public class AutoSequence : AuditedEntity<Guid>, IMayHaveTenant
    {
        public int? TenantId { get; set; }        
        public DocumentType DocumentType { get; private set; }        
        public string DefaultPrefix { get; private set; }
        public string SymbolFormat { get; private set; }
        public string NumberFormat { get; private set; }
        public bool CustomFormat { get; private set; }

        public bool RequireReference { get; private set; }

        public YearFormat? YearFormat { get; private set; }
        public string LastAutoSequenceNumber { get; private set; }
       
        public static AutoSequence Create(
            int? tenantId,
            long? creatorUserId,
            DocumentType documentType,          
            string defaultPrefix,
            string symbolFormat,
            string numberFormat,
            bool customFormat,           
            YearFormat? yearFormat,
            string lastAutoSequenceNumber
            )
        {
            return new AutoSequence()
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                TenantId = tenantId,
                DocumentType = documentType,
                DefaultPrefix = defaultPrefix,
                SymbolFormat = symbolFormat,
                NumberFormat = numberFormat,
                CustomFormat = customFormat,              
                YearFormat = yearFormat,
                LastAutoSequenceNumber = lastAutoSequenceNumber
                
            };
        }

        public void Update(
           int? tenantId,
           long? creatorUserId,
           DocumentType documentType,          
           string defaultPrefix,
           string symbolFormat,
           string numberFormat,
           bool customFormat,          
           YearFormat yearFormat,
            string lastAutoSequenceNumber)
        {
            CreatorUserId = creatorUserId;
            CreationTime = Clock.Now;
            TenantId = tenantId;           
            DefaultPrefix = defaultPrefix;
            SymbolFormat = symbolFormat;
            NumberFormat = numberFormat;
            CustomFormat = customFormat;          
            YearFormat = yearFormat;
            DocumentType = documentType;
            LastAutoSequenceNumber = lastAutoSequenceNumber;

        }
        public void UpdateRequireReference(bool requireReference)
        {
            RequireReference = requireReference;
        }

        public void UpdateLastAutoSequenceNumber(string lastAutoSequeneNumber)
        {
            LastAutoSequenceNumber = lastAutoSequeneNumber;           
        }
    }
}
