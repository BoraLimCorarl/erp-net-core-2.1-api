using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CorarlERP.BatchNos
{
    public enum BatchNoFormulaPrePos
    {
        Prefix = 1, 
        Postfix = 2
    }


    [Table("CarlErpBatchNoFormulas")]
    public class BatchNoFormula : AuditedEntity<long>   
    {
        [Required]
        public string Name { get; private set; }

        public BatchNoFormulaPrePos StandardPrePos { get; private set; }
        public string PrePosCode { get; private set; }

        [Required]
        public string DateFormat { get; private set; }
        [Required]
        public string FieldName { get; private set; }
        public bool IsActive { get; private set; }
        public void Enable(long userId, bool enable)
        {
            LastModifierUserId = userId;
            LastModificationTime = DateTime.UtcNow;
            IsActive = enable;
        }

        public static BatchNoFormula Create(long userId, string name, string fieldName, string dateFormat, BatchNoFormulaPrePos standardPrePos, string prePosCode)
        {
            return new BatchNoFormula
            {
                CreatorUserId = userId,
                CreationTime = Clock.Now,
                Name = name,
                StandardPrePos = standardPrePos,
                PrePosCode = prePosCode,
                DateFormat = dateFormat,
                FieldName = fieldName,
                IsActive = true
            };
        }

        public void Update(long userId, string name, string fieldName, string dateFormat, BatchNoFormulaPrePos standardPrePos, string prePosCode)
        {
            LastModifierUserId = userId;
            LastModificationTime= Clock.Now;
            Name = name;
            StandardPrePos = standardPrePos;
            PrePosCode = prePosCode;
            DateFormat = dateFormat;
            FieldName = fieldName;
        }
    }
}
