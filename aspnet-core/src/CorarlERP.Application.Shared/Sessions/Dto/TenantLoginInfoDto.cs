using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Timing;
using CorarlERP.AccountCycles.Dto;
using CorarlERP.Addresses.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.Classes.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Formats.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.MultiTenancy;
using CorarlERP.MultiTenancy.Dto;
using CorarlERP.MultiTenancy.Payments;
using CorarlERP.Settings.Dtos;

namespace CorarlERP.Sessions.Dto
{

    public class TenantLoginSummaryInfoDto: EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }

        public Guid? LogoId { get; set; }

        public string LogoFileType { get; set; }
    }

    public class TenantLoginInfoDto : EntityDto
    {
        public bool AutoItemCode { get; set; }
        public string TenancyName { get; set; }

        public string Name { get; set; }

        public Guid? LogoId { get; set; }

        public string LogoFileType { get; set; }

        public Guid? CustomCssId { get; set; }

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        public EditionInfoDto Edition { get; set; }

        public DateTime CreationTime { get; set; }

        public PaymentPeriodType PaymentPeriodType { get; set; }

        public ItemCodeSetting ItemCodeSetting { get;  set; }
        public string SubscriptionDateString { get; set; }

        public string CreationTimeString { get; set; }
        public string PackageCode { get; set; }



        #region Session Currency 
        public bool IsMultiCurrency { get; set; } 
        public long CurrencyId { get; set; }
        public CurrencyDto Currency { get; set; }


        public long? POSCurrencyId { get; set; }
        public CurrencyDto POSCurrency { get; set; }

        public long? FormatNumberId { get; set; }
        public FormatDto FormatNumber { get; set; }

        public long? FormatDateId { get; set; }
        public FormatDto FormatDate { get; set; }

        public Address CompanyAddress { get; set; }

        public Guid? TransitAccountId { get; set; }
        public ChartOfAccountDto TransitAccount { get; set; }

        public Guid? SaleAllowanceAccountId { get; set; }
        public ChartOfAccountDto SaleAllowanceAccount { get; set; }

        public Guid? BillPaymentAccountId { get; set; }
        public ChartOfAccountDto BillPaymentAccount { get; set; }

        public long? ClassId { get; set; }
        public ClassDto Class { get; set; }

        public long? TaxId { get; set; }

        public long? LocationId { get; set; }
        public LocationDto Location { get; set; }


        public long? DefaultUserLocationId { get; set; }
        public LocationDto DefaultUserLocation { get; set; }
        public List<LocationDto> UserLocations { get; set; }

        public Guid? ItemRecieptCustomerCreditId { get; set; }
        public ChartOfAccountDto ItemRecieptCustomerCredit { get; set; }

        public Guid? ItemRecieptTransferId { get; set; }
        public ChartOfAccountDto ItemRecieptTransfer { get; set; }

        public Guid? ItemRecieptAdjustmentId { get; set; }
        public ChartOfAccountDto ItemRecieptAdjustment { get; set; }

        public Guid? ItemRecieptOtherId { get; set; }
        public ChartOfAccountDto ItemRecieptOther { get; set; }

        //Item Issue info
        public Guid? ItemIssueVendorCreditId { get; set; }
        public ChartOfAccountDto ItemIssueVendorCredit { get; set; }

        public Guid? ItemIssueTransferId { get; set; }
        public ChartOfAccountDto ItemIssueTransfer { get; set; }

        public Guid? ItemIssueAdjustmentId { get; set; }
        public ChartOfAccountDto ItemIssueAdjustment { get; set; }

        public Guid? ItemIssueOtherId { get; set; }
        public ChartOfAccountDto ItemIssueOther { get; set; }

        public long AccountCycleId { get; set; }
        public AccountCycleDto AccountCycle { get; set; }

        public bool AutoPostBill { get; set; }
        public bool AutoPostVendorCredit { get; set; }
        public bool AutoPostCustomerCredit { get; set; }
        public bool AutoPostInvoice { get; set; }
        public Guid? RawProductionAccountId { get; set; }
        public ChartOfAccountDto RawProductionAccount { get; set; }

        public Guid? FinishProductionAccountId { get; set; }
        public ChartOfAccountDto FinishProductionAccount { get; set; }

        public Guid? VendorAccountId { get; set; }
        public ChartOfAccountDto VendorAccount { get; set; }

        public Guid? CustomerAccountId { get; set; }
        public ChartOfAccountDto CustomerAccount { get; set; }

        public Guid? ExchangeLossAndGainId { get; set; }
        public ChartOfAccountDto ExchangeLossAndGain { get; set; }


        public  List<AutoSequenceDto> AutoSequenceDetail {get;set;}

        public long? PropertyId { get; set; }

        public bool SplitCashCreditPayment { get; set; }
        public bool UseDefaultAccount { get; set; }

        public Guid? InventoryAccountId { get; set; }
        public ChartOfAccountDto InventoryAccount { get; set; }

        public Guid? COGSAccountId { get; set; }
        public ChartOfAccountDto COGSAccount { get; set; }

        public Guid? RevenueAccountId { get; set; }
        public ChartOfAccountDto RevenueAccount { get; set; }

        public Guid? ExpenseAccountId { get; set; }
        public ChartOfAccountDto ExpenseAccount { get; set; }
        public bool UseBatchNo { get; set; }
        public GetSubscriptionInput Subscription { get; set; }

        public bool ProductionSummaryQty { get; set; }
        public bool ProductionSummaryNetWeight { get; set; }
        public bool UseExchangeRate { get; set; }
        #endregion

        public BillInvoiceSettingInputOutput BillSetting { get; set; }
        public BillInvoiceSettingInputOutput InvoiceSetting { get; set; }

        public bool IsInTrial()
        {
            return IsInTrialPeriod;
        }

        public bool SubscriptionIsExpiringSoon(int subscriptionExpireNootifyDayCount)
        {
            if (SubscriptionEndDateUtc.HasValue)
            {
                return Clock.Now.ToUniversalTime().AddDays(subscriptionExpireNootifyDayCount) >= SubscriptionEndDateUtc.Value;
            }

            return false;
        }

        public int GetSubscriptionExpiringDayCount()
        {
            if (!SubscriptionEndDateUtc.HasValue)
            {
                return 0;
            }

            return Convert.ToInt32(SubscriptionEndDateUtc.Value.ToUniversalTime().Subtract(Clock.Now.ToUniversalTime()).TotalDays);
        }
    }
}