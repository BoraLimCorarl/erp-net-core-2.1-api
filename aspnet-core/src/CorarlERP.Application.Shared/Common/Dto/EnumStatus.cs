using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.Common.Dto
{
    public class EnumStatus
    {
        public enum PaidStatuse
        {
            Pending = 1,
            Partial = 2,
            Paid = 3,
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
            ItemIssueKitchenOrder =24
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
            ProductionOrder = 8
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
            ItemIssueProduction = 16
        }
    }
}
