using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using CorarlERP.Bills;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.CustomerCredits;
using CorarlERP.Deposits;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.ItemReceipts;
using CorarlERP.Locations;
using CorarlERP.MultiCurrencies;
using CorarlERP.PayBills;
using CorarlERP.ReceivePayments;
using CorarlERP.Withdraws;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.Journals
{
    [Table("CarlErpJournals")]
    public class Journal : BaseAuditedEntity<Guid>
    {
        public const int MaxJournalNoLength = 256;
        public const int MaxReferenceLength = 256;
        public const int MaxCreationIimeIndex = 19;
        [Required]
        [MaxLength(MaxJournalNoLength)]
        public string JournalNo { get; private set; }

        private DateTime _date;
        public DateTime Date { 
            get { return _date; } 
            private set { _date = value; DateOnly = value.Date; } 
        }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime DateOnly { get; private set; }

        public string Memo { get; private set; }
        public void SetMemo (string memo) { Memo = memo; }
        public decimal Debit { get; private set; }

        public decimal Credit { get; private set; }

        [Required]
        public bool IsActive { get; private set; }

        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }


        public long? MultiCurrencyId { get; private set; }
        public Currency MultiCurrency { get; private set; }

        public long? ClassId { get; private set; }
        public Class Class { get; private set; }

        public Guid? GeneralJournalId { get; private set; }
        public virtual Journal GeneralJournal { get; private set; }

        [MaxLength(MaxReferenceLength)]
        public string Reference { get; private set; }

        public TransactionStatus Status { get; private set; }
        public JournalType JournalType { get; private set; }

        public Guid? VendorCreditId { get; private set; }
        public virtual VendorCredit.VendorCredit VendorCredit { get; private set; }

        public Guid? BillId { get; private set; }
        public virtual Bill Bill { get; private set; }

        public Guid? ItemReceiptId { get; private set; }
        public virtual ItemReceipt ItemReceipt { get; private set; }

        public Guid? ItemReceiptCustomerCreditId { get; private set; }
        public virtual ItemReceiptCustomerCredit ItemReceiptCustomerCredit { get; private set; }

        public Guid? ItemIssueVendorCreditId { get; private set; }
        public virtual ItemIssueVendorCredit ItemIssueVendorCredit { get; private set; }

        //Item Issue
        public Guid? ItemIssueId { get; private set; }
        public virtual ItemIssue ItemIssue { get; private set; }

        public Guid? PayBillId { get; private set; }
        public virtual PayBill PayBill { get; private set; }

        public Guid? InvoiceId { get; private set; }
        public virtual Invoice Invoice { get; private set; }

        public Guid? ReceivePaymentId { get; private set; }
        public virtual ReceivePayment ReceivePayment { get; private set; }

        public Guid? CustomerCreditId { get; private set; }
        public virtual CustomerCredit CustomerCredit { get; private set; }

        public Guid? WithdrawId { get; private set; }
        public Withdraw withdraw { get; private set; }

        public Guid? DepositId { get; private set; }
        public Deposit Deposit { get; private set; }


        //public Guid? RoundedAdjustmentJournalId { get; set; }
        //public Journal RoundedAdjustmentJournal { get; set; }

        public Guid? JournalTransactionTypeId { get; private set; }
        public JournalTransactionType JournalTransactionType { get; private set; }

        public void SetDebitCredit(decimal amount) { Debit = amount; Credit = amount; }
       
        public Location Location { get; set; }
        public long? LocationId { get; set; }

        [MaxLength(MaxCreationIimeIndex)]
        public long? CreationTimeIndex { get; private set; }


        #region Link Item Issue Journal to Generarl Journal for rounding
        //public void UpdateRoundedAdjustmentJournal(Guid roundedAdjustmentJournalId)
        //{
        //    RoundedAdjustmentJournalId = roundedAdjustmentJournalId;
        //}

        //public void UpdateRoundedAdjustmentJournal(Journal roundedAdjustmentJournal)
        //{
        //    RoundedAdjustmentJournal = roundedAdjustmentJournal;
        //}
        #endregion


        //public void UpdateLocation (long? locationId)
        //{
        //    this.LocationId = locationId;
        //}

        public void SetJournalTransactionTypeId(Guid? journalTransactionTypeId)
        {          
            this.JournalTransactionTypeId = journalTransactionTypeId;
        }

        public static Journal Create(int? tenantId, long creatorUserId,
            string journalNo, DateTime date, string memo,
            decimal debit, decimal credit,
            long currencyId, long? classId, string reference,long? locationId
           )
        {
            return new Journal()
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                JournalNo = journalNo,
                Date = date,
                Memo = memo,
                Credit = credit,
                Debit = debit,
                IsActive = true,
                ClassId = classId,
                CurrencyId = currencyId,
                Reference = reference,
                LocationId = locationId,
                CreationTimeIndex= Convert.ToInt64(Clock.Now.ToString("yyMMddHHmmss00"))

            };
        }

        public void UpdateCreationTimeIndex (long creationTimeIndex)
        {
            this.CreationTimeIndex = creationTimeIndex;
        }

        public void ChangeDate(DateTime date)
        {
            this.Date = date;
        }
        public enum InventoryType
        {
            ItemReceiptPurchase = 0,
            ItemIssueSale = 1,
        }
        public void UpdateMultiCurrency(long? multiCurrencyId)
        {
            MultiCurrencyId = multiCurrencyId;
        }

        public void SetCurrency(long? companyCurrencyId, long? multiCurrencyId)
        {

            this.MultiCurrencyId = multiCurrencyId;
            this.CurrencyId = companyCurrencyId.Value;
        }

        public void SetClass (long? classId)
        {
            this.ClassId = classId;
        }
        public void SetLocation (long? locationId)
        {
            this.LocationId = locationId;
        }

        public void UpdateClass (long? classId,long? locationId)

        {
            ClassId = classId;
            LocationId = locationId;
        }
        public void UpdateCreditDebit (decimal debit ,decimal credit)
        {
            Debit = debit;
            Credit = credit;
        }

        public void UpdateStatusToDraft()
        {
            this.Status = TransactionStatus.Draft;
        }
        public void UpdateStatus(TransactionStatus status)
        {
            this.Status = status;
        }
        public void UpdatePublish()
        {
            this.Status = TransactionStatus.Publish;
        }
        public void UpdateVoid()
        {
            this.Status = TransactionStatus.Void;
        }

        public void Publish()
        {
            this.Status = TransactionStatus.Publish;
        }

        public void Daft()
        {
            this.Status = TransactionStatus.Draft;
        }
        public void Void()
        {
            this.Status = TransactionStatus.Void;
        }
        public void Enable(bool isEnable)
        {
            IsActive = isEnable;
        }
        public void Update(long lastModifiedUserId, string journalNo,
            DateTime date, string memo, decimal debit, decimal credit,
            long currencyId, long? classId, string reference, TransactionStatus status,long? locationId)
        {
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            JournalNo = journalNo;
            Date = date;
            Memo = memo;
            Credit = credit;
            Debit = debit;
            ClassId = classId;
            CurrencyId = currencyId;
            Reference = reference;
            Status = status;
            LocationId = locationId;
        }

        public void Update(long lastModifiedUserId, 
           decimal debit, decimal credit, DateTime date,
           long? classId, string reference,  long? locationId)
        {
            Date = date;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;         
            Credit = credit;
            Debit = debit;
            ClassId = classId;
            Reference = reference;
            LocationId = locationId;
        }

        public void Update(long lastModifiedUserId,
           decimal debit, decimal credit, DateTime date,
           long? classId, long? locationId)
        {
            Date = date;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            Credit = credit;
            Debit = debit;
            ClassId = classId;
            LocationId = locationId;
        }

        public void UpdateVendorCredit(Guid vendorCreditId)
        {
            this.VendorCreditId = vendorCreditId;
        }
        public void UpdateKitchen()
        {
            this.JournalType = JournalType.ItemIssueKitchenOrder;
        }
        public void UpdateVendorCredit(VendorCredit.VendorCredit vendorCredit)
        {
            ClearAllType();
            this.VendorCredit = vendorCredit;
            this.JournalType = JournalType.VendorCredit;
        }

        public void UpdateCustomerCredit(Guid customerCreditId)
        {
            this.CustomerCreditId = customerCreditId;
        }
        public void UpdateCustomerCredit(CustomerCredit customerCredit)
        {
            ClearAllType();
            this.CustomerCredit = customerCredit;
            this.JournalType = JournalType.CustomerCredit;
        }


        public void UpdateGeneralJournal(Guid generalJournalId)
        {
            this.GeneralJournalId = generalJournalId;
        }

        public void UpdateGeneralJournal(Journal journal)
        {
            ClearAllType();
            this.GeneralJournal = journal;
            this.JournalType = JournalType.GeneralJournal;
        }

        public void UpdateBill(Guid billId)
        {
            this.BillId = billId;
            this.JournalType = JournalType.Bill;
        }

        public void UpdateBill(Bill bill)
        {
            ClearAllType();
            this.Bill = bill;
            this.JournalType = JournalType.Bill;
        }

        public void UpdateInvoice(Guid invoiceId)
        {
            this.InvoiceId = invoiceId;
            this.JournalType = JournalType.Invoice;
        }
        public void UpdateInvoice(Invoice invoice)
        {
            ClearAllType();
            this.Invoice = invoice;
            this.JournalType = JournalType.Invoice;
        }

        public void UpdatePayBill(PayBill payBill)
        {
            ClearAllType();
            this.PayBill = payBill;
            this.JournalType = JournalType.PayBill;
        }

        public void UpdateReceivePayment(ReceivePayment receivePayment)
        {
            ClearAllType();
            this.ReceivePayment = receivePayment;
            this.JournalType = JournalType.ReceivePayment;
        }

        public void UpdateItemReceipt(Guid itemReceiptId)
        {
            this.ItemReceiptId = itemReceiptId;
            this.JournalType = JournalType.ItemReceiptPurchase;
        }

        public void UpdateItemReceipt(ItemReceipt itemReceipt)
        {
            ClearAllType();
            this.ItemReceipt = itemReceipt;
            this.JournalType = JournalType.ItemReceiptPurchase;
        }

        public void UpdateItemReceiptCustomerCredit(ItemReceiptCustomerCredit itemReceipt)
        {
            ClearAllType();
            this.ItemReceiptCustomerCredit = itemReceipt;
            this.JournalType = JournalType.ItemReceiptCustomerCredit;
        }
        public void UpdateItemReceiptCustomerCreditId(Guid id)
        {
            this.ItemReceiptCustomerCreditId = id;
        }

        public void UpdateReceiptTransfer(Guid itemReceiptId)
        {
            this.ItemReceiptId = itemReceiptId;
        }

        public void UpdateItemReceiptTransfer(ItemReceipt itemReceipt)
        {
            ClearAllType();
            this.ItemReceipt = itemReceipt;
            this.JournalType = JournalType.ItemReceiptTransfer;
        }

        public void UpdateItemReceiptProduction(ItemReceipt itemReceipt)
        {
            ClearAllType();
            this.ItemReceipt = itemReceipt;
            this.JournalType = JournalType.ItemReceiptProduction;
        }

        public void UpdateIssueTransfer(Guid itemIssueId)
        {
            this.ItemIssueId = itemIssueId;
        }

        public void UpdateIssueTransfer(ItemIssue itemIssue)
        {
            ClearAllType();
            this.ItemIssue = itemIssue;
            this.JournalType = JournalType.ItemIssueTransfer;
        }

        public void UpdateIssueProduction(Guid itemIssueId)
        {
            this.ItemIssueId = itemIssueId;
        }

     
        public void UpdateIssueProduction(ItemIssue itemIssue)
        {
            ClearAllType();
            this.ItemIssue = itemIssue;
            this.JournalType = JournalType.ItemIssueProduction;
        }

        public void UpdateItemIssueVendorCredit(ItemIssueVendorCredit itemIssue)
        {
            ClearAllType();
            this.ItemIssueVendorCredit = itemIssue;
            this.JournalType = JournalType.ItemIssueVendorCredit;
        }
        public void UpdateItemIssueVendorCreditId(Guid id)
        {
            this.ItemIssueVendorCreditId = id;
        }

        public void UpdateItemReceiptOther(Guid itemReceiptId)
        {
             ClearAllType();
            this.ItemReceiptId = itemReceiptId;
            this.JournalType = JournalType.ItemReceiptOther;
        }

        public void UpdateItemReceiptOther(ItemReceipt itemReceipt)
        {
            ClearAllType();
            this.ItemReceipt = itemReceipt;
            this.JournalType = JournalType.ItemReceiptOther;
        }

        public void UpdateItemReceiptAdjustment(ItemReceipt itemReceipt)
        {
            ClearAllType();
            this.ItemReceipt = itemReceipt;
            this.JournalType = JournalType.ItemReceiptAdjustment;
        }
        public void UpdateItemReceiptAdjustment(Guid itemReceiptId)
        {
            ClearAllType();
            this.ItemReceiptId = itemReceiptId;
            this.JournalType = JournalType.ItemReceiptAdjustment;
        }

        public void UpdateItemIssue(Guid itemReceiptId)
        {
            this.ItemIssueId = itemReceiptId;
            JournalType = JournalType.ItemIssueSale;
        }

        public void UpdateItemIssue(ItemIssue itemIssue)
        {
            ClearAllType();
            ItemIssue = itemIssue;
            JournalType = JournalType.ItemIssueSale;
        }

        public void UpdateIssueOther (ItemIssue itemIssue)
        {
            ClearAllType();
            this.ItemIssue = itemIssue;
            this.JournalType = JournalType.ItemIssueOther;
        }

        public void UpdateIssueOther (Guid itemIssueId)
        {
            this.ItemIssueId = itemIssueId;
        }

        public void UpdateReference(string reference)
        {
            this.Reference = reference;
        }

        public void UpdateIssueAdjustment(ItemIssue itemIssue)
        {
            ClearAllType();
            this.ItemIssue = itemIssue;
            this.JournalType = JournalType.ItemIssueAdjustment;
        }

        public void UpdateIssueAdjustmentId(Guid itemIssueId)
        {
            ClearAllType();
            this.ItemIssueId = itemIssueId;
            this.JournalType = JournalType.ItemIssueAdjustment;
        }

        public void UpdateIssueAdjustment(Guid itemIssueId)
        {
            this.ItemIssueId = itemIssueId;
        }

        public void UpdatePhysicalCountItemReceipt(ItemReceipt physicalCount)
        {
            ClearAllType();
            ItemReceipt = physicalCount;
            JournalType = JournalType.ItemReceiptPhysicalCount;
        }

        public void UpdatePhysicalCountItemReceipt(Guid physicalCountId)
        {
            ClearAllType();
            ItemReceiptId = physicalCountId;
            JournalType = JournalType.ItemReceiptPhysicalCount;
        }

        public void UpdatePhysicalCountItemIssue(ItemIssue physicalCount)
        {
            ClearAllType();
            ItemIssue = physicalCount;
            JournalType = JournalType.ItemIssuePhysicalCount;
        }

        public void UpdatePhysicalCountItemIssue(Guid physicalCountId)
        {
            ClearAllType();
            ItemIssueId = physicalCountId;
            JournalType = JournalType.ItemIssuePhysicalCount;
        }

        public void UpdateWithdraw (Withdraw withdraw)
        {
            ClearAllType();
            this.withdraw = withdraw;
            JournalType = JournalType.Withdraw;

        }
        public void UpdateWithdraw(Guid id)
        {
            this.WithdrawId = id;
        }
        public void UpdateJournalDate(DateTime date)
        {
            this.Date = date;
        }

        public void UpdateDeposit(Deposit deposit)
        {
            ClearAllType();
            this.Deposit = deposit;
            JournalType = JournalType.Deposit;
        }
        public void UpdateDepositId(Guid id)
        {
            this.DepositId = id;
        }

        private void ClearAllType()
        {

            this.ItemReceiptId = null;
            this.ReceivePayment = null;
            this.CustomerCredit = null;
            this.CustomerCreditId = null;
            this.Bill = null;
            this.BillId = null;
            this.PayBillId = null;
            this.PayBill = null;
            this.ItemReceipt = null;
            this.ItemReceiptId = null;
            this.GeneralJournal = null;
            this.GeneralJournalId = null;
            this.ItemIssue = null;
            this.ItemIssueId = null;
            this.Invoice = null;
            this.InvoiceId = null;
            this.WithdrawId = null;
            this.DepositId = null;
        }
    }
}
