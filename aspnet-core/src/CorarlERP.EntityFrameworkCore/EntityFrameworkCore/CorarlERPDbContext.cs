using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;
using CorarlERP.Chat;
using CorarlERP.Editions;
using CorarlERP.Friendships;
using CorarlERP.MultiTenancy;
using CorarlERP.MultiTenancy.Payments;
using CorarlERP.Storage;
using CorarlERP.Invoices;
using CorarlERP.SaleOrders;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.CustomerCredits;
using CorarlERP.PayBills;
using CorarlERP.ItemReceipts;
using CorarlERP.ReceivePayments;
using CorarlERP.Locations;
using CorarlERP.Bills;
using CorarlERP.ItemIssues;
using CorarlERP.Journals;
using CorarlERP.TransferOrders;
using CorarlERP.Taxes;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ItemTypes;
using CorarlERP.Formats;
using CorarlERP.Currencies;
using CorarlERP.AccountCycles;
using CorarlERP.PropertyValues;
using CorarlERP.Items;
using CorarlERP.SubItems;
using CorarlERP.Customers;
using CorarlERP.Vendors;
using CorarlERP.Caches;
using CorarlERP.PurchaseOrders;
using CorarlERP.Classes;
using CorarlERP.InventoryCostCloses;
using CorarlERP.AccountTrasactionCloses;

using CorarlERP.ReportTemplates;
using CorarlERP.Deposits;

using CorarlERP.Withdraws;
using CorarlERP.BankTransfers;
using CorarlERP.Productions;
using CorarlERP.AutoSequences;
using CorarlERP.ProductionProcesses;
using CorarlERP.CustomerTypes;
using CorarlERP.TransactionTypes;
using CorarlERP.Lots;
using CorarlERP.UserGroups;
using CorarlERP.MultiCurrencies;
using CorarlERP.VendorCustomerOpenBalances;
using CorarlERP.VendorTypes;
using CorarlERP.Locks;
using CorarlERP.PaymentMethods;
using CorarlERP.ItemPrices;
using CorarlERP.ExChanges;
using CorarlERP.Settings;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.InvoiceTemplates;
using CorarlERP.Galleries;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.InventoryCalculationItems;
using CorarlERP.InventoryCalculationSchedules;
using CorarlERP.BatchNos;
using CorarlERP.ProductionPlans;
using CorarlERP.Subscriptions;
//using CorarlERP.SubscriptionPayments;
using CorarlERP.CashFlowTemplates;
using CorarlERP.PropertyFormulas;
using CorarlERP.SignUps;
using CorarlERP.Authorization.ApiClients;
using CorarlERP.Promotions;
using CorarlERP.PackageEditions;
using CorarlERP.InventoryTransactionTypes;
using CorarlERP.Referrals;
using CorarlERP.Boms;
using CorarlERP.KitchenOrders;
using CorarlERP.LayaltyAndMemberships;
using CorarlERP.POSSettings;
using CorarlERP.DeliverySchedules;
using CorarlERP.PurchasePrices;
using CorarlERP.QCTests;

namespace CorarlERP.EntityFrameworkCore
{

    public class CorarlERPDbContext : AbpZeroDbContext<Tenant, Role, User, CorarlERPDbContext>, IAbpPersistedGrantDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<MultiTenancy.Accounting.Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        #region Setup
        public virtual DbSet<Tax> Taxes { get; set; }
        public virtual DbSet<AccountType> AccountTypes { get; set; }
        public virtual DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
        //..........Create DbSet of Currency,Property,ItemType...............................
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<MultiCurrency> MultiCurrencies { get; set; }
        public virtual DbSet<Format> Formats { get; set; }
        public virtual DbSet<AccountCycle> AccountCycles { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PropertyValue> PropertyValues { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemsUserGroup> ItemsUserGroup { get; set; }
        public virtual DbSet<ItemProperty> ItemProperties { get; set; }
        public virtual DbSet<SubItem> SubItems { get; set; }

        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<VendorGroup> VendorGroups { get; set; }
        public virtual DbSet<ContactPreson> ContactPersons { get; set; }

        public virtual DbSet<AutoSequence> AutoSequences { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerGroup> CustomerGroups { get; set; }
        public virtual DbSet<CustomerType> CustomerTypes { get; set; }
        public virtual DbSet<VendorType> VendorTypes { get; set; }

        public virtual DbSet<TransactionTypes.TransactionType> TransactionTypes { get; set; }

        public virtual DbSet<CustomerContactPerson> CustomerContactPersons { get; set; }

        public virtual DbSet<Cache> Caches { get; set; }

        public virtual DbSet<TransferOrder> TransferOrders { get; set; }
        public virtual DbSet<TransferOrderItem> TransferOrderItems { get; set; }

        public virtual DbSet<Production> Productions { get; set; }
        public virtual DbSet<FinishItems> FinishItems { get; set; }
        public virtual DbSet<RawMaterialItems> RawMaterialItems { get; set; }

        public virtual DbSet<PhysicalCount.PhysicalCount> PhysicalCounts { get; set; }
        public virtual DbSet<PhysicalCount.PhysicalCountItem> PhysicalCountItems { get; set; }
        public virtual DbSet<PhysicalCount.PhysicalCountItemFilter> PhysicalCountItemFilters { get; set; }
        public virtual DbSet<PhysicalCount.PhysicalCountSetting> PhysicalCountSettings { get; set; }

        public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }

        public virtual DbSet<Lock> Locks { get; set; }
        public virtual DbSet<PermissionLock> PermissionLocks { get; set; }
        public virtual DbSet<TransactionLockSchedule> TransactionLockShedules { get; set; }
        public virtual DbSet<TransactionLockSheduleItem> TransactionLockSheduleItems { get; set; }

        public virtual DbSet<Lot> Lots { get; set; }
        public virtual DbSet<ItemPriceItem> ItemPriceItems { get; set; }
        public virtual DbSet<ItemPrice> ItemPrices { get; set; }

        public virtual DbSet<Exchange> Exchanges { get; set; }
        public virtual DbSet<ExchangeItem> ExchangeItems { get; set; }

        public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<PaymentMethodBase> PaymentMethodBases { get; set; }
        public virtual DbSet<ProductionProcess> ProductionProcess { get; set; }
        public virtual DbSet<Withdraw> Withdraws { get; set; }
        public virtual DbSet<WithdrawItem> WithdrawItems { get; set; }
        public virtual DbSet<BankTransfer> BankTransfers { get; set; }

        //public DbSet<Address> Addresses { get; set; }
        //..................End code.......................................
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<JournalItem> JournalItems { get; set; }
        public virtual DbSet<ItemReceipt> ItemReceipts { get; set; }
        public virtual DbSet<ItemReceiptItem> ItemReceiptItems { get; set; }

        public virtual DbSet<ItemIssue> ItemIssues { get; set; }
        public virtual DbSet<ItemIssueItem> ItemIssueItems { get; set; }

        public virtual DbSet<BillItem> BillItems { get; set; }

        public virtual DbSet<PayBill> PayBill { get; set; }
        public virtual DbSet<PayBillDetail> PayBillDetail { get; set; }
        public virtual DbSet<PayBillExpense> PayBillExpense { get; set; }

        public virtual DbSet<VendorCredit.VendorCredit> VendorCredit { get; set; }
        public virtual DbSet<VendorCredit.VendorCreditDetail> VendorCreditDetail { get; set; }

        public virtual DbSet<ReceivePayment> ReceivePayment { get; set; }
        public virtual DbSet<ReceivePaymentDetail> ReceivePaymentDetail { get; set; }
        public virtual DbSet<ReceivePaymentExpense> ReceivePaymentExpense { get; set; }

        public virtual DbSet<CustomerCredit> CustomerCredit { get; set; }
        public virtual DbSet<CustomerCreditDetail> CustomerCreditDetail { get; set; }

        public virtual DbSet<ItemReceiptCustomerCredit> ItemReceiptCustomerCredit { get; set; }
        public virtual DbSet<ItemReceiptItemCustomerCredit> ItemReceiptCustomerCreditItem { get; set; }

        public virtual DbSet<ItemIssueVendorCredit> ItemIssueVendorCredit { get; set; }
        public virtual DbSet<ItemIssueVendorCreditItem> ItemIssueVendorCreditItem { get; set; }

        public virtual DbSet<SaleOrder> SaleOrder { get; set; }
        public virtual DbSet<SaleOrderItem> SaleOrderItems { get; set; }

        public virtual DbSet<Invoices.Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceItem> InvoiceItems { get; set; }

        public virtual DbSet<InventoryCostClose> InventoryCostCloses { get; set; }

        public virtual DbSet<VendorOpenBalance> VendorOpenBalances { get; set; }
        public virtual DbSet<CustomerOpenBalance> CustomerOpenBalances { get; set; }

        public virtual DbSet<InventoryCostCloseItem> InventoryCostCloseItems { get; set; }

        public virtual DbSet<AccountTransactionClose> AccountTrnasactionCloses { get; set; }


        public virtual DbSet<ReportTemplate> ReportTemplates { get; set; }
        public virtual DbSet<GroupMemberItemTamplate> GroupMemberItemTamplates { get; set; }
        public virtual DbSet<ReportColumnTemplate> ReportColumnTemplates { get; set; }
        public virtual DbSet<ReportFilterTemplate> ReportFilterTemplates { get; set; }

        public virtual DbSet<Deposit> Deposit { get; set; }
        public virtual DbSet<DepositItem> DepositItems { get; set; }

        public virtual DbSet<UserGroup> UserGroups { get; set; }
        public virtual DbSet<UserGroupMember> UserGroupMembers { get; set; }

        public virtual DbSet<VendorTypeMember> VendorTypeMembers { get; set; }
        public virtual DbSet<CustomerTypeMember> CustomerTypeMembers { get; set; }

        #endregion

        public virtual DbSet<BillInvoiceSetting> BillInvoiceSettings { get; set; }
        public virtual DbSet<InvoiceTemplate> InvoiceTemplates { get; set; }
        public virtual DbSet<InvoiceTemplateMap> InvoiceTemplateMaps { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<ItemLot> ItemLots { get; set; }
        public virtual DbSet<InventoryTransactionItem> InventoryTransactionItems { get; set; }
        public virtual DbSet<InventoryCalculationSchedule> InventoryCalculationSchedules { get; set; }
        public virtual DbSet<InventoryCalculationItem> InventoryCalculationItems { get; set; }
        public virtual DbSet<PaymentMethodUserGroup> PaymentMethodUserGroups { get; set; }

        public virtual DbSet<BatchNo> BatchNos { get; set; }
        public virtual DbSet<BatchNoFormula> BatchNoFormulas { get; set; }
        public virtual DbSet<ProductionPlan> ProductionPlans { get; set; }
        public virtual DbSet<InventoryCostCloseItemBatchNo> InventoryCostCloseItemBatchNos { get; set; }
        public virtual DbSet<ProductionStandardCost> ProductionStandardCosts { get; set; }
        public virtual DbSet<ProductionIssueStandardCost> ProductionIssueStandardCosts { get; set; }
        public virtual DbSet<ProductionStandardCostGroup> ProductionStandardCostGroups { get; set; }
        public virtual DbSet<ProductionIssueStandardCostGroup> ProductionIssueStandardCostGroups { get; set; }

        public virtual DbSet<ItemReceiptItemBatchNo> ItemReceiptItemBatchNos { get; set; }
        public virtual DbSet<ItemIssueItemBatchNo> ItemIssueItemBatchNos { get; set; }
        public virtual DbSet<ItemIssueVendorCreditItemBatchNo> ItemIssueVendorCreditItemBatchNos { get; set; }
        public virtual DbSet<ItemReceiptCustomerCreditItemBatchNo> ItemReceiptCustomerCreditItemBatchNos { get; set; }
        public virtual DbSet<VendorCreditItemBatchNo> VendorCreditItemBatchNos { get; set; }
        public virtual DbSet<CustomerCreditItemBatchNo> CustomerCreditItemBatchNos { get; set; }
        public virtual DbSet<ProductionLine> ProductionLines { get; set; }
        public virtual DbSet<Subscription> SubScriptions { get; set; }
        //public virtual DbSet<CorarlSubscriptionPayment> CorarlSubscriptionPayments { get; set; }

        public virtual DbSet<CashFlowAccountGroup> CashFlowAccountGroups { get; set; }
        public virtual DbSet<CashFlowCategory> CashFlowCategories { get; set; }
        public virtual DbSet<CashFlowTemplate> CashFlowTemplates { get; set; }
        public virtual DbSet<CashFlowTemplateCategory> CashFlowTemplateCategories { get; set; }
        public virtual DbSet<CashFlowTemplateAccount> CashFlowTemplateAccounts { get; set; }
        public virtual DbSet<SignUp> SignUps { get; set; }
        public virtual DbSet<ApiClient> ApiClients { get; set; }
        public virtual DbSet<Feature> Features { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<PackageEdition> PackageEditions { get; set; }
        public virtual DbSet<PackageEditionFeature> PackageEditionFeatures { get; set; }
        //public virtual DbSet<PackageEditionPromotion> PackageEditionPromotions { get; set; }

        public virtual DbSet<ItemCodeFormula> ItemCodeFormulas { get; set; }
        public virtual DbSet<ItemCodeFormulaProperty> ItemCodeFormulaProperties { get; set; }
        public virtual DbSet<ItemCodeFormulaCustom> ItemCodeFormulaCustoms { get; set; }
        public virtual DbSet<ItemCodeFormulaItemType> ItemCodeFormulaItemTypes { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<PromotionCampaign> PromotionCampaigns { get; set; }
        public virtual DbSet<PromotionCampaignEdition> PromotionCampaignEditions { get; set; }
        public virtual DbSet<SubscriptionPromotion> SubscriptionPromotions { get; set; }
        public virtual DbSet<SubscriptionCampaignPromotion> SubscriptionCampaignPromotions { get; set; }

        public virtual DbSet<JournalTransactionType> JournalTransactionTypes { get; set; }
        public virtual DbSet<Referral> Referrals { get; set; }
        public virtual DbSet<Bom> Recipes { get; set; }
        public virtual DbSet<BomItem> RecipeItems { get; set; }

        public virtual DbSet<KitchenOrder> KitchenOrders { get;set;}
        public virtual DbSet<KitchenOrderItem> KitchenOrderItems { get; set; }
        public virtual DbSet<KitchenOrderItemAndBOMItem> KitchenOrderItemAndBOMItems { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<POSSetting> POSSettings { get; set; }

        public virtual DbSet<PurchaseOrderExchangeRate> PurchaseOrderExchangeRates { get; set; }
        public virtual DbSet<BillExchangeRate> BillExchangeRates { get; set; }
        public virtual DbSet<VendorCredit.VendorCreditExchangeRate> VendorCreditExchangeRates { get; set; }
        public virtual DbSet<PayBillExchangeRate> PayBillExchangeRates { get; set; }
        public virtual DbSet<PayBillItemExchangeRate> PayBillItemExchangeRates { get; set; }

        public virtual DbSet<SaleOrderExchangeRate> SaleOrderExchangeRates { get; set; }
        public virtual DbSet<InvoiceExchangeRate> InvoiceExchangeRates { get; set; }
        public virtual DbSet<CustomerCreditExchangeRate> CustomerCreditExchangeRates { get; set; }
        public virtual DbSet<ReceivePaymentExchangeRate> ReceivePaymentExchangeRates { get; set; }
        public virtual DbSet<ReceivePaymentItemExchangeRate> ReceivePaymentItemExchangeRates { get; set; }


        public virtual DbSet<DeliveryScheduleItem> DeliverySheduleItems { get; set; }
        public virtual DbSet<DeliverySchedule> DeliveryShedules { get; set; }

        public virtual DbSet<PurchasePrice> PurchasePrices { get; set; }
        public virtual DbSet<PurchasePriceItem> PurchasePriceItems { get; set; }

        public virtual DbSet<TestParameter> TestParameters { get; set; }
        public virtual DbSet<QCTestTemplate> QCTestTemplates { get; set; }
        public virtual DbSet<QCTestTemplateParameter> QCTestTemplateParameters { get; set; }
        public virtual DbSet<QCSample> QCSamples { get; set; }
        public virtual DbSet<LabTestRequest> LabTestRequests { get; set; }
        public virtual DbSet<LabTestResult> LabTestResults { get; set; }
        public virtual DbSet<LabTestResultDetail> LabTestResultDetails { get; set; }

        public CorarlERPDbContext() : base(new DbContextOptions<CorarlERPDbContext>())
        {
        }

        public CorarlERPDbContext(DbContextOptions<CorarlERPDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });

                b.HasOne(u => u.FormatDate).WithMany().HasForeignKey(u => u.FormatDateId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FormatNumber).WithMany().HasForeignKey(u => u.FormatNumberId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.AccountCycle).WithMany().HasForeignKey(u => u.AccountCycleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.HasOne(u => u.BillPaymentAccount).WithMany().HasForeignKey(u => u.BillPaymentAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransitAccount).WithMany().HasForeignKey(u => u.TransitAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleAllowanceAccount).WithMany().HasForeignKey(u => u.SaleAllowanceAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Class).WithMany().HasForeignKey(u => u.ClassId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.OwnsOne(p => p.CompanyAddress);
                b.OwnsOne(p => p.LegalAddress);
                b.HasOne(u => u.ItemIssueAdjustment).WithMany().HasForeignKey(u => u.ItemIssueAdjustmentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueOther).WithMany().HasForeignKey(u => u.ItemIssueOtherId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemRecieptCustomerCredit).WithMany().HasForeignKey(u => u.ItemRecieptCustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueTransfer).WithMany().HasForeignKey(u => u.ItemIssueTransferId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueVendorCredit).WithMany().HasForeignKey(u => u.ItemIssueVendorCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemRecieptAdjustment).WithMany().HasForeignKey(u => u.ItemRecieptAdjustmentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemRecieptTransfer).WithMany().HasForeignKey(u => u.ItemRecieptTransferId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemRecieptOther).WithMany().HasForeignKey(u => u.ItemRecieptOtherId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemRecieptPhysicalCount).WithMany().HasForeignKey(u => u.ItemRecieptPhysicalCountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssuePhysicalCount).WithMany().HasForeignKey(u => u.ItemIssuePhysicalCountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.BankTransferAccount).WithMany().HasForeignKey(u => u.BankTransferAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.HasOne(u => u.RawProductionAccount).WithMany().HasForeignKey(u => u.RawProductionAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FinishProductionAccount).WithMany().HasForeignKey(u => u.FinishProductionAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.RoundDigitAccount).WithMany().HasForeignKey(u => u.RoundDigitAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.HasOne(u => u.VendorAccount).WithMany().HasForeignKey(u => u.VendorAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerAccount).WithMany().HasForeignKey(u => u.CustomerAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Property).WithMany().HasForeignKey(u => u.PropertyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.POSCurrency).WithMany().HasForeignKey(u => u.POSCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ExchangeLossAndGain).WithMany().HasForeignKey(u => u.ExchangeLossAndGainId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.HasOne(u => u.InventoryAccount).WithMany().HasForeignKey(u => u.InventoryAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.COGSAccount).WithMany().HasForeignKey(u => u.COGSAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.RevenueAccount).WithMany().HasForeignKey(u => u.RevenueAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ExpenseAccount).WithMany().HasForeignKey(u => u.ExpenseAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.SubscriptionId });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { e.PaymentId, e.Gateway });
            });


            modelBuilder.Entity<BankTransfer>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.FromLocation).WithMany().HasForeignKey(u => u.FromLocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToLocation).WithMany().HasForeignKey(u => u.ToLocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.BankTransferFromAccount).WithMany().HasForeignKey(u => u.BankTransferFromAccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.BankTransferToAccount).WithMany().HasForeignKey(u => u.BankTransferToAccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferFromClass).WithMany().HasForeignKey(u => u.TransferFromClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferToClass).WithMany().HasForeignKey(u => u.TransferToClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.BankTransferNo }).IsUnique(true);
                b.Property(p => p.Amount).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<Tax>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.Property(u => u.TaxRate).HasColumnType("decimal(6,4)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.TenantId, e.TaxName });
            });

            modelBuilder.Entity<Currency>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.CreatorUserId });

            });
            modelBuilder.Entity<MultiCurrency>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.CreatorUserId });
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<Subscription>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.CreatorUserId });
                b.HasOne(u => u.Tenant).WithMany().HasForeignKey(u => u.TenantId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Edition).WithMany().HasForeignKey(u => u.EditionId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            //modelBuilder.Entity<CorarlSubscriptionPayment>(b =>
            //{
            //    b.HasKey(u => u.Id);
            //    b.HasIndex(i => i.SubscriptionType);
            //    b.HasOne(u => u.Tenant).WithMany().HasForeignKey(u => u.TenantId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            //    b.HasOne(u => u.Edition).WithMany().HasForeignKey(u => u.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            //});

            modelBuilder.Entity<ItemType>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.CreatorUserId });
            });


            modelBuilder.Entity<Format>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.Name, e.Key });
            });

            modelBuilder.Entity<AccountCycle>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.StartDate, e.EndDate });
            });

            modelBuilder.Entity<AutoSequence>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.DocumentType, e.NumberFormat, e.YearFormat, e.DefaultPrefix });
            });

            modelBuilder.Entity<ReportTemplate>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.CreatorUserId, e.TenantId });
            });

            modelBuilder.Entity<GroupMemberItemTamplate>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.ReportTemplate).WithMany().HasForeignKey(u => u.ReportTemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.UserGroup).WithMany().HasForeignKey(u => u.UserGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.MemberUser).WithMany().HasForeignKey(u => u.MemberUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.CreatorUserId, e.TenantId });
            });

            //........................modelbuilder.Entity of PropertyValue...............
            modelBuilder.Entity<PropertyValue>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.Property).WithMany().HasForeignKey(u => u.PropertyId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(e => e.BaseUnit).WithMany().HasForeignKey(e => e.BaseUnitId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<SubItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Cost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Quantity).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ParentSubItem).WithMany().HasForeignKey(u => u.ParentSubItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            //........................End code...........................................
            //................modelbuilder.Entity of Property ...........................
            modelBuilder.Entity<Property>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });


            });
            //........................End code ..........................................
            modelBuilder.Entity<Class>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.ParentClass).WithMany().HasForeignKey(u => u.ParentClassId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.ClassName });
            });
            modelBuilder.Entity<InventoryCostClose>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.AccountCycle).WithMany().HasForeignKey(u => u.AccountCycleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.QtyOnhand).HasColumnType("decimal(19,6)");
                b.Property(p => p.Cost).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalCost).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<VendorOpenBalance>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.AccountCycle).WithMany().HasForeignKey(u => u.AccountCycleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => e.TransactionId);
                b.HasIndex(e => e.Key);
            });

            modelBuilder.Entity<CustomerOpenBalance>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.AccountCycle).WithMany().HasForeignKey(u => u.AccountCycleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => e.TransactionId);
                b.HasIndex(e => e.Key);
            });

            modelBuilder.Entity<InventoryCostCloseItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.InventoryCostClose).WithMany().HasForeignKey(u => u.InventoryCostCloseId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
            });



            modelBuilder.Entity<AccountTransactionClose>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.AccountCycle).WithMany().HasForeignKey(u => u.AccountCycleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.Balance, e.Debit, e.Credit });
                b.Property(p => p.Debit).HasColumnType("decimal(19,6)");
                b.Property(p => p.Credit).HasColumnType("decimal(19,6)");
                b.Property(p => p.Balance).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyDebit).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyCredit).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyBalance).HasColumnType("decimal(19,6)");
                b.HasIndex(p => p.DateOnly);
            });


            modelBuilder.Entity<Location>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.ParentLocation).WithMany().HasForeignKey(u => u.ParentLocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.LocationName });

            });
            modelBuilder.Entity<Lock>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.LockKey });

            });

            modelBuilder.Entity<PermissionLock>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => e.LockTransaction);
                b.HasIndex(e => e.TransactionNo);

                b.HasOne(e => e.Location).WithMany().HasForeignKey(e => e.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<TransactionLockSchedule>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => e.ScheduleType);
                b.HasIndex(e => e.ScheduleDate);

            });

            modelBuilder.Entity<TransactionLockSheduleItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();

                b.HasOne(s => s.TransactionLockShedule).WithMany().HasForeignKey(s => s.TransactionLockSheduleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Lot>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.LotName });

            });


            modelBuilder.Entity<ItemPriceItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.Price).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.ItemPrice).WithMany().HasForeignKey(u => u.ItemPriceId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });

            modelBuilder.Entity<ItemPrice>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransactionTypeSale).WithMany().HasForeignKey(u => u.TransactionTypeSaleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(e => e.CustomerType).WithMany().HasForeignKey(e => e.CustomerTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });


            modelBuilder.Entity<ExchangeItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.Bid).HasColumnType("decimal(19,6)");
                b.Property(p => p.Ask).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.Exchange).WithMany().HasForeignKey(u => u.ExchangeId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });

            modelBuilder.Entity<Exchange>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });


            modelBuilder.Entity<PaymentMethodBase>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.Name });
            });

            modelBuilder.Entity<PaymentMethod>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PaymentMethodBase).WithMany().HasForeignKey(u => u.PaymentMethodId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PaymentMethodUserGroup>(b =>
            {
                b.HasOne(p => p.PaymentMethod).WithMany().HasForeignKey(p => p.PaymentMethodId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.UserGroup).WithMany().HasForeignKey(p => p.UserGroupId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductionProcess>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.ProcessName });
                b.HasIndex(e => e.ProductionProcessType);
            });

            modelBuilder.Entity<Withdraw>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<WithdrawItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(u => u.Withdraw).WithMany().HasForeignKey(u => u.WithdrawId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
            });

            modelBuilder.Entity<Vendor>(b =>
            {
                b.HasKey(u => u.Id);
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.HasOne(u => u.ChartOfAccount).WithMany().HasForeignKey(u => u.AccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorType).WithMany().HasForeignKey(u => u.VendorTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.VendorCode });
                b.HasIndex(e => new { e.TenantId, e.VendorName });
            });
            modelBuilder.Entity<ContactPreson>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VenderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });

            modelBuilder.Entity<VendorGroup>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorId });
            });

            modelBuilder.Entity<BillInvoiceSetting>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.SettingType });
                b.HasIndex(e => e.SettingType);
            });

            modelBuilder.Entity<CustomerType>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerTypeName });
            });

            modelBuilder.Entity<VendorType>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorTypeName });
            });

            modelBuilder.Entity<TransactionTypes.TransactionType>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => (new { e.TenantId, e.TransactionTypeName }));
            });


            modelBuilder.Entity<UserGroup>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.Name });
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserGroupMember>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.UserGroup).WithMany().HasForeignKey(u => u.UserGroupId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(u => u.Member).WithMany().HasForeignKey(u => u.MemberId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
            });

            modelBuilder.Entity<Customer>(b =>
            {
                b.HasKey(u => u.Id);
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerType).WithMany().HasForeignKey(u => u.CustomerTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CustomerCode });
                b.HasIndex(e => new { e.TenantId, e.CustomerName });
            });
            modelBuilder.Entity<CustomerContactPerson>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });

            modelBuilder.Entity<CustomerGroup>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerId, e.UserGroupId });
            });

            modelBuilder.Entity<PurchaseOrder>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.OrderNumber });
                b.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.Reference });
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.MultiCurrency).WithMany().HasForeignKey(u => u.MultiCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.Tax).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTax).HasColumnType("decimal(19,6)");
                b.Property(p => p.ApprovalStatus).HasDefaultValue(ApprovalStatus.Recorded);

            });
            modelBuilder.Entity<PurchaseOrderItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.PurchaseOrder).WithMany().HasForeignKey(u => u.PurchaseOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                //b.HasOne(u => u.PurchaseAccount).WithMany().HasForeignKey(u => u.PurchaseAccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Unit).HasColumnType("decimal(19,6)");
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.DiscountRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.TaxRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<TransferOrder>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.TransferNo }).IsUnique(true);
                b.HasOne(u => u.TransferFromClass).WithMany().HasForeignKey(u => u.TransferFromClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferToClass).WithMany().HasForeignKey(u => u.TransferToClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferFromLocation).WithMany().HasForeignKey(u => u.TransferFromLocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferToLocation).WithMany().HasForeignKey(u => u.TransferToLocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<TransferOrderItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.TransferOrder).WithMany().HasForeignKey(u => u.TransferOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromLot).WithMany().HasForeignKey(u => u.FromLotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToLot).WithMany().HasForeignKey(u => u.ToLotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });

            });


            modelBuilder.Entity<Production>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.SubTotalFinishProduction).HasColumnType("decimal(19,6)");
                b.Property(u => u.SubTotalRawProduction).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.ProductionNo }).IsUnique(true);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromClass).WithMany().HasForeignKey(u => u.FromClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToClass).WithMany().HasForeignKey(u => u.ToClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromLocation).WithMany().HasForeignKey(u => u.FromLocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionAccount).WithMany().HasForeignKey(u => u.ProductionAccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionProcess).WithMany().HasForeignKey(u => u.ProductionProcessId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToLocation).WithMany().HasForeignKey(u => u.ToLocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.ProductionPlan).WithMany().HasForeignKey(e => e.ProductionPlanId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalIssueQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalReceiptQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalIssueNetWeight).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalReceiptNetWeight).HasColumnType("decimal(19,6)");
            });
            modelBuilder.Entity<FinishItems>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.ToLot).WithMany().HasForeignKey(u => u.ToLotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Production).WithMany().HasForeignKey(u => u.ProductionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");

            });
            modelBuilder.Entity<RawMaterialItems>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.FromLot).WithMany().HasForeignKey(u => u.FromLotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Production).WithMany().HasForeignKey(u => u.ProductionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<ProductionStandardCostGroup>(b =>
            {
                b.HasOne(p => p.Production).WithMany().HasForeignKey(p => p.ProductionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.StandardCostGroup).WithMany().HasForeignKey(p => p.StandardCostGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalNetWeight).HasColumnType("decimal(19,6)");
            });
            
            modelBuilder.Entity<ProductionIssueStandardCostGroup>(b =>
            {
                b.HasOne(p => p.Production).WithMany().HasForeignKey(p => p.ProductionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.StandardCostGroup).WithMany().HasForeignKey(p => p.StandardCostGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalNetWeight).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<PhysicalCount.PhysicalCount>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.PhysicalCountNo });
                b.HasOne(u => u.Class).WithMany().HasForeignKey(u => u.ClassId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<PhysicalCount.PhysicalCountItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.PhysicalCount).WithMany().HasForeignKey(u => u.PhysicalCountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
            });
            modelBuilder.Entity<PhysicalCount.PhysicalCountItemFilter>(b =>
            {
                b.HasOne(e => e.Item).WithMany().HasForeignKey(e => e.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.PhysicalCount).WithMany().HasForeignKey(e => e.PhysicalCountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhysicalCount.PhysicalCountSetting>(b =>
            {
               
            });


            modelBuilder.Entity<AccountType>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasIndex(e => new { e.CreatorUserId });
                b.HasIndex(e => e.AccountTypeName);
            });
            #region Item
            modelBuilder.Entity<Item>(b =>
            {
                b.HasKey(u => u.Id);

                b.Property(u => u.PurchaseCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.SalePrice).HasColumnType("decimal(19,6)");
                b.Property(u => u.ReorderPoint).HasColumnType("decimal(19,6)");
                b.Property(u => u.Min).HasColumnType("decimal(19,6)");
                b.Property(u => u.Max).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.SaleTax).WithMany().HasForeignKey(u => u.SaleTaxId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PurchaseTax).WithMany().HasForeignKey(u => u.PurchaseTaxId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleAccount).WithMany().HasForeignKey(u => u.SaleAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.InventoryAccount).WithMany().HasForeignKey(u => u.InventoryAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PurchaseAccount).WithMany().HasForeignKey(u => u.PurchaseAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleCurrency).WithMany().HasForeignKey(u => u.SaleCurrenyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PurchaseCurrency).WithMany().HasForeignKey(u => u.PurchaseCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasMany(u => u.Properties).WithOne(s => s.Item).HasForeignKey(s => s.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
           
                b.HasOne(e => e.Image).WithMany().HasForeignKey(e => e.ImageId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNoFormula).WithMany().HasForeignKey(e => e.BatchNoFormulaId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.ItemCode }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.ItemName });
                b.HasIndex(e => new { e.TenantId, e.Barcode });
            });
            #endregion
            modelBuilder.Entity<ItemsUserGroup>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.UserGroup).WithMany().HasForeignKey(u => u.UserGroupId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(e => new { e.CreatorUserId, e.TenantId, e.UserGroupId, e.ItemId });
            });

            modelBuilder.Entity<ItemProperty>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Property).WithMany().HasForeignKey(u => u.PropertyId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PropertyValue).WithMany().HasForeignKey(u => u.PropertyValueId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.CreatorUserId, e.TenantId });
            });

            modelBuilder.Entity<ChartOfAccount>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ParentAccount).WithMany().HasForeignKey(u => u.ParentAccountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.AccountType).WithMany().HasForeignKey(u => u.AccountTypeId).IsRequired().OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.TenantId, e.AccountCode });
                b.HasIndex(e => new { e.TenantId, e.AccountName });

            });

            modelBuilder.Entity<Journal>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Credit).HasColumnType("decimal(28,6)");
                b.Property(u => u.Debit).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.TenantId, e.Date });
                b.HasIndex(e => new { e.Date });
                b.HasIndex(e => e.DateOnly);
                b.HasIndex(e => new { e.TenantId, e.JournalType, e.JournalNo }).IsUnique(true);
                b.HasIndex(e => new { e.JournalType });
                b.HasIndex(e => e.JournalNo);
                b.HasIndex(e => e.Status);
                b.HasOne(u => u.Class).WithMany().HasForeignKey(u => u.ClassId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.GeneralJournal).WithMany().HasForeignKey(u => u.GeneralJournalId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceipt).WithMany().HasForeignKey(u => u.ItemReceiptId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceiptCustomerCredit).WithMany().HasForeignKey(u => u.ItemReceiptCustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Bill).WithMany().HasForeignKey(u => u.BillId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PayBill).WithMany().HasForeignKey(u => u.PayBillId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssue).WithMany().HasForeignKey(u => u.ItemIssueId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueVendorCredit).WithMany().HasForeignKey(u => u.ItemIssueVendorCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Invoice).WithMany().HasForeignKey(u => u.InvoiceId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorCredit).WithMany().HasForeignKey(u => u.VendorCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerCredit).WithMany().HasForeignKey(u => u.CustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ReceivePayment).WithMany().HasForeignKey(u => u.ReceivePaymentId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.withdraw).WithMany().HasForeignKey(u => u.WithdrawId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Deposit).WithMany().HasForeignKey(u => u.DepositId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.LastModifierUser).WithMany().HasForeignKey(u => u.LastModifierUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.MultiCurrency).WithMany().HasForeignKey(u => u.MultiCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.JournalTransactionType).WithMany().HasForeignKey(u => u.JournalTransactionTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<JournalItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Credit).HasColumnType("decimal(28,6)");
                b.Property(u => u.Debit).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.Identifier, e.Key });

                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Journal).WithMany().HasForeignKey(u => u.JournalId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.LastModifierUser).WithMany().HasForeignKey(u => u.LastModifierUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ItemReceipt>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorId });
                b.OwnsOne(p => p.ShippingAddress);
                b.OwnsOne(p => p.BillingAddress);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferOrder).WithMany().HasForeignKey(u => u.TransferOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PhysicalCount).WithMany().HasForeignKey(u => u.PhysicalCountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionOrder).WithMany().HasForeignKey(u => u.ProductionOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionProcess).WithMany().HasForeignKey(u => u.ProductionProcessId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemReceiptItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(28,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(9,4)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceipt).WithMany().HasForeignKey(u => u.ItemReceiptId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.OrderItem).WithMany(u => u.ItemReceiptItems).HasForeignKey(u => u.OrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FinishItems).WithMany().HasForeignKey(u => u.FinishItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ItemReceiptCustomerCredit>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerId });
                b.OwnsOne(p => p.ShippingAddress);
                b.OwnsOne(p => p.BillingAddress);
                b.HasOne(u => u.CustomerCredit).WithMany().HasForeignKey(u => u.CustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueSale).WithMany().HasForeignKey(u => u.ItemIssueSaleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<ItemReceiptItemCustomerCredit>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(28,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(9,4)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceiptCustomerCredit).WithMany().HasForeignKey(u => u.ItemReceiptCustomerCreditId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerCreditItem).WithMany().HasForeignKey(u => u.CustomerCreditItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueSaleItem).WithMany().HasForeignKey(u => u.ItemIssueSaleItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bill>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.Tax).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTax).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorId });

                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);

                b.HasOne(u => u.ItemReceipt).WithMany().HasForeignKey(u => u.ItemReceiptId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                //  b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<BillItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.TenantId, e.ParentId });
                b.HasOne(u => u.Bill).WithMany().HasForeignKey(u => u.BillId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceiptItem).WithMany().HasForeignKey(u => u.ItemReceiptItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.OrderItem).WithMany(s => s.BillItems).HasForeignKey(u => u.OrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            //Item Issue
            modelBuilder.Entity<ItemIssue>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerId, });
                b.OwnsOne(p => p.ShippingAddress);
                b.OwnsOne(p => p.BillingAddress);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferOrder).WithMany().HasForeignKey(u => u.TransferOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.PhysicalCount).WithMany().HasForeignKey(u => u.PhysicalCountId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionOrder).WithMany().HasForeignKey(u => u.ProductionOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ProductionProcess).WithMany().HasForeignKey(u => u.ProductionProcessId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransactionTypeSale).WithMany().HasForeignKey(u => u.TransactionTypeSaleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.KitchenOrder).WithMany().HasForeignKey(u => u.KitchenOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<ItemIssueItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(28,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(9,4)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssue).WithMany().HasForeignKey(u => u.ItemIssueId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleOrderItem).WithMany(u => u.ItemIssueItems).HasForeignKey(u => u.SaleOrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.TransferOrderItem).WithMany().HasForeignKey(u => u.TransferOrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.RawMaterialItem).WithMany().HasForeignKey(u => u.RawMaterialItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.KitchenOrderItemAndBOMItem).WithMany().HasForeignKey(u => u.KitchenOrderItemAndBOMItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.DeliveryScheduleItem).WithMany(u => u.ItemIssueItems).HasForeignKey(u => u.DeliverySchedulItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<ItemIssueVendorCredit>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorId });
                b.OwnsOne(p => p.ShippingAddress);
                b.OwnsOne(p => p.BillingAddress);
                b.HasOne(u => u.ItemReceiptPurchase).WithMany().HasForeignKey(u => u.ItemReceiptPurchaseId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorCredit).WithMany().HasForeignKey(u => u.VendorCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<ItemIssueVendorCreditItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(28,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(28,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(9,4)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueVendorCredit).WithMany().HasForeignKey(u => u.ItemIssueVendorCreditId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorCreditItem).WithMany().HasForeignKey(u => u.VendorCreditItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceiptPurchaseItem).WithMany().HasForeignKey(u => u.ItemReceiptPurchaseItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });


            // Paybill 
            modelBuilder.Entity<PayBill>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Change).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyChange).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaymentVendorCredit).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaymentBill).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalOpenBalanceVendorCredit).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentBill).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentVendorCredit).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentDue).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentDueVendorCredit).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.PaymentMethod).WithMany().HasForeignKey(u => u.PaymentMethodId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<PayBillDetail>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.TotalAmount).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.Payment).HasColumnType("decimal(19,6)");

                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalAmount).HasColumnType("decimal(19,6)");


                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.PayBill).WithMany().HasForeignKey(u => u.PayBillId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Bill).WithMany().HasForeignKey(u => u.BillId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorCredit).WithMany().HasForeignKey(u => u.VendorCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<PayBillExpense>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Amount).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.PayBill).WithMany().HasForeignKey(u => u.PayBillId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<VendorCredit.VendorCredit>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.Tax).HasColumnType("decimal(19,6)");

                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTax).HasColumnType("decimal(19,6)");

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.VendorId });

                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceipt).WithMany().HasForeignKey(u => u.ItemReceiptId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VendorCredit.VendorCreditDetail>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.PurchaseCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemReceiptItem).WithMany().HasForeignKey(u => u.ItemReceiptItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            #region receivePayment and customerCredit
            modelBuilder.Entity<ReceivePayment>(b =>
            {
                b.HasKey(u => u.Id);

                b.Property(u => u.MultiCurrencyChange).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaymentInvoice).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaymentCustomerCredit).HasColumnType("decimal(19,6)");

                b.Property(u => u.TotalOpenBalanceCustomerCredit).HasColumnType("decimal(19,6)");
                b.Property(u => u.Change).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentCustomerCredit).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentInvoice).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentDue).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaymentDueCustomerCredit).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.PaymentMethod).WithMany().HasForeignKey(u => u.PaymentMethodId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                //b.HasOne(u => u.CustomerCredit).WithMany().HasForeignKey(u => u.CustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);   

                b.Property(e => e.TotalCashInvoice).HasColumnType("decimal(19,6)");
                b.Property(e => e.TotalCreditInvoice).HasColumnType("decimal(19,6)");
                b.Property(e => e.TotalExpenseInvoice).HasColumnType("decimal(19,6)");
                b.Property(e => e.TotalCashCustomerCredit).HasColumnType("decimal(19,6)");
                b.Property(e => e.TotalCreditCustomerCredit).HasColumnType("decimal(19,6)");
                b.Property(e => e.TotalExpenseCustomerCredit).HasColumnType("decimal(19,6)");
            });
            modelBuilder.Entity<ReceivePaymentDetail>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.TotalAmount).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.Payment).HasColumnType("decimal(19,6)");

                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyPayment).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalAmount).HasColumnType("decimal(19,6)");

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.ReceivePayment).WithMany().HasForeignKey(u => u.ReceivePaymentId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Invoice).WithMany().HasForeignKey(u => u.InvoiceId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerCredit).WithMany().HasForeignKey(u => u.CustomerCreditId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

                b.Property(e => e.Cash).HasColumnType("decimal(19,6)");
                b.Property(e => e.Credit).HasColumnType("decimal(19,6)");
                b.Property(e => e.Expense).HasColumnType("decimal(19,6)");
                b.Property(e => e.MultiCurrencyCash).HasColumnType("decimal(19,6)");
                b.Property(e => e.MultiCurrencyCredit).HasColumnType("decimal(19,6)");
                b.Property(e => e.MultiCurrencyExpense).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<ReceivePaymentExpense>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Amount).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.ReceivePayment).WithMany().HasForeignKey(u => u.ReceivePaymentId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<CustomerCredit>(b =>
            {
                b.HasKey(u => u.Id);

                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTax).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaid).HasColumnType("decimal(19,6)");

                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.Tax).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerId });

                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);

                //  b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueSale).WithMany().HasForeignKey(u => u.ItemIssueSaleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<CustomerCreditDetail>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.SalePrice).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueSaleItem).WithMany().HasForeignKey(u => u.ItemIssueSaleItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

            });
            #endregion


            modelBuilder.Entity<SaleOrder>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.OrderNumber });
                b.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.Reference });
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.MultiCurrency).WithMany().HasForeignKey(u => u.MultiCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleTransactionType).WithMany().HasForeignKey(u => u.SaleTransactionTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.Property(p => p.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.Tax).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTax).HasColumnType("decimal(19,6)");
                b.Property(p => p.ApprovalStatus).HasDefaultValue(ApprovalStatus.Recorded);
            });
            modelBuilder.Entity<SaleOrderItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.SaleOrder).WithMany().HasForeignKey(u => u.SaleOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.DiscountRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.TaxRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");

            });

            modelBuilder.Entity<Invoices.Invoice>(b =>
            {
                b.HasKey(u => u.Id);

                b.Property(u => u.MultiCurrencyOpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTax).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyTotalPaid).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.Tax).HasColumnType("decimal(19,6)");
                b.Property(u => u.OpenBalance).HasColumnType("decimal(19,6)");
                b.Property(u => u.TotalPaid).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.CustomerId, });
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);

                b.HasOne(u => u.TransactionTypeSale).WithMany().HasForeignKey(u => u.TransactionTypeSaleId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssue).WithMany().HasForeignKey(u => u.ItemIssueId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<InvoiceItem>(b =>
            {
                b.HasKey(u => u.Id);

                b.Property(u => u.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(u => u.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.Total).HasColumnType("decimal(19,6)");
                b.Property(u => u.Qty).HasColumnType("decimal(19,6)");
                b.Property(u => u.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(u => u.DiscountRate).HasColumnType("decimal(19,6)");
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasIndex(e => new { e.TenantId, e.ParentId });
                b.HasOne(u => u.Invoice).WithMany().HasForeignKey(u => u.InvoiceId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ItemIssueItem).WithMany().HasForeignKey(u => u.ItemIssueItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.SaleOrderItem).WithMany(u => u.InvoiceItems).HasForeignKey(u => u.OrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.DeliveryScheduleItem).WithMany(u => u.InvoiceItems).HasForeignKey(u => u.DeliverySchedulItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Deposit>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.ReceiveFromVendorId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.ReceiveFromCustomerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");

            });
            modelBuilder.Entity<DepositItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Deposit).WithMany().HasForeignKey(u => u.DepositId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Account).WithMany().HasForeignKey(u => u.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);

                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
            });


            modelBuilder.Entity<VendorTypeMember>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Member).WithMany().HasForeignKey(u => u.MemberId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.VendorType).WithMany().HasForeignKey(u => u.VendorTypeId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerTypeMember>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Member).WithMany().HasForeignKey(u => u.MemberId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.CustomerType).WithMany().HasForeignKey(u => u.CustomerTypeId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Gallery>(b =>
            {
                b.HasKey(k => k.Id);
                b.HasIndex(i => new { i.TenantId, i.Name });
                b.HasIndex(i => new { i.TenantId, i.UploadFrom });
                b.HasIndex(i => new { i.TenantId, i.UploadSource });
            });

            modelBuilder.Entity<InvoiceTemplate>(b =>
            {
                b.HasKey(k => k.Id);
                b.HasIndex(i => new { i.TenantId, i.Name });
                b.HasIndex(i => new { i.TenantId, i.TemplateType });
                b.HasIndex(p => new { p.TenantId, p.IsActive });
                b.HasOne(k => k.Gallery).WithMany().HasForeignKey(k => k.GalleryId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InvoiceTemplateMap>(b =>
            {
                b.HasKey(k => k.Id);
                b.HasIndex(i => new { i.TenantId, i.TemplateType });
                b.HasOne(k => k.InvoiceTemplate).WithMany().HasForeignKey(k => k.TemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(k => k.SaleType).WithMany().HasForeignKey(k => k.SaleTypeId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<ItemLot>(b =>
            {
                b.HasIndex(k => new { k.TenantId, k.ItemId, k.LotId }).IsUnique(true);
                b.HasOne(i => i.Item).WithMany().HasForeignKey(i => i.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.Lot).WithMany().HasForeignKey(i => i.LotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryTransactionItem>(b =>
            {
                b.HasIndex(k => k.TransactionId);
                b.HasIndex(k => k.Date);
                b.HasIndex(k => k.OrderIndex);
                b.HasIndex(k => k.JournalId);
                b.HasIndex(k => k.LocationId);
                b.HasIndex(k => k.LotId);
                b.HasIndex(k => k.TransferOrProductionId);
                b.HasIndex(k => k.TransferOrProductionItemId);
                b.HasIndex(k => k.JournalType);
                b.HasIndex(k => k.ItemId);
                b.HasIndex(k => k.InventoryAccountId);
                b.HasIndex(k => k.IsItemReceipt);
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.LineCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.QtyOnHand).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.AvgCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.AdjustmentCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.LatestCost).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<InventoryCalculationItem>(b =>
            {
                b.HasOne(s => s.Item).WithMany().HasForeignKey(s => s.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductionLine>(b =>
            {
                b.HasIndex(s => new { s.TenantId, s.ProductionLineName }).IsUnique(true);
            });

            modelBuilder.Entity<ProductionPlan>(b =>
            {
                b.HasIndex(s => new { s.TenantId, s.DocumentNo }).IsUnique(true);
                b.HasIndex(s => s.Reference);
                b.HasOne(s => s.Location).WithMany().HasForeignKey(s => s.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(s => s.ProductionLine).WithMany().HasForeignKey(s => s.ProductionLineId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalIssueQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalReceiptQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalIssueNetWeight).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalReceiptNetWeight).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<ProductionStandardCost>(b =>
            {
                b.HasOne(p => p.ProductionPlan).WithMany().HasForeignKey(p => p.ProductionPlanId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.StandardCostGroup).WithMany().HasForeignKey(p => p.StandardCostGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalNetWeight).HasColumnType("decimal(19,6)");
                b.Property(p => p.QtyPercentage).HasColumnType("decimal(19,6)");
                b.Property(p => p.NetWeightPercentage).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<ProductionIssueStandardCost>(b =>
            {
                b.HasOne(p => p.ProductionPlan).WithMany().HasForeignKey(p => p.ProductionPlanId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(p => p.StandardCostGroup).WithMany().HasForeignKey(p => p.StandardCostGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.TotalQty).HasColumnType("decimal(19,6)");
                b.Property(p => p.TotalNetWeight).HasColumnType("decimal(19,6)");
                b.Property(p => p.QtyPercentage).HasColumnType("decimal(19,6)");
                b.Property(p => p.NetWeightPercentage).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<BatchNo>(b =>
            {
                b.HasIndex(s => new { s.TenantId, s.Code, s.ItemId }).IsUnique(true);
                b.HasIndex(s => s.IsStandard);
                b.HasIndex(s => s.ExpirationDate);
            });

            modelBuilder.Entity<ItemIssueItemBatchNo>(b =>
            {
                b.HasOne(e => e.ItemIssueItem).WithMany().HasForeignKey(e => e.ItemIssueItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemReceiptItemBatchNo>(b =>
            {
                b.HasOne(e => e.ItemReceiptItem).WithMany().HasForeignKey(e => e.ItemReceiptItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemIssueVendorCreditItemBatchNo>(b =>
            {
                b.HasOne(e => e.ItemIssueVendorCreditItem).WithMany().HasForeignKey(e => e.ItemIssueVendorCreditItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemReceiptCustomerCreditItemBatchNo>(b =>
            {
                b.HasOne(e => e.ItemReceiptCustomerCreditItem).WithMany().HasForeignKey(e => e.ItemReceiptCustomerCreditItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VendorCreditItemBatchNo>(b =>
            {
                b.HasOne(e => e.VendorCreditItem).WithMany().HasForeignKey(e => e.VendorCreditItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerCreditItemBatchNo>(b =>
            {
                b.HasOne(e => e.CustomerCreditItem).WithMany().HasForeignKey(e => e.CustomerCreditItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BatchNo).WithMany().HasForeignKey(e => e.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryCostCloseItemBatchNo>(b =>
            {
                b.HasOne(s => s.InventoryCostClose).WithMany().HasForeignKey(s => s.InventoryCostCloseId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(s => s.Lot).WithMany().HasForeignKey(s => s.LotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(s => s.BatchNo).WithMany().HasForeignKey(s => s.BatchNoId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscribableEdition>(b =>
            {
                b.Property(e => e.IsPaid).IsRequired(true);
                b.Property(e => e.IsPaid).HasDefaultValue(false);
               
            });

            modelBuilder.Entity<CashFlowAccountGroup>(b =>
            {
                b.HasIndex(e => e.Name);
                b.HasIndex(e => e.SortOrder);
            });

            modelBuilder.Entity<CashFlowCategory>(b =>
            {
                b.HasIndex(e => e.Name);
                b.HasIndex(e => e.SortOrder);
                b.HasIndex(e => e.Type);
                b.HasIndex(e => e.IsDefault);
            });

            modelBuilder.Entity<CashFlowTemplate>(b =>
            {
                b.HasIndex(e => e.Name);
                b.HasIndex(e => e.IsDefault);
                b.HasIndex(e => e.IsActive);
            });

            modelBuilder.Entity<CashFlowTemplateCategory>(b =>
            {
                b.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Template).WithMany().HasForeignKey(e => e.TemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CashFlowTemplateAccount>(b =>
            {
                b.HasOne(e => e.Template).WithMany().HasForeignKey(e => e.TemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Category).WithMany().HasForeignKey(e => e.CategoryId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.AccontGroup).WithMany().HasForeignKey(e => e.AccountGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.OutAccontGroup).WithMany().HasForeignKey(e => e.OutAccountGroupId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Account).WithMany().HasForeignKey(e => e.AccountId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SignUp>(b =>
            {
                b.HasIndex(e => e.FirstName);
                b.HasIndex(e => e.LastName);
                b.HasIndex(e => e.PhoneNumber);
                b.HasIndex(e => e.Position);
                b.Property(e => e.Description).HasMaxLength(128);
                b.HasOne(e => e.Referral).WithMany().HasForeignKey(e => e.ReferralId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Tenant).WithMany().HasForeignKey(e => e.TenantId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ItemCodeFormula>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
               
            });

            modelBuilder.Entity<ItemCodeFormulaProperty>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(e => e.ItemCodeFormula).WithMany().HasForeignKey(e => e.ItemCodeFormulaId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Property).WithMany().HasForeignKey(e => e.PropertyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);            
            });

            modelBuilder.Entity<ItemCodeFormulaCustom>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => e.Prefix);
                b.HasIndex(e => e.ItemCode);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(e => e.ItemCodeFormula).WithMany().HasForeignKey(e => e.ItemCodeFormulaId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);              
            });

            modelBuilder.Entity<ItemCodeFormulaItemType>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
                b.HasOne(e => e.ItemCodeFormula).WithMany().HasForeignKey(e => e.ItemCodeFormulaId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.ItemType).WithMany().HasForeignKey(e => e.ItemTypeId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);             
            });



            modelBuilder.Entity<ApiClient>(b =>
            {
                b.HasIndex(e => e.Name);
                b.HasIndex(e => e.ClientId);
                b.HasIndex(e => e.Secret);           
            });


            modelBuilder.Entity<Feature>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique(true);
                b.HasIndex(e => e.SortOrder);
            });

            modelBuilder.Entity<Package>(b =>
            {
                b.HasIndex(e => e.Name).IsUnique(true);
                b.HasIndex(e => e.SortOrder);
            });

            modelBuilder.Entity<PackageEdition>(b =>
            {
                b.HasIndex(e => new { e.PackageId, e.EditionId }).IsUnique(true);
                b.HasIndex(e => e.SortOrder);
                b.HasOne(e => e.Package).WithMany().HasForeignKey(e => e.PackageId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Edition).WithMany().HasForeignKey(e => e.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PackageEditionFeature>(b =>
            {
                b.HasIndex(e => new { e.PackageId, e.EditionId, e.FeatureId }).IsUnique(true);
                b.HasOne(e => e.Package).WithMany().HasForeignKey(e => e.PackageId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Edition).WithMany().HasForeignKey(e => e.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Feature).WithMany().HasForeignKey(e => e.FeatureId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            //modelBuilder.Entity<PackageEditionPromotion>(b =>
            //{
            //    b.HasIndex(e => new { e.PackageId, e.EditionId, e.PromotionId }).IsUnique(true);
            //    b.HasOne(e => e.Package).WithMany().HasForeignKey(e => e.PackageId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            //    b.HasOne(e => e.Edition).WithMany().HasForeignKey(e => e.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            //    b.HasOne(e => e.Promotion).WithMany().HasForeignKey(e => e.PromotionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            //});


            modelBuilder.Entity<Promotion>(b =>
            {
                b.HasIndex(e => e.PromotionName);
                b.HasIndex(e => e.PromotionType);
                b.Property(p => p.DiscountRate).HasColumnType("decimal(19,6)");
                b.Property(p => p.ExtraMonth).HasColumnType("decimal(19,6)");
            });

            modelBuilder.Entity<PromotionCampaign>(b =>
            {
                b.HasIndex(e => e.CampaignType);
                b.HasOne(e => e.Promotion).WithMany().HasForeignKey(e => e.PromotionId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Package).WithMany().HasForeignKey(e => e.PackageId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PromotionCampaignEdition>(b =>
            {
                b.HasIndex(e => new { e.PromotionCampaignId, e.EditionId, e.PromotionId }).IsUnique(true);
                b.HasOne(e => e.PromotionCampaign).WithMany().HasForeignKey(e => e.PromotionCampaignId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Edition).WithMany().HasForeignKey(e => e.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Promotion).WithMany().HasForeignKey(e => e.PromotionId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscriptionPromotion>(b =>
            {
                b.HasOne(e => e.PromotionCampaign).WithMany().HasForeignKey(e => e.CampaignId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Promotion).WithMany().HasForeignKey(e => e.PromotionId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Subscription).WithMany().HasForeignKey(e => e.SubscriptionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SubscriptionCampaignPromotion>(b =>
            {
                b.HasOne(e => e.PromotionCampaign).WithMany().HasForeignKey(e => e.CampaignId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Edition).WithMany().HasForeignKey(e => e.EditionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.Promotion).WithMany().HasForeignKey(e => e.PromotionId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.SubscriptionPromotion).WithMany(e => e.SubscriptionCampaignPromotions).HasForeignKey(e => e.SubscriptionPromotionId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<JournalTransactionType>(b =>
            {
                b.HasIndex(e => new { e.Name});
                b.HasIndex(e => new { e.InventoryTransactionType});
            });

            modelBuilder.Entity<Referral>(b =>
            {
                b.HasIndex(e => new { e.Name });
                b.HasIndex(e => new { e.Code }).IsUnique();
            });

            modelBuilder.Entity<Bom>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.Name });
                b.HasIndex(e => new { e.IsDefault });
                b.HasOne(e => e.Item).WithMany().HasForeignKey(e => e.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BomItem>(b =>
            {
                b.HasKey(u => u.Id);              
                b.HasOne(e => e.Item).WithMany().HasForeignKey(e => e.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(e => e.BOM).WithMany().HasForeignKey(e => e.BomId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });



            modelBuilder.Entity<KitchenOrder>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, e.OrderNumber });
                b.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.Reference });
                b.HasOne(u => u.CreatorUser).WithMany().HasForeignKey(u => u.CreatorUserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.MultiCurrency).WithMany().HasForeignKey(u => u.MultiCurrencyId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);              
                b.OwnsOne(p => p.BillingAddress);
                b.OwnsOne(p => p.ShippingAddress);
                b.Property(p => p.SubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.Tax).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencySubTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyTax).HasColumnType("decimal(19,6)");          
            });

            modelBuilder.Entity<KitchenOrderItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.KitchenOrder).WithMany().HasForeignKey(u => u.KitchenOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
                b.Property(p => p.UnitCost).HasColumnType("decimal(19,6)");
                b.Property(p => p.DiscountRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.Total).HasColumnType("decimal(19,6)");
                b.Property(p => p.TaxRate).HasColumnType("decimal(6,4)");
                b.Property(p => p.MultiCurrencyTotal).HasColumnType("decimal(19,6)");
                b.Property(p => p.MultiCurrencyUnitCost).HasColumnType("decimal(19,6)");

            });

            modelBuilder.Entity<KitchenOrderItemAndBOMItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.BomItem).WithMany().HasForeignKey(u => u.BomItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Lot).WithMany().HasForeignKey(u => u.LotId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.KitchenOrderItem).WithMany().HasForeignKey(u => u.KitchenOrderItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Tax).WithMany().HasForeignKey(u => u.TaxId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId });
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");              
                b.Property(p => p.TaxRate).HasColumnType("decimal(6,4)");            
            });

            modelBuilder.Entity<Card>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CardId }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.CardNumber }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.SerialNumber}).IsUnique(true);                        
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
              
            });
            modelBuilder.Entity<POSSetting>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PurchaseOrderExchangeRate>(b =>
            {
                b.HasOne(u => u.PurchaseOrder).WithMany().HasForeignKey(u => u.PurchaseOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BillExchangeRate>(b =>
            {
                b.HasOne(u => u.Bill).WithMany().HasForeignKey(u => u.BillId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VendorCredit.VendorCreditExchangeRate>(b =>
            {
                b.HasOne(u => u.VendorCredit).WithMany().HasForeignKey(u => u.VendorCreditId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PayBillExchangeRate>(b =>
            {
                b.HasOne(u => u.PayBill).WithMany().HasForeignKey(u => u.PayBillId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PayBillItemExchangeRate>(b =>
            {
                b.HasOne(u => u.PayBillItem).WithMany().HasForeignKey(u => u.PayBillItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SaleOrderExchangeRate>(b =>
            {
                b.HasOne(u => u.SaleOrder).WithMany().HasForeignKey(u => u.SaleOrderId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InvoiceExchangeRate>(b =>
            {
                b.HasOne(u => u.Invoice).WithMany().HasForeignKey(u => u.InvoiceId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CustomerCreditExchangeRate>(b =>
            {
                b.HasOne(u => u.CustomerCredit).WithMany().HasForeignKey(u => u.CustomerCreditId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReceivePaymentExchangeRate>(b =>
            {
                b.HasOne(u => u.ReceivePayment).WithMany().HasForeignKey(u => u.ReceivePaymentId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReceivePaymentItemExchangeRate>(b =>
            {
                b.HasOne(u => u.ReceivePaymentItem).WithMany().HasForeignKey(u => u.ReceivePaymentItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.FromCurrency).WithMany().HasForeignKey(u => u.FromCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.ToCurrency).WithMany().HasForeignKey(u => u.ToCurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<DeliverySchedule>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, });
                b.HasIndex(e => new { e.TenantId, e.DeliveryNo }).IsUnique(true);
                b.HasIndex(e => new { e.TenantId, e.Reference });
                b.HasOne(u => u.SaleOrder).WithMany().HasForeignKey(u => u.SaleOrderId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);            
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);          
            });

            modelBuilder.Entity<DeliveryScheduleItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(e => new { e.TenantId, e.CreatorUserId, });
                b.Property(p => p.Qty).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.DeliverySchedule).WithMany().HasForeignKey(u => u.DeliveryScheduleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                //b.HasOne(u => u.SaleOrderItem).WithMany(u => u.DeliveryScheduleItems).HasForeignKey(u => u.SaleOrderItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchasePrice>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasOne(u => u.Location).WithMany().HasForeignKey(u => u.LocationId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PurchasePriceItem>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => new { u.PurchasePriceId, u.ItemId, u.VendorId, u.CurrencyId }).IsUnique(true);
                b.HasOne(u => u.Currency).WithMany().HasForeignKey(u => u.CurrencyId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Item).WithMany().HasForeignKey(u => u.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.Property(p => p.Price).HasColumnType("decimal(19,6)");
                b.HasOne(u => u.PurchasePrice).WithMany(u => u.PurchasePriceItems).HasForeignKey(u => u.PurchasePriceId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(u => u.Vendor).WithMany().HasForeignKey(u => u.VendorId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TestParameter>(b =>
            {
                b.HasIndex(i => i.Name);
            });

            modelBuilder.Entity<QCTestTemplate>(b =>
            {
                b.HasIndex(i => i.Name);
            });

            modelBuilder.Entity<QCTestTemplateParameter>(b =>
            {
                b.HasOne(i => i.QCTestTemplate).WithMany().HasForeignKey(i => i.QCTestTemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QCSample>(b =>
            {
                b.HasIndex(i => i.SampleId).IsUnique(true);
                b.HasOne(i => i.Item).WithMany().HasForeignKey(i => i.ItemId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.Location).WithMany().HasForeignKey(i => i.LocationId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LabTestRequest>(b =>
            {
                b.HasOne(i => i.QCTestTemplate).WithMany().HasForeignKey(i => i.QCTestTemplateId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.QCSample).WithMany().HasForeignKey(i => i.QCSampleId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.Lab).WithMany().HasForeignKey(i => i.LabId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LabTestResult>(b =>
            {
                b.HasIndex(i => new { i.ReferenceNo, i.TenantId, i.LabId }).IsUnique(true);
                b.HasOne(i => i.LabTestRequest).WithMany().HasForeignKey(i => i.LabTestRequestId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.Lab).WithMany().HasForeignKey(i => i.LabId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<LabTestResultDetail>(b =>
            {
                b.HasOne(i => i.LabTestResult).WithMany().HasForeignKey(i => i.LabTestResultId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
                b.HasOne(i => i.TestParameter).WithMany().HasForeignKey(i => i.TestParameterId).IsRequired(true).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }

    }
}
