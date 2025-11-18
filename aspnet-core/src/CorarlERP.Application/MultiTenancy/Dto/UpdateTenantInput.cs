using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CorarlERP.AccountCycles;
using CorarlERP.AccountCycles.Dto;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies;
using CorarlERP.Formats;
using CorarlERP.Locations;
using CorarlERP.PropertyValues.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.MultiTenancy.Dto
{
    public class UpdateTenantInput : EntityDto
    {
        //[AutoMap(typeof(Tenant))]
        public string Name { get; set; }
        public string Email { get; set; }
        public string LegalName { get; set; }
        public string BusinessId { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public CAddress CompanyAddress { get; set; }
        public CAddress LegalAddress { get; set; }
        public bool SameAsCompanyAddress { get; set; }
        public bool IsMultiCurrency { get; set; }
        public List<MultiCurrencyOutput> MultiCurrencies { get; set; }
        public long? CurrencyId { get; set; }
        // public Currency Currency { get; set; }

        public long? AccountCycleId { get; set; }
        public AccountCycle AccountCycle { get; set; }

        public int RoundingDigit { get; set; }
        public int RoundingDigitUnitCost { get; set; }
        public long? FormatNumberId { get; set; }
        public Format FormatNumber { get; set; }

        public long? FormatDateId { get; set; }
        public Format FormatDate { get; set; }

        public Guid? TransitAccountId { get; set; }
        public ChartOfAccount TransitAccount { get; set; }

        public Guid? SaleAllowanceAccountId { get; set; }
        public ChartOfAccount SaleAllowanceAccount { get; set; }

        public Guid? BillPaymentAccountId { get; set; }
        public ChartAccountSummaryOutput BillPaymentAccount { get; set; }

        public long? ClassId { get; set; }
        public ClassDetailOutput Class { get; set; }

        public long? TaxId { get; set; }
        public TaxDetailOutput Tax { get; set; }

        public long? LocationId { get; set; }
        public Location Location { get; set; }

        public Guid? ExchangeLossAndGainId { get; set; }

        //Item Reciept info
        public Guid? ItemRecieptCustomerCreditId { get; set; }
        //public ChartOfAccount ItemRecieptCustomerCredit { get; set; }

        public Guid? ItemRecieptTransferId { get; set; }
        // public ChartOfAccount ItemRecieptTransfer { get; set; }

        public Guid? ItemRecieptAdjustmentId { get; set; }
        //public ChartOfAccount ItemRecieptAdjustment { get; set; }

        public Guid? ItemRecieptOtherId { get; set; }
        //public ChartOfAccount ItemRecieptOther { get; set; }

        public Guid? ItemRecieptPhysicalCountId { get; set; }

        //Item Issue info
        public Guid? ItemIssueVendorCreditId { get; set; }
        //public ChartOfAccount ItemIssueVendorCredit { get; set; }

        public Guid? ItemIssueTransferId { get; set; }
        // public ChartOfAccount ItemIssueTransfer { get; set; }

        public Guid? ItemIssueAdjustmentId { get; set; }
        // public ChartOfAccount ItemIssueAdjustment { get; set; }

        public Guid? ItemIssueOtherId { get; set; }
        // public ChartOfAccount ItemIssueOther { get; set; }

        public Guid? ItemIssuePhysicalCountId { get; set; }

        public DateTime CurrentPeriod { get; set; }
        // public DateTime EndDate { get; set; }

        public Guid? BankTransferAccountId { get; set; }
        public Guid? RawProductionAccountId { get; set; }
        public Guid? FinishProductionAccountId { get; set; }
        public Guid? RoundDigitAccountId { get; set; }

        public Guid? CustomerAccountId { get; set; }
        public Guid? VendorAccountId { get; set; }

        public bool IsAutoSequence { get; set; }

        public List<GetLsitAutoSequenceDetailOutput> Items { get; set; }
        //end code

        public long? PropertyId { get; set; }
        public PropertyDetailOutput Property { get; set; }

        public long? POSCurrencyId { get; set; }
        public bool SplitCashCreditPayment { get; set; }
        public bool UseDefaultAccount { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public Guid? COGSAccountId { get; set; }
        public Guid? RevenueAccountId { get; set; }
        public Guid? ExpenseAccountId { get; set; }
        public bool ValidateProductionNetWeight { get; set; }
        public bool AutoItemCode { get; set; }
        public string Prifix { get; set; }
        public string ItemCode { get; set; }
        public bool UseBatchNo { get; set; }
        public bool ProductionSummaryQty { get; set; }
        public bool ProductionSummaryNetWeight { get; set; }
        public bool UseFormula { get; set; }
        public Guid?  LogoId { get; set; }
        public bool UseExchangeRate { get; set; }
    }
    public class UpdateAutoSequenceInput
    {

        public Guid Id { get; set; }
        public string LastAutoSequenceNumber { get; set; }
    }
    public class UpdateAutoSequenceOutput
    {

        public Guid Id { get; set; }
        public string LastAutoSequenceNumber { get; set; }
    }
    public class UpdateTransactionNoInput
    {
        public DocumentType DocumentType { get; set; }
        public string DefaultPrefix { get; set; }
        public YearFormat YearFormat { get; set; }
        public string SymbolFormat { get; set; }


    }

}
