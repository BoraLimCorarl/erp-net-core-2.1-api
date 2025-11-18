using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CorarlERP.ChartOfAccounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Journals
{
    
    [Table("CarlErpJournalItems")]
    public class JournalItem : BaseAuditedEntity<Guid>
    {
        public Guid JournalId { get; private set;}
        public virtual Journal Journal { get; private set; }
        public void SetJournal(Guid journalId) => JournalId = journalId;

        public Guid AccountId { get; private set; }
        public virtual ChartOfAccount Account { get; private set; }

        public string Description { get; private set; }

        public decimal Debit { get; private set; }

        public decimal Credit { get; private set; }
        
        public PostingKey Key { get; private set; }
        public Guid? Identifier { get; private set; } 
        

        private static JournalItem CreateJournalItem(int? tenantId, long? creatorUserId,
            Guid accountId,string description ,decimal debit,decimal credit, PostingKey key, Guid? identifier)
        {
            return new JournalItem
            {
                Id = Guid.NewGuid(),
                CreatorUserId = creatorUserId,
                TenantId = tenantId,
                AccountId = accountId,
                Description = description,
                Debit = debit,
                Credit = credit,
                Key = key,
                Identifier = identifier
            };
        }

        //use for create General Detail in Update General Journal (when we have generalId)
        public static JournalItem CreateJournalItem(
            int? tenantId, 
            long? creatorUserId, 
            Guid journalId,
            Guid accountId,
            string description,
            decimal debit, 
            decimal credit,
            PostingKey key,
            Guid? identifier)
        {
            var result = CreateJournalItem(tenantId, creatorUserId,accountId,description,debit,credit, key, identifier);
            result.JournalId = journalId;
            return result;
        }

        //user for create general journal detail in Create new general journal (when this generaljournal does not have Id yet)
        public static JournalItem CreateJournalItem(int? tenantId, long? creatorUserId, 
            Journal journal,Guid accountId, string description, decimal debit, decimal credit, PostingKey key, Guid? identifier)
        {
            var result = CreateJournalItem(tenantId, creatorUserId,accountId,description,debit,credit, key, identifier);
            result.Journal = journal;
            return result;
        }


        //General journal cannot just change general journal 
        public void UpdateJournalItem(long lastModifiedUserId,Guid accountId ,string description ,decimal debit,decimal credit)
        {
            LastModifierUserId = lastModifiedUserId;
            AccountId = accountId;
            Description = description;
            Debit = debit;
            Credit = credit;
        }

        public void UpdateJournalItemAccount(Guid accountId)
        {
            AccountId = accountId;
        }

        public void SetDebitCredit (decimal debit ,decimal credit)
        {
            Debit = debit;
            Credit = credit;
        }

    }
}
