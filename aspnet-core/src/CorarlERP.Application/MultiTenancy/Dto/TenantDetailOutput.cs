using Abp.AutoMapper;
using CorarlERP.AccountCycles;
using CorarlERP.Addresses;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies;
using CorarlERP.Currencies.Dto;
using CorarlERP.Formats;
using CorarlERP.Locations;
using CorarlERP.Locations.Dto;
using CorarlERP.PropertyValues.Dto;
using CorarlERP.Taxes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorarlERP.MultiTenancy.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class TenantDetailOutput
    {
        // My code
        public string Name { get; set; }
        public string LegalName { get; set; }
        public string BusinessId { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public CAddress CompanyAddress { get; set; }
        public CAddress LegalAddress { get; set; }
        public bool SameAsCompanyAddress { get; set; }
        public bool IsDebug { get; set; }
        public bool CanChangeCurrentPeriod { get; set; }
        public long? CurrencyId { get; set; }
        public Currency Currency { get; set; }
     
        public long? AccountCycleId { get; set; }
        public AccountCycle AccountCycle { get; set; }

        public long? FormatNumberId { get; set; }
        public Format FormatNumber { get; set; }

        public long? FormatDateId { get; set; }
        public Format FormatDate { get; set; }

        public string Email { get; set; }

        public Guid? TransitAccountId { get; set; }
        public ChartAccountSummaryOutput TransitAccount { get; set; }

        public Guid? SaleAllowanceAccountId { get; set; }
        public ChartAccountSummaryOutput SaleAllowanceAccount { get; set; }

        public Guid? BillPaymentAccountId { get; set; }
        public ChartAccountSummaryOutput BillPaymentAccount { get; set; }

        public long? ClassId { get; set; }
        public ClassSummaryOutput Class { get; set; }
        public long? TaxId { get; set; }
        public TaxSummaryOutput Tax { get; set; }
        public long? LocationId { get; set; }
        public LocationDetailOutput Location { get; set; }

        //Item Reciept info
        public Guid? ItemRecieptCustomerCreditId { get; set; }
        public ChartAccountSummaryOutput ItemRecieptCustomerCredit { get; set; }

        public Guid? ItemRecieptTransferId { get; set; }
        public ChartAccountSummaryOutput ItemRecieptTransfer { get; set; }

        public Guid? ItemRecieptAdjustmentId { get; set; }
        public ChartAccountSummaryOutput ItemRecieptAdjustment { get; set; }

        public Guid? ItemRecieptOtherId { get; set; }
        public ChartAccountSummaryOutput ItemRecieptOther { get; set; }

        public Guid? ItemRecieptPhysicalCountId { get; set; }
        public ChartAccountSummaryOutput ItemRecieptPhysicalCount { get; set; }
        //Item Issue info
        public Guid? ItemIssueVendorCreditId { get; set; }
        public ChartAccountSummaryOutput ItemIssueVendorCredit { get; set; }

        public Guid? ItemIssueTransferId { get; set; }
        public ChartAccountSummaryOutput ItemIssueTransfer { get; set; }

        public Guid? ItemIssueAdjustmentId { get; set; }
        public ChartAccountSummaryOutput ItemIssueAdjustment { get; set; }

        public Guid? ItemIssueOtherId { get; set; }
        public ChartAccountSummaryOutput ItemIssueOther { get; set; }

        public Guid? ItemIssuePhysicalCountId { get; set; }
        public ChartAccountSummaryOutput ItemIssuePhysicalCount { get; set; }

        public Guid? BankTransferAccountId { get; set; }
        public ChartAccountSummaryOutput BankTransferAccount { get; set; }

        public DateTime? CurrentPeriod { get; set; }

        public bool IsMultiCurrency { get { return MultiCurreceis != null && MultiCurreceis.Any(); } }
        public List<MultiCurrencyOutput> MultiCurreceis { get; set; }

        public Guid? RawProductionAccountId { get; set; }
        public ChartAccountSummaryOutput RawProductionAccount { get; set; }
        public Guid? FinishProductionAccountId { get; set; }
        public ChartAccountSummaryOutput FinishProductionAccount { get; set; }

        public Guid? RoundDigitAccountId { get; set; }
        public ChartAccountSummaryOutput RoundDigitAccount { get; set; }

        public Guid? VendorAccountId { get; set; }
        public ChartAccountSummaryOutput VendorAccount { get; set; }

        public Guid? CustomerAccountId { get; set; }
        public ChartAccountSummaryOutput CustomerAccount { get; set; }

        public List<AutoSequenceGroupOutput> AutoSequenceItems { get; set; }

        public bool IsAutoSequence { get; set; }
        //end code

        public long? PropertyId { get; set; }
        public PropertyDetailOutput Property { get; set; }

        public long? POSCurrencyId { get; set; }
        public CurrencyDetailOutput POSCurrency { get; set; }

        public Guid? ExchangeLossAndGainId { get; set; }
        public ChartAccountSummaryOutput ExchangeLossAndGain { get; set; }

        public bool SplitCashCreditPayment { get; set; }
        public bool UseDefaultAccount { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartAccountSummaryOutput InventoryAccount { get; set; }

        public Guid? COGSAccountId { get; set; }
        public ChartAccountSummaryOutput COGSAccount { get; set; }

        public Guid? RevenueAccountId { get; set; }
        public ChartAccountSummaryOutput RevenueAccount { get; set; }

        public Guid? ExpenseAccountId { get; set; }
        public ChartAccountSummaryOutput ExpenseAccount { get; set; }
        public bool ValidateProductionNetWeight { get; set; }

        public bool AutoItemCode { get; set; }
        public string Prifix { get; set; }
        public string ItemCode { get; set; }
        public bool UseBatchNo { get; set; }
        public bool ProductionSummaryQty { get; set; }
        public bool ProductionSummaryNetWeight { get; set; }
        public bool UseFormula { get; set; }
        public bool UseExchangeRate { get; set; }
    }
}
