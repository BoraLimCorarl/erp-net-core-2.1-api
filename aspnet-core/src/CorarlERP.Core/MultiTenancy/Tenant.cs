using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.MultiTenancy;
using Abp.Timing;
using CorarlERP.AccountCycles;
using CorarlERP.Addresses;
using CorarlERP.Authorization.Users;
using CorarlERP.AutoSequences;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Editions;
using CorarlERP.Formats;
using CorarlERP.Locations;
using CorarlERP.MultiTenancy.Payments;
using CorarlERP.PropertyValues;
using CorarlERP.Taxes;

namespace CorarlERP.MultiTenancy
{
    /// <summary>
    /// Represents a Tenant in the system.
    /// A tenant is a isolated customer for the application
    /// which has it's own users, roles and other application entities.
    /// </summary>
    /// 
    
        
    public class Tenant : AbpTenant<User>
    {
        public const int MaxLogoMimeTypeLength = 64;

        //Can add application specific tenant properties here

        public DateTime? SubscriptionEndDateUtc { get; set; }

        public bool IsInTrialPeriod { get; set; }

        public virtual Guid? CustomCssId { get; set; }

        public virtual Guid? LogoId { get; set; }

        [MaxLength(MaxLogoMimeTypeLength)]
        public virtual string LogoFileType { get; set; }


        //Can add application specific tenant properties here
        #region MY code
        public const int MaxEmailLength = 512;
        public const int MaxPhoneNumberLength = 128;
        public const int MaxWebsiteLength = 512;
        public const int MaxLegalNameLength = 512;
        public const int MaxBusinessIdLength = 256;

        [MaxLength(MaxLegalNameLength)]
        public string LegalName { get; private set; }

        [MaxLength(MaxBusinessIdLength)]
        public string BusinessId { get; private set; }

        [MaxLength(MaxPhoneNumberLength)]
        public string PhoneNumber { get; private set; }

        [MaxLength(MaxWebsiteLength)]
        public string Website { get; private set; }

        [MaxLength(MaxEmailLength)]
        public string Email { get; private set; }

        public CAddress CompanyAddress { get; private set; }
        public CAddress LegalAddress { get; private set; }
        public bool SameAsCompanyAddress { get; private set; }

        public long? CurrencyId { get; private set; }
        public Currency Currency { get; set; }
        //add new 
        public Guid? TransitAccountId { get; private set; }
        public ChartOfAccount TransitAccount { get; private set; }

        public Guid? SaleAllowanceAccountId { get; private set; }
        public ChartOfAccount SaleAllowanceAccount { get; private set; }

        public Guid? BillPaymentAccountId { get; private set; }
        public ChartOfAccount BillPaymentAccount { get; private set; }

        public long? ClassId { get; private set; }
        public Class Class { get; private set; }

        public long? LocationId { get; private set; }
        public Location Location { get; private set; }

        public long? AccountCycleId { get; private set; }
        public AccountCycle AccountCycle { get; set; }

        public long? FormatNumberId { get; private set; }
        public Format FormatNumber { get; private set; }

        public long? FormatDateId { get; private set; }
        public Format FormatDate { get; private set; }

        //Item Reciept info
        public Guid? ItemRecieptCustomerCreditId { get; private set; }
        public ChartOfAccount ItemRecieptCustomerCredit { get; private set; }

        public Guid? ItemRecieptTransferId { get; private set; }
        public ChartOfAccount ItemRecieptTransfer { get; private set; }

        public Guid? ItemRecieptAdjustmentId { get; private set; }
        public ChartOfAccount ItemRecieptAdjustment { get; private set; }

        public Guid? ItemRecieptOtherId { get; private set; }
        public ChartOfAccount ItemRecieptOther { get; private set; }

        public Guid? ItemRecieptPhysicalCountId { get; private set; }
        public ChartOfAccount ItemRecieptPhysicalCount { get; private set; }

        //Item Issue info
        public Guid? ItemIssueVendorCreditId { get; private set; }
        public ChartOfAccount ItemIssueVendorCredit { get; private set; }

        public Guid? ItemIssueTransferId { get; private set; }
        public ChartOfAccount ItemIssueTransfer { get; private set; }

        public Guid? ItemIssueAdjustmentId { get; private set; }
        public ChartOfAccount ItemIssueAdjustment { get; private set; }

        public Guid? ItemIssueOtherId { get; private set; }
        public ChartOfAccount ItemIssueOther { get; private set; }

        public Guid? ItemIssuePhysicalCountId { get; private set; }
        public ChartOfAccount ItemIssuePhysicalCount { get; private set; }

        public Guid? BankTransferAccountId { get; private set; }
        public ChartOfAccount BankTransferAccount { get; private set; }

        public Guid? RawProductionAccountId { get; private set; }
        public ChartOfAccount RawProductionAccount { get; private set; }

        public Guid? FinishProductionAccountId { get; private set; }
        public ChartOfAccount FinishProductionAccount { get; private set; }

        public Guid? RoundDigitAccountId { get; private set; }
        public ChartOfAccount RoundDigitAccount { get; private set; }

        public DateTime CurrentPeriod { get; private set; }

        public Guid? VendorAccountId { get; private set; }
        public ChartOfAccount VendorAccount { get; private set; }

        public Guid? CustomerAccountId { get; private set; }
        public ChartOfAccount CustomerAccount { get; private set; }

        public bool IsAutoSequence { get; private set; }

        public long? PropertyId { get; set; }
        public Property Property { get; set; }

        public long? POSCurrencyId { get; set; }
        public Currency POSCurrency { get; set; }

        public Guid? ExchangeLossAndGainId { get; private set; }
        public ChartOfAccount ExchangeLossAndGain { get; private set; }

        public void SetProperty(long? id) { PropertyId = id; }

        public void SetPOSCurrency(long? currencyId)
        {
            POSCurrencyId = currencyId;
        }

        public bool SplitCashCreditPayment { get; private set; }
        public bool UseDefaultAccount { get; private set; }
        public bool SetUseDefaultAccount(bool useDefaultAccount) => UseDefaultAccount = useDefaultAccount;


        public Guid? InventoryAccountId { get; private set; }
        public ChartOfAccount InventoryAccount { get; private set; }

        public Guid? COGSAccountId { get; private set; }
        public ChartOfAccount COGSAccount { get; private set; }

        public Guid? RevenueAccountId { get; private set; }
        public ChartOfAccount RevenueAccount { get; private set; }
        public Guid? ExpenseAccountId { get; private set; }
        public ChartOfAccount ExpenseAccount { get; private set; }

        public bool ValidateProductionNetWeight { get; private set; }

        // public ICollection<AutoSequence> AutoSequences { get; private set; }

     //   public bool AutoItemCode { get; private set; }
        public MultiTenancy.ItemCodeSetting ItemCodeSetting { get; private set; }
        //public string Prifix { get; private set; }
        //public string ItemCode { get; private set; }
        public Guid? SubscriptionId { get; private set; }

        public bool UseBatchNo { get; private set; }

        public Tax Tax { get; private set; }
        public long? TaxId { get; private set; }

        public bool DefaultInventoryReportTemplate { get; private set; }

        public bool ProductionSummaryQty { get; private set; }
        public bool ProductionSummaryNetWeight { get; private set; }

        public bool IsDebug { get; private set; }
        public void SetDebug(bool enable) => IsDebug = enable;

        public void SetEnable() => IsActive = true;
        public void SetDisable() => IsActive = false;

        public bool UseExchangeRate { get; private set; }


        public void UpdateTenant(
            string legalName,
            string businessId,
            string phoneNumber,
            string website,
            CAddress companyAddress,
            CAddress legalAddress,
            bool sameAsCompanyAddress,
            long? currencyId,
            long? formatDateId,
            long? formatNumberId,
            string name,
            string email,
            Guid? transitAccountId,
            Guid? saleAllowanceAccountId,
            Guid? billPaymentAccountId,
            long? locationId,
            long? classid,

            Guid? vendorCreditSaleReturnId,
            Guid? itemIssueTransferId,
            Guid? itemIssueAdjustmentId,
            Guid? itemIssueOtherId,
            Guid? itemIssuePhysicalCountId,

            Guid? customerCreditSaleReturnId,
            Guid? itemReceiptTransferId,
            Guid? itemReceiptAdjustmentId,
            Guid? itemReceiptOtherId,
            Guid? itemReceiptPhysicalCountId,
            Guid? bankTransferAccountId,
            Guid? rawProductionAccountId,
            Guid? finishProductionAccountId,
            Guid? roundDigitAccountId,
            Guid? vendorAcountId,
            Guid? customerAccountId,
            bool isAutoSequence,
            Guid? exchangeLossAndGainId,
            bool splitCashCreditPayment,
            Guid? inventoryAccountId,
            Guid? cogsAccountId,
            Guid? revenueAccountId,
            Guid? expenseAccountId,
            bool validateProductionNetWeight,
            long? taxId,
            bool productionSummaryQty,
            bool productionSummaryNetWeight,
            bool useExchangeRate
            )
        {
            BillPaymentAccountId = billPaymentAccountId;
            ClassId = classid;
            TransitAccountId = transitAccountId;
            SaleAllowanceAccountId = saleAllowanceAccountId;
            LocationId = locationId;
            PhoneNumber = phoneNumber;
            LegalName = legalName;
            CurrencyId = currencyId;
            FormatDateId = formatDateId;
            FormatNumberId = formatNumberId;
            Website = website;
            BusinessId = businessId;
            SameAsCompanyAddress = sameAsCompanyAddress;

            CompanyAddress.Update(companyAddress);
            LegalAddress.Update(sameAsCompanyAddress ? companyAddress : legalAddress);
            Name = name;
            Email = email;

            ItemIssueVendorCreditId = vendorCreditSaleReturnId;
            ItemIssueTransferId = itemIssueTransferId;
            ItemIssueAdjustmentId = itemIssueAdjustmentId;
            ItemIssueOtherId = itemIssueOtherId;
            ItemIssuePhysicalCountId = itemIssuePhysicalCountId;

            ItemRecieptCustomerCreditId = customerCreditSaleReturnId;
            ItemRecieptTransferId = itemReceiptTransferId;
            ItemRecieptAdjustmentId = itemReceiptAdjustmentId;
            ItemRecieptOtherId = itemReceiptOtherId;
            ItemRecieptPhysicalCountId = itemReceiptPhysicalCountId;

            BankTransferAccountId = bankTransferAccountId;

            RawProductionAccountId = rawProductionAccountId;
            FinishProductionAccountId = finishProductionAccountId;
            RoundDigitAccountId = roundDigitAccountId;
            VendorAccountId = vendorAcountId;
            CustomerAccountId = customerAccountId;
            //AutoSequences = new List<AutoSequence>();
            IsAutoSequence = isAutoSequence;
            ExchangeLossAndGainId = exchangeLossAndGainId;
            SplitCashCreditPayment = splitCashCreditPayment;

            InventoryAccountId = inventoryAccountId;
            COGSAccountId = cogsAccountId;
            RevenueAccountId = revenueAccountId;
            ExpenseAccountId = expenseAccountId;
            ValidateProductionNetWeight = validateProductionNetWeight;
            TaxId = taxId;
            ProductionSummaryQty = productionSummaryQty;
            ProductionSummaryNetWeight = productionSummaryNetWeight;
            UseExchangeRate = useExchangeRate;
        }
        #endregion

        public void SetAutoItemCode(ItemCodeSetting itemCodeCustomOrFormula)
        {
            this.ItemCodeSetting = itemCodeCustomOrFormula;
           
        }
        public void SettemCodeSetting(ItemCodeSetting itemCodeCustomOrFormula)
        {
            this.ItemCodeSetting = itemCodeCustomOrFormula;           
        }
        public void SetUserItemCodeFormula(ItemCodeSetting useFormula)
        {
            this.ItemCodeSetting = useFormula;
        }
        public void SetSubScription(Guid? subScriptionId,int? editionId)
        {
            this.SubscriptionId = subScriptionId;
            this.EditionId = editionId;
        }
        public void SetUseBatchNo(bool useBatchNo)
        {
            this.UseBatchNo = useBatchNo;
        }
        protected Tenant()
        {

        }
        public void SetDefaultTemplateReport (bool defaultTemplateReport)
        {
            DefaultInventoryReportTemplate = defaultTemplateReport;
        }

        public void SetDefaultValueTenant(long formatDateId, long formatNumberId, long currencyId, string country)
        {
            IsAutoSequence = true;
            FormatDateId = formatDateId;
            FormatNumberId = formatNumberId;
            CurrencyId = currencyId;
            CompanyAddress = new CAddress("", country, "", "", "");
            LegalAddress = new CAddress("", country, "", "", "");

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
            CompanyAddress = new CAddress("", "", "", "", "");
            LegalAddress = new CAddress("", "", "", "", "");
        }

        public virtual bool HasLogo()
        {
            return LogoId != null && LogoFileType != null;
        }

        public void SetLog(Guid? logoId)
        {
            this.LogoId = logoId;
        }
        public void ClearLogo()
        {
            LogoId = null;
            LogoFileType = null;
        }

        public void UpdateSubscriptionDateForPayment(PaymentPeriodType paymentPeriodType, EditionPaymentType editionPaymentType)
        {
            switch (editionPaymentType)
            {
                case EditionPaymentType.NewRegistration:
                case EditionPaymentType.BuyNow:
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                        break;
                    }
                case EditionPaymentType.Extend:
                    ExtendSubscriptionDate(paymentPeriodType);
                    break;
                case EditionPaymentType.Upgrade:
                    if (HasUnlimitedTimeSubscription())
                    {
                        SubscriptionEndDateUtc = Clock.Now.ToUniversalTime().AddDays((int)paymentPeriodType);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private void ExtendSubscriptionDate(PaymentPeriodType paymentPeriodType)
        {
            if (SubscriptionEndDateUtc == null)
            {
                throw new InvalidOperationException("Can not extend subscription date while it's null!");
            }

            if (IsSubscriptionEnded())
            {
                SubscriptionEndDateUtc = Clock.Now.ToUniversalTime();
            }

            SubscriptionEndDateUtc = SubscriptionEndDateUtc.Value.AddDays((int)paymentPeriodType);
        }

        public void UpdateCurrentPeriod(DateTime date)
        {
            this.CurrentPeriod = date;
        }

        public void UpdateAccountCycleId(long? accountCycleId)
        {
            this.AccountCycleId = accountCycleId;
        }

        private bool IsSubscriptionEnded()
        {
            return SubscriptionEndDateUtc < Clock.Now.ToUniversalTime();
        }

        public int CalculateRemainingDayCount()
        {
            return SubscriptionEndDateUtc != null ? (SubscriptionEndDateUtc.Value - Clock.Now.ToUniversalTime()).Days : 0;
        }

        public bool HasUnlimitedTimeSubscription()
        {
            return SubscriptionEndDateUtc == null;
        }
        public void setDefaultInventoryReport(bool defaultInventoryReportTemplate)
        {
            this.DefaultInventoryReportTemplate = defaultInventoryReportTemplate;
        }


        public void UpdateDetault(
            long locationId,
            long? classId,
            Guid? transitAccountId,
            Guid? saleAllowanceAccountId,
            Guid? billPaymentAccountId,
            Guid? itemIssueVendorCreditId,
            Guid? itemIssueTransferId,
            Guid? itemIssueAdjustmentId,
            Guid? itemIssueOtherId,
            Guid? itemIssuePhysicalCountId,

            Guid? customerCreditSaleReturnId,
            Guid? itemReceiptTransferId,
            Guid? itemReceiptAdjustmentId,
            Guid? itemReceiptOtherId,
            Guid? itemReceiptPhysicalCountId,
            Guid? bankTransferAccountId,
            Guid? rawProductionAccountId,
            Guid? finishProductionAccountId,
            Guid? roundDigitAccountId,
            Guid? vendorAcountId,
            Guid? customerAccountId,
            Guid? exchangeLossAndGainId,
            Guid? inventoryAccountId,
            Guid? cogsAccountId,
            Guid? revenueAccountId,
            Guid? expenseAccountId,
            long? taxId)
        {
            LocationId = locationId;
            ClassId = classId;
            TransitAccountId = transitAccountId;
            SaleAllowanceAccountId = saleAllowanceAccountId;
            BillPaymentAccountId = billPaymentAccountId;
            ItemIssueVendorCreditId = itemIssueVendorCreditId;
            ItemIssueTransferId = itemIssueTransferId;
            ItemIssueAdjustmentId = itemIssueAdjustmentId;
            ItemIssueOtherId = itemIssueOtherId;
            ItemIssuePhysicalCountId = itemIssuePhysicalCountId;

            ItemRecieptCustomerCreditId = customerCreditSaleReturnId;
            ItemRecieptTransferId = itemReceiptTransferId;
            ItemRecieptAdjustmentId = itemReceiptAdjustmentId;
            ItemRecieptOtherId = itemReceiptOtherId;
            ItemRecieptPhysicalCountId = itemReceiptPhysicalCountId;

            BankTransferAccountId = bankTransferAccountId;

            RawProductionAccountId = rawProductionAccountId;
            FinishProductionAccountId = finishProductionAccountId;
            RoundDigitAccountId = roundDigitAccountId;
            VendorAccountId = vendorAcountId;
            CustomerAccountId = customerAccountId;
            ExchangeLossAndGainId = exchangeLossAndGainId;

            InventoryAccountId = inventoryAccountId;
            COGSAccountId = cogsAccountId;
            RevenueAccountId = revenueAccountId;
            ExpenseAccountId = expenseAccountId;
            TaxId = taxId;
        }


        public void ResetDefaultValue()
        {
            LocationId = null;
            ClassId = null;
            TransitAccountId = null;
            SaleAllowanceAccountId = null;
            BillPaymentAccountId = null;
            ItemIssueVendorCreditId = null;
            ItemIssueTransferId = null;
            ItemIssueAdjustmentId = null;
            ItemIssueOtherId = null;
            ItemIssuePhysicalCountId = null;
            ItemRecieptCustomerCreditId = null;
            ItemRecieptTransferId = null;
            ItemRecieptAdjustmentId = null;
            ItemRecieptOtherId = null;
            ItemRecieptPhysicalCountId = null;
            BankTransferAccountId = null;
            RawProductionAccountId = null;
            FinishProductionAccountId = null;
            RoundDigitAccountId = null;
            VendorAccountId = null;
            CustomerAccountId = null;
            ExchangeLossAndGainId = null;
            InventoryAccountId = null;
            COGSAccountId = null;
            RevenueAccountId = null;
            ExpenseAccountId = null;
            TaxId = null;
        }

    }
}