using Abp.AutoMapper;
using CorarlERP.Customers;
using CorarlERP.Journals;
using CorarlERP.Vendors;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AccountTransactions
{
    public class AccountTransactionOutput
    {
        public long? CreationTimeIndex { get; set; }
        public Guid? JournalId { get; set; }
        public string JournalNo { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string JournalMemo { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public string OtherReference
        {
            get
            {
                if (this.OtherReferenceBill != null) return this.OtherReferenceBill;
                if (this.OtherReferenceInvoice != null) return this.OtherReferenceInvoice;
                return "";
            }
        }
        public string OtherReferenceBill { get; set; }
        public string OtherReferenceInvoice { get; set; }
        public string User { get { return this.CreatorUserName; } }

        public JournalType? JournalType { get; set; }
        public TransactionStatus? JournalStatus { get; set; }
        public Guid? Id { get; set; }
        public DateTime JournalDate { get; set; }
        public DateTime DateOnly { get; set; }
        public Guid? TransactionId {
            get {
                if (this.TransactionGeneralJournalId != null) return this.TransactionGeneralJournalId;
                if (this.TransactionBankId != null) return this.TransactionBankId;
                if (this.TransactionCustomerId != null) return this.TransactionCustomerId;
                if (this.TransactionVendorId != null) return this.TransactionVendorId;
                return (Guid?)null;
            }
        }//mean for id of journal id, invoice id, item receipt or issue id ..... that is in journal

        public Guid? TransactionGeneralJournalId { get; set; }
        public Guid? TransactionBankId { get; set; }
        public Guid? TransactionCustomerId { get; set; } //mean for id transaction that is in journal of customer module => item issue, receive payment...
        public Guid? TransactionVendorId { get; set; }//mean for id transaction that is in journal of vendor module => item receipt, paybill .....


        public Guid? PartnerId
        {
            get
            {
                if (this.Vendor != null) return this.Vendor.Id;
                if (this.Customer != null) return this.Customer.Id;

                return (Guid?)null;
            }
        }

        public string PartnerName
        {
            get
            {
                if (this.Vendor != null) return this.Vendor.VendorName;
                if (this.Customer != null) return this.Customer.CustomerName;

                return "";
            }
        }
        public string PartnerCode
        {
            get
            {
                if (this.Vendor != null) return this.Vendor.VendorCode;
                if (this.Customer != null) return this.Customer.CustomerCode;

                return "";
            }
        }

        public DateTime? CreationTime { get; set; }
        public string CreatorUserName { get; set; }
        public long? CreatorUserId { get; set; }

        public Guid AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public long AccountTypeId { get; set; }
        public TypeOfAccount AccountType { get; set; }
        public decimal AccumBalance { get; set; }


        public VendorDataQuery Vendor
        {
            get
            {

                if (BillVendor != null) return BillVendor;
                if (VendorCreditVendor != null) return VendorCreditVendor;
                if (ItemReceiptVendor != null) return ItemReceiptVendor;
                if (ItemIssueVendor != null) return ItemIssueVendor;
                if (WithdrawlVendor != null) return WithdrawlVendor;
                if (DepositVendor != null) return DepositVendor;
                if (PayBillVendor != null) return PayBillVendor;

                return null;
            }
        }
        
        public CustomerDataQuery Customer
        {
            get
            {

                if (InviceCustomer != null) return InviceCustomer;
                if (CustomerCreditCustomer != null) return CustomerCreditCustomer;
                if (ItemIssueCustomer != null) return ItemIssueCustomer;
                if (ItemReceiptCustomer != null) return ItemReceiptCustomer;
                if (ReceivePaymentCustomer != null) return ReceivePaymentCustomer;

                return null;
            }
        }
        public VendorDataQuery PayBillVendor { get; set; }
        public VendorDataQuery BillVendor { get; set; }
        public VendorDataQuery VendorCreditVendor { get; set; }
        public VendorDataQuery ItemReceiptVendor { get; set; }
        public VendorDataQuery ItemIssueVendor { get; set; }
        public VendorDataQuery WithdrawlVendor { get; set; }
        public VendorDataQuery DepositVendor { get; set; }
        
        public CustomerDataQuery InviceCustomer { get; set; }
        public CustomerDataQuery CustomerCreditCustomer { get; set; }
        public CustomerDataQuery ItemIssueCustomer { get; set; }
        public CustomerDataQuery ItemReceiptCustomer { get; set; }
        public CustomerDataQuery ReceivePaymentCustomer { get; set; }
        public decimal Beginning { get; set; }
    }


    public class GetListAccountTransactionGroupByOutput
    {
        public string KeyName { get; set; }
        public List<AccountTransactionOutput> Items { get; set; }
    }
    public class VendorDataQuery
    {
        public string VendorName { get; set;}
        public string VendorCode { get; set; }
        
        public Guid Id { get; set; }
    }
    
    public class CustomerDataQuery
    {
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public Guid Id { get; set; }
    }
}
