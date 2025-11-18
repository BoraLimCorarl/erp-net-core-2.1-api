using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.enumStatus
{
    public class EnumStatus
    {

        public enum PaidStatuse
        {
            Pending = 1,
            Partial = 2,
            Paid = 3,
        }
        public enum InventoryStockBalanceStatus
        {
            Positive = 1,
            Negative = 2,
            Zero = 3,
        }
        public enum OpeningBalanceStatus
        {
            All = 1,
            Opening = 2,
            Zero = 3,
        }

        public enum InventoryMovementStatus
        {
            None = 0,
            Beginning = 1,
            StockIn = 2,
            StockOut = 3,
        }

        public enum PartnerType
        {
            Vendor = 1,
            Customer = 2
        }
        public enum Member
        {
            All = 1,
            UserGroup = 2
        }

        public enum DeliveryStatus
        {
            ReceivePending = 1,
            ReceivePartial = 2,
            ReceiveAll = 3,
            ShipPending = 4,
            ShipPartial = 5,
            ShipAll = 6,
            NoReceive = 7
        }


        public enum TransferStatus
        {
            Pending = 1,
            ShipAll = 2,
            ReceiveAll = 3
        }

        public enum ApprovalStatus
        {
            Recorded = 1,
            Requested = 2,
            Approved = 3
        }


        public enum CurrencyFilter { AccountingCurrency = 1, MultiCurrencies = 2 }

        public enum TypeOfAccount
        {
            CurrentAsset = 1,
            FixedAsset = 2,
            CurrentLiability = 3,
            LongTermLiability = 4,
            Equity = 5,
            Income = 6,
            COGS = 7,
            Expense = 8,

            OtherCurrentAsset = 9
        }

        public enum TransactionStatus
        {
            Draft = 1,
            Publish = 2,
            Void = 3,
            Close = 4
        }

        public enum JournalType
        {
            GeneralJournal = 1,
            Bill = 2,
            ItemReceiptPurchase = 3,
            PayBill = 4,
            VendorCredit = 5,
            Invoice = 6,
            ItemIssueSale = 7,
            CustomerCredit = 8,
            ReceivePayment = 9,
            ItemReceiptTransfer = 10,
            ItemReceiptAdjustment = 11,
            ItemReceiptOther = 12,
            ItemReceiptCustomerCredit = 13,

            ItemIssueTransfer = 14,
            ItemIssueAdjustment = 15,
            ItemIssueOther = 16,
            ItemIssueVendorCredit = 17,
            ItemIssuePhysicalCount = 18,
            ItemReceiptPhysicalCount = 19,

            Withdraw = 20,
            Deposit = 21,
            ItemIssueProduction = 22,
            ItemReceiptProduction = 23,
            ItemIssueKitchenOrder = 24,

        }

        public enum PostingKey
        {
            None = 0,
            Inventory = 1,
            Clearance = 2,
            AP = 3,
            Payment = 4,
            AR = 5,
            COGS = 6,
            Revenue = 7,
            SaleAllowance = 8,
            Bank = 9,
            InventoryAdjustmentInv = 10,
            InventoryAdjustmentAdj = 11,
            PaymentChange = 12
        }

        public enum ReceiveFromPayBill
        {
            Cash = 1,
            VendorCredit = 2
        }

        public enum ReceiveFromRecievePayment
        {
            Cash = 1,
            CustomerCredit = 2
        }


        public enum ReceiveFrom
        {
            None = 1,
            SaleOrder = 2,
            ItemIssue = 3,
            Invoice = 4,
            TransferOrder = 5,
            ItemIssuePhysicalCount = 6,
            ItemReceiptPhysicalCount = 7,
            ProductionOrder = 8,
            ItemIssueVendorCredit = 9,
            ItemReceiptCustomerCredit = 10,
            ItemIssueSale = 11,
            ItemReceiptPurchase = 12,
            VendorCredit = 13,
            CustomerCredit = 14,
            KitchenOrder = 15,
            DeliverySchedule = 16
        }

        public enum InventoryTransactionType
        {
            ItemReceiptPurchase = 1,
            ItemReceiptCustomerCredit = 2,
            ItemReceiptTransfer = 3,
            ItemReceiptAdjustment = 4,
            ItemReceiptOther = 5,
            ItemReceiptVendorCredit = 6,

            ItemIssueSale = 7,
            ItemIssueVendorCredit = 8,
            ItemIssueTransfer = 9,
            ItemIssueAdjustment = 10,
            ItemIssueOther = 11,
            ItemIssueCustomerCredit = 12,
            ItemIssuePhysicalCount = 13,
            ItemReceiptPhysicalCount = 14,
            ItemReceiptProduction = 15,
            ItemIssueProduction = 16,
            ItemIssueKitchenOrder = 17,
        }

        //public enum ReportTemplateType
        //{
        //    Journal = 1,
        //    Inventory = 2,
        //    BalanceSheet = 3,
        //    ProfitAndLoss = 4
        //}


        public enum PermissionReadWrite
        {
            Read = 1,
            ReadWrite = 2,
        }

        public enum TemplateType
        {
            Gloable = 1,
            OnlyMe = 2,
            User = 3,
            Group = 4,
        }

        //public enum VendorType
        //{
        //    Supplier = 1,
        //    Employee = 2

        //}

        public enum DocumentType
        {
            PurchaseOrder = 1,
            ItemReceipt = 2,          
            ItemReceipt_CustomerCredit = 3,           
            Bill = 4,
            VendorCredit = 5,
            PayBill = 6,
            SaleOrder = 7,
            Invoice = 8,
            CustomerCredit = 9,
            ItemIssue = 10,           
            ItemIssue_VendorCredit = 11,
            RecievePayment = 12,
            BankTransferOrder = 13,
            Deposit = 14,
            Withdraw = 15,
            ProductionOrder = 16,
            TransferOrder = 17,
            PhysicalCount = 18,
            POS = 19,
            ProductionPlan = 20,
            KitchenOrder = 21,
            //DeliverySchedule =22
        }
       
        public enum DocumentTypeGroup
        {
            Vendor = 1,
            Customer = 2,
            Bank = 3,
            Production = 4,
            Inventory = 5
        }

        public enum YearFormat
        {
            YYYY = 1,
            YY = 2,
            None = 3
        }

        public enum TransactionLockType
        {
            PurchaseOrder = 1,
            ItemReceipt = 2,
            Bill = 3,
            PayBill = 4,
            SaleOrder = 5,
            ItemIssue = 6,
            Invoice = 7,
            ReceivePayment = 8,
            ProductionOrder = 9,
            InventoryTransaction = 10,
            TransferOrder = 11,
            PhysicalCount = 12,
            BankTransaction = 13,
            BankTransfer = 14,
            GeneralJournal = 15,
            DeliverySchedule =16

        }

        public enum LockAction
        {
            Create = 1,
            Update = 2,
            Delete = 3,
        }

        public class LockTransactionActionOutput
        {
            public int Id { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public enum ErrorState
        {
            All = 1,
            Success= 2,
            HasError= 3,          
        }
       
        public enum FilterType
        {
            Contain = 1,
            StartWith = 2,
            Exact = 3,
        }
    }

}
