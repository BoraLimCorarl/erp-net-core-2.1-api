using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Abp.Threading;
using Microsoft.AspNetCore.Identity;
using CorarlERP.Authorization.Users;
using CorarlERP.MultiTenancy;
using CorarlERP.AccountCycles;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Collections.Generic;
using Abp.Linq.Extensions;

using Newtonsoft.Json.Linq;
using System.IO;
using Abp.Application.Services.Dto;
using CorarlERP.Configuration;
using Abp.Dependency;
using CorarlERP.UserGroups;
using CorarlERP.Locations;
using static CorarlERP.enumStatus.EnumStatus;
using Abp.UI;
using CorarlERP.Locks;
using CorarlERP.LockTransactions.Dto;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using CorarlERP.Timing;
using Abp.Timing.Timezone;
using CorarlERP.VendorTypes;
using CorarlERP.Authorization.Users.Dto;
using CorarlERP.Locations.Dto;
using CorarlERP.Customers.Dto;
using CorarlERP.Customers;
using CorarlERP.Inventories.Data;
using CorarlERP.Items;
using CorarlERP.Vendors;
using CorarlERP.Vendors.Dto;
using CorarlERP.ChartOfAccounts.Dto;
using CorarlERP.ChartOfAccounts;
using CorarlERP.Dto;
using CorarlERP.Currencies.Dto;
using CorarlERP.Currencies;
using Abp.Extensions;
using CorarlERP.PropertyValues;
using CorarlERP.CustomerTypes;
using CorarlERP.TransactionTypes;
using CorarlERP.Taxes;
using CorarlERP.Taxes.Dto;
using CorarlERP.TransactionTypes.Dto;
using CorarlERP.ItemReceiptCustomerCredits;
using CorarlERP.PurchaseOrders;
using CorarlERP.SaleOrders;
using Abp.Domain.Uow;
using CorarlERP.InventoryTransactionItems;
using CorarlERP.Reports.Dto;
using System.Transactions;
using CorarlERP.Journals;
using CorarlERP.ItemReceipts;
using CorarlERP.ItemIssueVendorCredits;
using CorarlERP.ItemIssues;
using CorarlERP.EntityFrameworkCore.Repositories;
using static CorarlERP.Authorization.Roles.StaticRoleNames;
using Hangfire;
using CorarlERP.InventoryCalculationSchedules;
using CorarlERP.InventoryCalculationJobSchedules;
using Hangfire.Storage;
using CorarlERP.Productions;
using CorarlERP.BatchNos;
using CorarlERP.Bills;
using CorarlERP.Invoices;
using CorarlERP.TransferOrders;
using CorarlERP.InventoryCostCloses;
using Abp.Timing;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization;
using CorarlERP.DeliverySchedules;

namespace CorarlERP
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class CorarlERPAppServiceBase : ApplicationService
    {

        protected const string Item_Service = "Service";
        protected const string Item_Item = "Item";
        protected const long Item_Type_Service = 3;

        protected const string EvosLicenKey = "Evo_License_Key";
        protected const string EvosIPAddress = "Evo_IP_Address";
        protected const string EvosTCPPort = "Evo_TCP_Port";


        protected string SubscriptionBotToken => _configurationAccessor.Configuration["Telegram:Token:SubscriptionBot"];
        protected string SignupBotToken => _configurationAccessor.Configuration["Telegram:Token:SignupBot"];
        protected string SubscriptionGroup => _configurationAccessor.Configuration["Telegram:Groups:Subscription"];
        protected string SignupGroup => _configurationAccessor.Configuration["Telegram:Groups:Signup"];
        protected string SubscriptionGroupUrl => $"https://t.me/{SubscriptionGroup}/";
        protected string SignupGroupUrl => $"https://t.me/{SignupGroup}/";

        protected TimeZoneInfo MyTimeZoneInfo()
        {

            TimeZoneInfo tz = TimezoneHelper.FindTimeZoneInfo("SE Asia Standard Time");

            return tz;
        }
        protected async Task<DateTime> GetCurrentDateInUserTimezoneAnync()
        {
            var currentUser = await GetCurrentUserAsync();
            var defaultTimeZoneId = await SettingManager.GetSettingValueForUserAsync(TimingSettingNames.TimeZone,
                 currentUser.ToUserIdentifier());

            var timezone = TimezoneHelper.FindTimeZoneInfo(defaultTimeZoneId);

            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);

        }

        protected string GetEvoLicenKey()
        {
            //var appsettingsjson = JObject.Parse(File.ReadAllText("appsettings.json"));

            //var key = JObject.Parse(File.ReadAllText("appsettings.json"))[EvosLicenKey];

            //return key.ToString();

            return _configurationAccessor.Configuration[EvosLicenKey];

        }


        protected string GetEvoIPAddress()
        {
            return _configurationAccessor.Configuration[EvosIPAddress];
        }

        protected uint GetEvosTCPPort()
        {
            var tcpPortStr = _configurationAccessor.Configuration[EvosTCPPort];
            if (uint.TryParse(tcpPortStr, out uint tcpPort))
            {
                return tcpPort;
            }

            return 40001;
        }



        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }


        private readonly IRepository<AccountCycle, long> _accountCycleRepository;

        private readonly IAppConfigurationAccessor _configurationAccessor;
        private readonly IRepository<UserGroupMember, Guid> _userGroupMemberRepository;

        private readonly IRepository<Location, long> _locationRepository;

        private readonly ILockManager _lockManager;
        private readonly IRepository<Lock, long> _lockRepository;
        private readonly IRepository<PermissionLock, long> _permissionLockRepository;

        private readonly ITimeZoneService _timeZoneService;
        private readonly ITimeZoneConverter _timeZoneConverter;

        private readonly IRepository<VendorTypeMember, long> _vendorTypeMemberRepository;
        private readonly IRepository<Vendor, Guid> _vendorRepository;
        private readonly IRepository<VendorType, long> _vendorTypeRepository;
        private readonly IRepository<VendorGroup, Guid> _vendorGroupRepository;
        private readonly IRepository<Customer, Guid> _customerRepository;
        private readonly IRepository<CustomerType, long> _customerTypeRepository;
        private readonly IRepository<CustomerGroup, Guid> _customerGroupRepository;
        private readonly IRepository<Item, Guid> _itemRepository;
        private readonly IRepository<ItemProperty, Guid> _itemPropertyRepository;
        private readonly IRepository<PropertyValue, long> _propertyValueRepository;
        private readonly IRepository<Property, long> _propertyRepository;

        private readonly IRepository<ItemsUserGroup, Guid> _itemUserGroupRepository;
        private readonly IRepository<User, long> _userRepository;

        private readonly IRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        protected readonly IRepository<Currency, long> _currencyRepository;

        private readonly IRepository<TransactionType, long> _transactionTypeRepository;
        private readonly IRepository<Tax, long> _taxRepository;
        private readonly IRepository<ItemReceiptItemCustomerCredit, Guid> _itemReceiptCustomerCreditItemRepository;
        private readonly IRepository<SaleOrderItem, Guid> _saleOrderItemRepository;
        private readonly IRepository<SaleOrder, Guid> _saleOrderRepository;
        private readonly ISaleOrderManager _saleOrderManager;

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Journal, Guid> _journalRepository;
        private readonly IRepository<JournalItem, Guid> _journalItemRepository;
        private readonly IRepository<ItemReceiptItem, Guid> _itemReceiptItemRepository;
        private readonly IRepository<ItemIssueItem, Guid> _itemIssueItemRepository;
        private readonly IRepository<ItemIssueVendorCreditItem, Guid> _itemIssueVendorCreditItemRepository;
        private readonly ICorarlRepository<InventoryTransactionItem, Guid> _inventoryTransactionItemRepository;
        private readonly IInventoryCalculationJobScheduleManager _inventoryCalculationScheduleJobManager;

        private readonly IRepository<VendorTypeMember, long> _vendorTypeMemeberRepository;
        private readonly IVendorTypeMemberManager _vendorTypeMemeberManager;
        private readonly IRepository<CustomerTypeMember, long> _customerTypeMemeberRepository;

        private readonly ICorarlRepository<Production, Guid> _productionRepository;
        private readonly ICorarlRepository<BillItem, Guid> _billItemRepository;
        private readonly ICorarlRepository<InvoiceItem, Guid> _invoiceItemRepository;
        private readonly ICorarlRepository<TransferOrderItem, Guid> _transferItemRepository;
        private readonly ICorarlRepository<RawMaterialItems, Guid> _rawMaterialItemRepository;
        private readonly ICorarlRepository<FinishItems, Guid> _finishItemRepository;

        private readonly ICorarlRepository<ItemReceiptItemBatchNo, Guid> _itemReceiptItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueItemBatchNo, Guid> _itemIssueItemBatchNoRepository;
        private readonly ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid> _itemReceiptCustomerCreditItemBatchNoRepository;
        private readonly ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid> _itemIssueVendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<CustomerCreditItemBatchNo, Guid> _customerCreditItemBatchNoRepository;
        private readonly ICorarlRepository<VendorCreditItemBatchNo, Guid> _vendorCreditItemBatchNoRepository;
        private readonly ICorarlRepository<InventoryCostCloseItemBatchNo, Guid> _invnetoryCostCloseItemBatchNoRepository;
        private readonly ICorarlRepository<BatchNo, Guid> _batchNoRepository;
        private readonly ICorarlRepository<DeliverySchedule, Guid> _deliveryScheduleRepository;
        private readonly ICorarlRepository<DeliveryScheduleItem, Guid> _deliveryScheduleItemRepository;
        protected CorarlERPAppServiceBase()
        {
            _configurationAccessor = IocManager.Instance.Resolve<IAppConfigurationAccessor>();
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
            _accountCycleRepository = IocManager.Instance.Resolve<IRepository<AccountCycle, long>>();

            _lockManager = IocManager.Instance.Resolve<ILockManager>();
            _lockRepository = IocManager.Instance.Resolve<IRepository<Lock, long>>();
            _permissionLockRepository = IocManager.Instance.Resolve<IRepository<PermissionLock, long>>();

            _timeZoneService = IocManager.Instance.Resolve<ITimeZoneService>();
            _timeZoneConverter = IocManager.Instance.Resolve<ITimeZoneConverter>();

            _vendorTypeMemberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _vendorRepository = IocManager.Instance.Resolve<IRepository<Vendor, Guid>>();
            _vendorGroupRepository = IocManager.Instance.Resolve<IRepository<VendorGroup, Guid>>();

            _customerRepository = IocManager.Instance.Resolve<IRepository<Customer, Guid>>();
            _customerGroupRepository = IocManager.Instance.Resolve<IRepository<CustomerGroup, Guid>>();

            _itemRepository = IocManager.Instance.Resolve<IRepository<Item, Guid>>();
            _itemPropertyRepository = IocManager.Instance.Resolve<IRepository<ItemProperty, Guid>>();
            _propertyValueRepository = IocManager.Instance.Resolve<IRepository<PropertyValue, long>>();
            _propertyRepository = IocManager.Instance.Resolve<IRepository<Property, long>>();
            _itemUserGroupRepository = IocManager.Instance.Resolve<IRepository<ItemsUserGroup, Guid>>();

            _userGroupMemberRepository = IocManager.Instance.Resolve<IRepository<UserGroupMember, Guid>>();

            _locationRepository = IocManager.Instance.Resolve<IRepository<Location, long>>();
            _chartOfAccountRepository = IocManager.Instance.Resolve<IRepository<ChartOfAccount, Guid>>();

            _userRepository = IocManager.Instance.Resolve<IRepository<User, long>>();

            _currencyRepository = IocManager.Instance.Resolve<IRepository<Currency, long>>();
            _customerTypeRepository = IocManager.Instance.Resolve<IRepository<CustomerType, long>>();
            _vendorTypeRepository = IocManager.Instance.Resolve<IRepository<VendorType, long>>();

            _transactionTypeRepository = IocManager.Instance.Resolve<IRepository<TransactionType, long>>();
            _taxRepository = IocManager.Instance.Resolve<IRepository<Tax, long>>();

            _saleOrderManager = IocManager.Instance.Resolve<ISaleOrderManager>();
            _saleOrderRepository = IocManager.Instance.Resolve<IRepository<SaleOrder, Guid>>();
            _saleOrderItemRepository = IocManager.Instance.Resolve<IRepository<SaleOrderItem, Guid>>();
            _itemReceiptCustomerCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItemCustomerCredit, Guid>>();

            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            _journalRepository = IocManager.Instance.Resolve<IRepository<Journal, Guid>>();
            _journalItemRepository = IocManager.Instance.Resolve<IRepository<JournalItem, Guid>>();
            _itemReceiptItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItem, Guid>>();
            _itemIssueItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueItem, Guid>>();
            _itemIssueVendorCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueVendorCreditItem, Guid>>();
            _inventoryTransactionItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryTransactionItem, Guid>>();
            _inventoryCalculationScheduleJobManager = IocManager.Instance.Resolve<IInventoryCalculationJobScheduleManager>();

            _vendorTypeMemeberManager = IocManager.Instance.Resolve<IVendorTypeMemberManager>();
            _vendorTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _customerTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<CustomerTypeMember, long>>();
            _productionRepository = IocManager.Instance.Resolve<ICorarlRepository<Production, Guid>>();
            _billItemRepository = IocManager.Instance.Resolve<ICorarlRepository<BillItem, Guid>>();
            _invoiceItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InvoiceItem, Guid>>();
            _transferItemRepository = IocManager.Instance.Resolve<ICorarlRepository<TransferOrderItem, Guid>>();
            _rawMaterialItemRepository = IocManager.Instance.Resolve<ICorarlRepository<RawMaterialItems, Guid>>();
            _finishItemRepository = IocManager.Instance.Resolve<ICorarlRepository<FinishItems, Guid>>();

            _itemReceiptItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptItemBatchNo, Guid>>();
            _itemIssueItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueItemBatchNo, Guid>>();
            _itemReceiptCustomerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid>>();
            _itemIssueVendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid>>();
            _customerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<CustomerCreditItemBatchNo, Guid>>();
            _vendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<VendorCreditItemBatchNo, Guid>>();
            _batchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<BatchNo, Guid>>();
            _invnetoryCostCloseItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCostCloseItemBatchNo, Guid>>();
            _deliveryScheduleRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliverySchedule, Guid>>();
            _deliveryScheduleItemRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliveryScheduleItem, Guid>>();
        }


        protected CorarlERPAppServiceBase(IRepository<AccountCycle, long> accountCycleRepository,
            IRepository<UserGroupMember, Guid> userGroupMemberRepository, IRepository<Location, long> locationRepository)
        {
            _configurationAccessor = IocManager.Instance.Resolve<IAppConfigurationAccessor>();
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
            _accountCycleRepository = accountCycleRepository;

            _lockManager = IocManager.Instance.Resolve<ILockManager>();
            _lockRepository = IocManager.Instance.Resolve<IRepository<Lock, long>>();
            _permissionLockRepository = IocManager.Instance.Resolve<IRepository<PermissionLock, long>>();

            _timeZoneService = IocManager.Instance.Resolve<ITimeZoneService>();
            _timeZoneConverter = IocManager.Instance.Resolve<ITimeZoneConverter>();

            _vendorTypeMemberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _vendorRepository = IocManager.Instance.Resolve<IRepository<Vendor, Guid>>();
            _vendorGroupRepository = IocManager.Instance.Resolve<IRepository<VendorGroup, Guid>>();

            _customerRepository = IocManager.Instance.Resolve<IRepository<Customer, Guid>>();
            _customerGroupRepository = IocManager.Instance.Resolve<IRepository<CustomerGroup, Guid>>();

            _itemRepository = IocManager.Instance.Resolve<IRepository<Item, Guid>>();
            _itemPropertyRepository = IocManager.Instance.Resolve<IRepository<ItemProperty, Guid>>();
            _propertyValueRepository = IocManager.Instance.Resolve<IRepository<PropertyValue, long>>();
            _propertyRepository = IocManager.Instance.Resolve<IRepository<Property, long>>();
            _itemUserGroupRepository = IocManager.Instance.Resolve<IRepository<ItemsUserGroup, Guid>>();

            _userGroupMemberRepository = userGroupMemberRepository;

            _locationRepository = locationRepository;
            _chartOfAccountRepository = IocManager.Instance.Resolve<IRepository<ChartOfAccount, Guid>>();
            _userRepository = IocManager.Instance.Resolve<IRepository<User, long>>();

            _currencyRepository = IocManager.Instance.Resolve<IRepository<Currency, long>>();
            _customerTypeRepository = IocManager.Instance.Resolve<IRepository<CustomerType, long>>();
            _vendorTypeRepository = IocManager.Instance.Resolve<IRepository<VendorType, long>>();

            _transactionTypeRepository = IocManager.Instance.Resolve<IRepository<TransactionType, long>>();
            _taxRepository = IocManager.Instance.Resolve<IRepository<Tax, long>>();

            _saleOrderManager = IocManager.Instance.Resolve<ISaleOrderManager>();
            _saleOrderRepository = IocManager.Instance.Resolve<IRepository<SaleOrder, Guid>>();
            _saleOrderItemRepository = IocManager.Instance.Resolve<IRepository<SaleOrderItem, Guid>>();
            _itemReceiptCustomerCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItemCustomerCredit, Guid>>();

            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            _journalRepository = IocManager.Instance.Resolve<IRepository<Journal, Guid>>();
            _journalItemRepository = IocManager.Instance.Resolve<IRepository<JournalItem, Guid>>();
            _itemReceiptItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItem, Guid>>();
            _itemIssueItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueItem, Guid>>();
            _itemIssueVendorCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueVendorCreditItem, Guid>>();
            _inventoryTransactionItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryTransactionItem, Guid>>();
            _inventoryCalculationScheduleJobManager = IocManager.Instance.Resolve<IInventoryCalculationJobScheduleManager>();

            _vendorTypeMemeberManager = IocManager.Instance.Resolve<IVendorTypeMemberManager>();
            _vendorTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _customerTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<CustomerTypeMember, long>>();
            _productionRepository = IocManager.Instance.Resolve<ICorarlRepository<Production, Guid>>();
            _billItemRepository = IocManager.Instance.Resolve<ICorarlRepository<BillItem, Guid>>();
            _invoiceItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InvoiceItem, Guid>>();
            _transferItemRepository = IocManager.Instance.Resolve<ICorarlRepository<TransferOrderItem, Guid>>();
            _rawMaterialItemRepository = IocManager.Instance.Resolve<ICorarlRepository<RawMaterialItems, Guid>>();
            _finishItemRepository = IocManager.Instance.Resolve<ICorarlRepository<FinishItems, Guid>>();

            _itemReceiptItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptItemBatchNo, Guid>>();
            _itemIssueItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueItemBatchNo, Guid>>();
            _itemReceiptCustomerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid>>();
            _itemIssueVendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid>>();
            _customerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<CustomerCreditItemBatchNo, Guid>>();
            _vendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<VendorCreditItemBatchNo, Guid>>();
            _batchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<BatchNo, Guid>>();
            _invnetoryCostCloseItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCostCloseItemBatchNo, Guid>>();
            _deliveryScheduleRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliverySchedule, Guid>>();
            _deliveryScheduleItemRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliveryScheduleItem, Guid>>();
        }


        protected CorarlERPAppServiceBase(IRepository<UserGroupMember, Guid> userGroupMemberRepository,
            IRepository<Location, long> locationRepository)
        {

            _configurationAccessor = IocManager.Instance.Resolve<IAppConfigurationAccessor>();
            LocalizationSourceName = CorarlERPConsts.LocalizationSourceName;
            _accountCycleRepository = IocManager.Instance.Resolve<IRepository<AccountCycle, long>>();

            _lockManager = IocManager.Instance.Resolve<ILockManager>();
            _lockRepository = IocManager.Instance.Resolve<IRepository<Lock, long>>();
            _permissionLockRepository = IocManager.Instance.Resolve<IRepository<PermissionLock, long>>();

            _timeZoneService = IocManager.Instance.Resolve<ITimeZoneService>();
            _timeZoneConverter = IocManager.Instance.Resolve<ITimeZoneConverter>();

            _vendorTypeMemberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _vendorRepository = IocManager.Instance.Resolve<IRepository<Vendor, Guid>>();
            _vendorGroupRepository = IocManager.Instance.Resolve<IRepository<VendorGroup, Guid>>();

            _customerGroupRepository = IocManager.Instance.Resolve<IRepository<CustomerGroup, Guid>>();
            _customerRepository = IocManager.Instance.Resolve<IRepository<Customer, Guid>>();

            _itemRepository = IocManager.Instance.Resolve<IRepository<Item, Guid>>();
            _itemPropertyRepository = IocManager.Instance.Resolve<IRepository<ItemProperty, Guid>>();
            _propertyValueRepository = IocManager.Instance.Resolve<IRepository<PropertyValue, long>>();
            _propertyRepository = IocManager.Instance.Resolve<IRepository<Property, long>>();
            _itemUserGroupRepository = IocManager.Instance.Resolve<IRepository<ItemsUserGroup, Guid>>();

            _userGroupMemberRepository = userGroupMemberRepository;

            _locationRepository = locationRepository;
            _chartOfAccountRepository = IocManager.Instance.Resolve<IRepository<ChartOfAccount, Guid>>();
            _userRepository = IocManager.Instance.Resolve<IRepository<User, long>>();

            _currencyRepository = IocManager.Instance.Resolve<IRepository<Currency, long>>();
            _customerTypeRepository = IocManager.Instance.Resolve<IRepository<CustomerType, long>>();
            _vendorTypeRepository = IocManager.Instance.Resolve<IRepository<VendorType, long>>();

            _transactionTypeRepository = IocManager.Instance.Resolve<IRepository<TransactionType, long>>();
            _taxRepository = IocManager.Instance.Resolve<IRepository<Tax, long>>();

            _saleOrderManager = IocManager.Instance.Resolve<ISaleOrderManager>();
            _saleOrderRepository = IocManager.Instance.Resolve<IRepository<SaleOrder, Guid>>();
            _saleOrderItemRepository = IocManager.Instance.Resolve<IRepository<SaleOrderItem, Guid>>();
            _itemReceiptCustomerCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItemCustomerCredit, Guid>>();

            _unitOfWorkManager = IocManager.Instance.Resolve<IUnitOfWorkManager>();
            _journalRepository = IocManager.Instance.Resolve<IRepository<Journal, Guid>>();
            _journalItemRepository = IocManager.Instance.Resolve<IRepository<JournalItem, Guid>>();
            _itemReceiptItemRepository = IocManager.Instance.Resolve<IRepository<ItemReceiptItem, Guid>>();
            _itemIssueItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueItem, Guid>>();
            _itemIssueVendorCreditItemRepository = IocManager.Instance.Resolve<IRepository<ItemIssueVendorCreditItem, Guid>>();
            _inventoryTransactionItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryTransactionItem, Guid>>();
            _inventoryCalculationScheduleJobManager = IocManager.Instance.Resolve<IInventoryCalculationJobScheduleManager>();

            _vendorTypeMemeberManager = IocManager.Instance.Resolve<IVendorTypeMemberManager>();
            _vendorTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<VendorTypeMember, long>>();
            _customerTypeMemeberRepository = IocManager.Instance.Resolve<IRepository<CustomerTypeMember, long>>();
            _productionRepository = IocManager.Instance.Resolve<ICorarlRepository<Production, Guid>>();
            _billItemRepository = IocManager.Instance.Resolve<ICorarlRepository<BillItem, Guid>>();
            _invoiceItemRepository = IocManager.Instance.Resolve<ICorarlRepository<InvoiceItem, Guid>>();
            _transferItemRepository = IocManager.Instance.Resolve<ICorarlRepository<TransferOrderItem, Guid>>();
            _rawMaterialItemRepository = IocManager.Instance.Resolve<ICorarlRepository<RawMaterialItems, Guid>>();
            _finishItemRepository = IocManager.Instance.Resolve<ICorarlRepository<FinishItems, Guid>>();

            _itemReceiptItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptItemBatchNo, Guid>>();
            _itemIssueItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueItemBatchNo, Guid>>();
            _itemReceiptCustomerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemReceiptCustomerCreditItemBatchNo, Guid>>();
            _itemIssueVendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<ItemIssueVendorCreditItemBatchNo, Guid>>();
            _customerCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<CustomerCreditItemBatchNo, Guid>>();
            _vendorCreditItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<VendorCreditItemBatchNo, Guid>>();
            _batchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<BatchNo, Guid>>();
            _invnetoryCostCloseItemBatchNoRepository = IocManager.Instance.Resolve<ICorarlRepository<InventoryCostCloseItemBatchNo, Guid>>();
            _deliveryScheduleRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliverySchedule, Guid>>();
            _deliveryScheduleItemRepository = IocManager.Instance.Resolve<ICorarlRepository<DeliveryScheduleItem, Guid>>();
        }

        protected IQueryable<VendorTypeMemberDto> GetVendorTypeMembers()
        {
            var query = _vendorTypeMemeberRepository.GetAll().Include(s => s.VendorType).Where(s => s.MemberId == AbpSession.UserId.Value).AsNoTracking()
                        .Select(s => new VendorTypeMemberDto
                        {
                            Id = s.Id,
                            MemberId = s.MemberId,
                            VendorTypeId = s.VendorTypeId,
                            VendorTypeName = s.VendorType.VendorTypeName,
                        });
            return query;

        }

        protected IQueryable<CustomerTypeMemberDto> GetCustomerTypeMembers()
        {
            var query = _customerTypeMemeberRepository.GetAll().Include(s => s.CustomerType).Where(s => s.MemberId == AbpSession.UserId.Value).AsNoTracking()
                        .Select(s => new CustomerTypeMemberDto
                        {
                            Id = s.Id,
                            MemberId = s.MemberId,
                            CustomerTypeId = s.CustomerTypeId,
                            CustomerTypeName = s.CustomerType.CustomerTypeName,
                        });
            return query;

        }

        protected async Task UpdateOrderInventoryStatus(Guid OrderId)
        {
            var entity = await _saleOrderRepository.FirstOrDefaultAsync(s => s.Id == OrderId);

            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var saleOrderItems = await _saleOrderItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)                                    
                                        .Where(s => s.SaleOrderId == OrderId)
                                        .AsNoTracking()
                                        .ToListAsync();
            var deliveryScheduleItems = await _deliveryScheduleItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)
                                        .Where(s => s.DeliverySchedule.SaleOrderId == OrderId)
                                        .AsNoTracking()
                                        .ToListAsync();

            if (saleOrderItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0) && deliveryScheduleItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0))
            {
                entity.UpdateReceiveStatusToPending();
            }
            else
            {
                var totalOrderQty = saleOrderItems.Sum(s => s.Qty);
                var totalIssueQty = saleOrderItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty)) + deliveryScheduleItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty));
                var totalReturnQty = 0m;

                var itemIssueIds = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                if (itemIssueIds.Any())
                {
                var totalReturnQtyOrder = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                           .AsNoTracking()
                                           .Select(s => s.Qty)
                                           .SumAsync();

                    var itemIssueIdDeliveries = deliveryScheduleItems.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                    var totalReturnQtyShedule = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => itemIssueIdDeliveries.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIdDeliveries.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                           .AsNoTracking()
                                           .Select(s => s.Qty)
                                           .SumAsync();
                    totalReturnQty = totalReturnQtyOrder + totalReturnQtyShedule;
                }


                if (totalOrderQty - totalIssueQty + totalReturnQty == 0)
                {
                    entity.UpdateReceiveStatusToReceiveAll();
                }
                else if (totalIssueQty > 0 && totalIssueQty == totalReturnQty)
                {
                    entity.UpdateReceiveStatusToPending();
                }
                else
                {
                    entity.UpdateReceiveStatusToPartial();
                }
            }

            //Update Issue Count
            var issueCount = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();
            var invoiceCount = saleOrderItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();
            var issueCountDelivery = deliveryScheduleItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();    
            var invoiceCountDelivery = deliveryScheduleItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();
            entity.SetIssueCount(issueCount + invoiceCount + issueCountDelivery + invoiceCountDelivery);
            await _saleOrderManager.UpdateAsync(entity);

        }

        protected async Task UpdateDeliveryInventoryStatus(Guid DeliveryId, bool updateSaleOrder)
        {
            var entity = await _deliveryScheduleRepository.FirstOrDefaultAsync(s => s.Id == DeliveryId);

            if (entity == null) throw new UserFriendlyException("RecordNotFound");

            var deliveryItems = await _deliveryScheduleItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)
                                        .Where(s => s.DeliveryScheduleId == DeliveryId)
                                        .AsNoTracking()
                                        .ToListAsync();
            var totalDeliveryQty = 0m;
            var totalReturnDeliveryQty = 0m;
            var totalIssueDeliveryQty = 0m;
            if (deliveryItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0))
            {
                entity.UpdateReceiveStatusToPending();
            }
            else
            {
                totalDeliveryQty = deliveryItems.Sum(s => s.Qty);
                totalIssueDeliveryQty = deliveryItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty));


                var itemIssueIds = deliveryItems.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                if (itemIssueIds.Any())
                {
                    totalReturnDeliveryQty = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                           .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                           .AsNoTracking()
                                           .Select(s => s.Qty)
                                           .SumAsync();
                }


                if (totalDeliveryQty - totalIssueDeliveryQty + totalReturnDeliveryQty == 0)
                {
                    entity.UpdateReceiveStatusToReceiveAll();
                }
                else if (totalDeliveryQty > 0 && totalIssueDeliveryQty == totalReturnDeliveryQty)
                {
                    entity.UpdateReceiveStatusToPending();
                }
                else
                {
                    entity.UpdateReceiveStatusToPartial();
                }
            }
            await _deliveryScheduleRepository.UpdateAsync(entity);

            #region Update Status SaleOrder

            if (updateSaleOrder)
            {
                var OrderId = entity.SaleOrderId;
                var entitySaleOrder = await _saleOrderRepository.FirstOrDefaultAsync(s => s.Id == OrderId);

                if (entitySaleOrder == null) throw new UserFriendlyException("RecordNotFound");

                var saleOrderItems = await _saleOrderItemRepository.GetAll()
                                            .Include(s => s.ItemIssueItems)
                                            .Include(s => s.InvoiceItems)
                                            .Where(s => s.SaleOrderId == OrderId)
                                            .AsNoTracking()
                                            .ToListAsync();

                var deliveryScheduleItems = await _deliveryScheduleItemRepository.GetAll()
                                        .Include(s => s.ItemIssueItems)
                                        .Include(s => s.InvoiceItems)
                                        .Where(s => s.DeliverySchedule.SaleOrderId == OrderId)
                                        .AsNoTracking()
                                        .ToListAsync();

                var totalQtyIssueDelivery = deliveryScheduleItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty));
                if (saleOrderItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0) && deliveryScheduleItems.All(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Count() == 0 && s.ItemIssueItems.Count() == 0))
                {
                    entitySaleOrder.UpdateReceiveStatusToPending();
                }
                else
                {
                    var totalOrderQty = saleOrderItems.Sum(s => s.Qty);
                    var totalIssueQty = saleOrderItems.Sum(s => s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Sum(i => i.Qty) + s.ItemIssueItems.Sum(r => r.Qty) ) + totalQtyIssueDelivery;
                    var totalReturnQty = 0m;

                    var itemIssueIds = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(r => r.Id).Concat(s.InvoiceItems.Where(r => r.ItemIssueItemId.HasValue).Select(i => i.ItemIssueItemId.Value))).ToList();
                    if (itemIssueIds.Any())
                    {
                        totalReturnQty = await _itemReceiptCustomerCreditItemRepository.GetAll()
                                               .Where(s => itemIssueIds.Contains(s.ItemIssueSaleItemId.Value) || (s.CustomerCreditItemId.HasValue && itemIssueIds.Contains(s.CustomerCreditItem.ItemIssueSaleItemId.Value)))
                                               .AsNoTracking()
                                               .Select(s => s.Qty)
                                               .SumAsync() ;
                    }


                    if (totalOrderQty - totalIssueQty + totalReturnQty + totalReturnDeliveryQty == 0)
                    {
                        entitySaleOrder.UpdateReceiveStatusToReceiveAll();
                    }
                    else if (totalIssueQty > 0 && totalIssueQty == totalReturnQty + totalReturnDeliveryQty)
                    {
                        entitySaleOrder.UpdateReceiveStatusToPending();
                    }
                    else
                    {
                        entitySaleOrder.UpdateReceiveStatusToPartial();
                    }
                }

                //Update Issue Count
                var issueCount = saleOrderItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();
                var invoiceCount = saleOrderItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();
                var issueCountDelivery = deliveryScheduleItems.SelectMany(s => s.ItemIssueItems.Select(b => b.ItemIssueId)).GroupBy(g => g).Count();
                var invoiceCountDelivery = deliveryScheduleItems.SelectMany(s => s.InvoiceItems.Where(b => b.ItemIssueItemId.HasValue).Select(b => b.InvoiceId)).GroupBy(g => g).Count();
                entitySaleOrder.SetIssueCount(issueCount + invoiceCount + issueCountDelivery + invoiceCountDelivery);
                await _saleOrderManager.UpdateAsync(entitySaleOrder);
            }
            #endregion



        }


        protected IQueryable<VendorTypeMemberDto> GetVendorTypeMembers(long? userId = null)
        {
            var _userId = userId ?? AbpSession.UserId.Value;
            var query = _vendorTypeMemberRepository.GetAll().Include(s => s.VendorType).Where(s => s.MemberId == _userId).AsNoTracking()
                        .Select(s => new VendorTypeMemberDto
                        {
                            Id = s.Id,
                            MemberId = s.MemberId,
                            VendorTypeId = s.VendorTypeId,
                            VendorTypeName = s.VendorType.VendorTypeName,
                        });
            return query;

        }


        protected async Task<string> GetUserDefaultTimeZone()
        {
            var defaultTimezoneId = await _timeZoneService.GetDefaultTimezoneAsync(Abp.Configuration.SettingScopes.User, AbpSession.TenantId);
            return defaultTimezoneId;
        }

        //protected async Task<string> ConverToUserTimeZoneFromUTC(DateTime utcTime)
        //{
        //    var defaultTimeZoneId = await GetUserDefaultTimeZone();

        //}

        protected async Task CleanTransactionLockPassword(long? id)
        {
            
            if (id == null) throw new UserFriendlyException(L("IsNotValid", L("PermissionLock")));

            var query = await _permissionLockRepository.GetAll()
                        .Where(t => t.Id == id).FirstOrDefaultAsync();

            if (query != null)
            {           
                await _permissionLockRepository.DeleteAsync(query);
            }
        }

        protected async Task DebugLogWriter(int tenantId, bool debugMode, string message, IAppFolders _appFolders)
        {
            await Task.Run(() =>
            {

                if (!debugMode) return;
                var tokenName = $"debug_{tenantId}.txt";
                var path = Path.Combine(_appFolders.WebLogsFolder, tokenName);
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine($"{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}: {message}");
                    }
                }
                else
                {
                    File.Delete(path);
                    // This text is always added, making the file longer over time
                    // if it is not deleted.
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine($"{DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")}: {message}");
                    }
                }

            });

        }

        protected virtual async Task<User> GetCurrentUserAsync()
        {
            var user = await UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual User GetCurrentUser()
        {
            return AsyncHelper.RunSync(GetCurrentUserAsync);
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
            }
        }

        protected virtual Tenant GetCurrentTenant()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                return TenantManager.GetById(AbpSession.GetTenantId());
            }
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected virtual async Task<AccountCycle> GetPreviousCloseCyleAsync(DateTime date)
        {
            var query = await _accountCycleRepository.GetAll().AsNoTracking()
                            .Where(u => u.EndDate != null && u.EndDate.Value.Date <= date.Date)
                            .OrderByDescending(u => u.EndDate.Value).FirstOrDefaultAsync();
            return query;
        }

        protected virtual async Task<AccountCycle> GetCurrentCycleAsync()
        {
            var query = await _accountCycleRepository.GetAll().AsNoTracking()
                        .Where(u => u.EndDate == null)
                        .FirstOrDefaultAsync();
            return query;
        }


        protected virtual async Task<AccountCycle> GetPreviousRoundingCloseCyleAsync(DateTime date)
        {
            var result = await _accountCycleRepository.GetAll().AsNoTracking()
                          .Where(u => u.StartDate.Date <= date.Date && (u.EndDate == null || date.Date <= u.EndDate.Value.Date))
                          .OrderByDescending(u => u.StartDate).FirstOrDefaultAsync();
            return result;

            //// 1. query check if current cycle date is now with current start date
            //var queryHasEndDateNull = await _accountCycleRepository.GetAll().AsNoTracking()
            //               .Where(u => u.EndDate == null && u.StartDate.Date <= date.Date)
            //               .OrderByDescending(u => u.EndDate.Value).FirstOrDefaultAsync();
            //var query = new AccountCycle(); 
            //if (queryHasEndDateNull == null) {
            //    query = await GetPreviousCloseCyleAsync(date);
            //}
            //return queryHasEndDateNull != null ? queryHasEndDateNull : query;
        }

        protected virtual async Task<List<AccountCycle>> GetCloseCyleAsync(DateTime? fromDate = null)
        {
            var query = await _accountCycleRepository
                            .GetAll()
                            .AsNoTracking()
                            .WhereIf(fromDate != null, u => u.StartDate >= fromDate.Value)
                            .ToListAsync();


            if (query == null || !query.Any()) query = await _accountCycleRepository
                               .GetAll().AsNoTracking().Where(s => s.EndDate == null).ToListAsync();

            return query;
        }

        protected virtual async Task<List<string>> GetUserPermissions(long userId)
        {
            var user = await UserManager.GetUserByIdAsync(userId);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);
            var result = grantedPermissions.Select(p => p.Name).ToList();
            var userRole = await UserManager.GetRolesAsync(user);
            if (userRole.Contains(StaticRoleNames.Tenants.Admin))
            {
                result = result.Where(t => !t.Equals(AppPermissions.Pages_Tenant_Inventorys_InventoryTransactions_EditDeleteby48Hour)  &&
                                           !t.Equals(AppPermissions.Pages_Tenant_Inventory_TransferOrder_EditDelete48hour) &&
                                           !t.Equals(AppPermissions.Pages_Tenant_Production_ProductionOrder_EditDeleteBy48hour)).Select(t=>t).ToList();
            }
            return result;
        }

        protected virtual async Task<List<long>> GetUserGroupByLocation()
        {
            var userId = AbpSession.GetUserId();
            // get user by group member
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Include(t => t.UserGroup)
                            .Where(x => x.MemberId == userId)
                            .Where(x => x.UserGroup.LocationId != null)
                            .AsNoTracking()
                            .Select(x => x.UserGroup.LocationId.Value);

            var @queryLocation = _locationRepository
                                .GetAll()
                                .Where(t => t.Member == Member.All && t.IsActive == true)
                                .AsNoTracking()
                                .Select(t => t.Id);
            var result = await userGroups.Union(@queryLocation).ToListAsync();
            return result;
        }

        protected string FormatNumberCurrency(decimal number, int digit)
        {
            var formatDigit = "{0:n" + digit + "}";
            var result = String.Format(formatDigit, number);
            return result;
        }
        protected string FormatDate(DateTime date, string formatValue)
        {
            var formatDate = "{0:" + formatValue + "}";
            var result = String.Format(formatDate, date);
            return result;
        }

        protected async Task CheckClosePeriod(DateTime lastDate, DateTime newDate)
        {
            var validationDateClosePeriod = await _accountCycleRepository.GetAll()
             .Where(t => t.EndDate == null && (t.StartDate.Date > lastDate.Date || t.StartDate.Date > newDate.Date)).AsNoTracking().CountAsync();

            if (validationDateClosePeriod > 0)
            {
                throw new UserFriendlyException(L("PeriodIsClose"));
            }
        }

        #region Query Helper

        protected virtual IQueryable<TaxSummaryOutput> GetTaxs()
        {
            var result = _taxRepository.GetAll()
                         .AsNoTracking()
                            .Select(s => new TaxSummaryOutput
                            {
                                Id = s.Id,
                                TaxName = s.TaxName,
                                TaxRate = s.TaxRate
                            });

            return result;
        }

        protected virtual IQueryable<TransactionTypeSummaryOutput> GetTransactionTypes(List<long> saleTypeIds)
        {
            var result = _transactionTypeRepository.GetAll()
                               .WhereIf(saleTypeIds != null && saleTypeIds.Any(), u => saleTypeIds.Contains(u.Id))
                               .AsNoTracking()
                               .Select(s => new TransactionTypeSummaryOutput
                               {
                                   Id = s.Id,
                                   TransactionTypeName = s.TransactionTypeName
                               });
            return result;
        }

        protected virtual IQueryable<UserDto> GetUsers(List<long?> userIds)
        {
            var users = _userRepository.GetAll()
                .WhereIf(userIds != null && userIds.Count > 0, s => userIds.Contains(s.Id))
                .AsNoTracking()
                .Select(s => new UserDto
                {
                    Id = s.Id,
                    UserName = s.UserName
                });

            return users;
        }

        protected virtual IQueryable<CurrencySummaryOutput> GetCurrencies()
        {
            var currencyQuery = _currencyRepository.GetAll()
                              .AsNoTracking()
                              .Select(s => new CurrencySummaryOutput
                              {
                                  Id = s.Id,
                                  Code = s.Code
                              });

            return currencyQuery;
        }

        protected virtual IQueryable<GetAccountOutput> GetAccounts(
           List<Guid?> accountIds = null,
           List<long> accountTypeIds = null)
        {
            var aIds = accountIds == null ? null : accountIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetAccounts(aIds, accountTypeIds);
        }

        protected virtual IQueryable<GetAccountOutput> GetAccounts(
            List<Guid> accountIds = null,
            List<long> accountTypeIds = null)
        {
            var accountQuery = _chartOfAccountRepository.GetAll()
                                .WhereIf(accountIds != null && accountIds.Count() > 0, s => accountIds.Contains(s.Id))
                                .WhereIf(accountTypeIds != null && accountTypeIds.Count() > 0, s => accountTypeIds.Contains(s.AccountTypeId))
                                .AsNoTracking()
                                .Select(s => new GetAccountOutput
                                {
                                    Id = s.Id,
                                    AccountName = s.AccountName
                                });

            return accountQuery;
        }

        protected virtual IQueryable<ChartAccountSummaryOutput> GetAccountWithCode(
           List<Guid?> accountIds = null,
           List<long> accountTypeIds = null)
        {
            var aIds = accountIds == null ? null : accountIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetAccountWithCode(aIds, accountTypeIds);
        }

        protected virtual IQueryable<ChartAccountSummaryOutput> GetAccountWithCode(
            List<Guid> accountIds = null,
            List<long> accountTypeIds = null)
        {
            var accountQuery = _chartOfAccountRepository.GetAll()
                                .WhereIf(accountIds != null && accountIds.Count() > 0, s => accountIds.Contains(s.Id))
                                .WhereIf(accountTypeIds != null && accountTypeIds.Count() > 0, s => accountTypeIds.Contains(s.AccountTypeId))
                                .AsNoTracking()
                                .Select(s => new ChartAccountSummaryOutput
                                {
                                    Id = s.Id,
                                    AccountCode = s.AccountCode,
                                    AccountName = s.AccountName
                                });

            return accountQuery;
        }

        protected virtual IQueryable<GetAccountWithTypeOutput> GetAccountWithType(
           List<Guid> accountIds = null,
           List<long> accountTypeIds = null)
        {
            var accountQuery = _chartOfAccountRepository.GetAll()
                                .WhereIf(accountIds != null && accountIds.Count() > 0, s => accountIds.Contains(s.Id))
                                .WhereIf(accountTypeIds != null && accountTypeIds.Count() > 0, s => accountTypeIds.Contains(s.AccountTypeId))
                                .AsNoTracking()
                                .Select(s => new GetAccountWithTypeOutput
                                {
                                    Id = s.Id,
                                    AccountCode = s.AccountCode,
                                    AccountName = s.AccountName,
                                    AccountTypeId = s.AccountTypeId,
                                    AccountTypeName = s.AccountType.AccountTypeName,
                                    TypeCode = s.AccountType.Type
                                });

            return accountQuery;
        }

        protected virtual async Task<List<long>> GetUserGroupLocations()
        {
            var userGroups = _userGroupMemberRepository.GetAll()
                            .Where(x => x.MemberId == AbpSession.UserId)
                            .Where(x => x.UserGroup.LocationId != null)
                            .AsNoTracking()
                            .Select(x => x.UserGroup.LocationId.Value);

            return await userGroups.ToListAsync();
        }

        protected virtual IQueryable<LocationSummaryDto> GetLocations(
            List<long> userGroupLocations,
            List<long?> locationIds = null)
        {
            var ids = locationIds == null ? null : locationIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetLocations(userGroupLocations, ids);
        }

        protected virtual IQueryable<LocationSummaryDto> GetLocations(
            List<long> userGroupLocations,
            List<long> locationIds = null)
        {
            return _locationRepository.GetAll()
                    .WhereIf(userGroupLocations != null && userGroupLocations.Any(), s => userGroupLocations.Contains(s.Id) || s.Member == Member.All)
                    .WhereIf(locationIds != null && locationIds.Count() > 0, s => locationIds.Contains(s.Id))
                    .AsNoTracking()
                    .Select(l => new LocationSummaryDto
                    {
                        Id = l.Id,
                        LocationName = l.LocationName,
                    });
        }

        protected async Task<List<Guid>> GetUserGroupVendors()
        {
            var vendorGroupQuery = from g in _vendorGroupRepository.GetAll()
                                               .AsNoTracking()
                                               .Select(s => new { s.UserGroupId, s.VendorId })
                                   join m in _userGroupMemberRepository.GetAll()
                                             .Where(s => s.MemberId == AbpSession.UserId)
                                             .AsNoTracking()
                                             .Select(s => s.UserGroupId)
                                   on g.UserGroupId equals m
                                   into ms
                                   where ms.Count() > 0
                                   select g.VendorId;

            return await vendorGroupQuery.Distinct().ToListAsync();
        }


        protected async Task<List<Guid>> GetUserGroupCustomers()
        {
            var customerGroupQuery = from g in _customerGroupRepository.GetAll()
                                            .AsNoTracking()
                                            .Select(s => new { s.UserGroupId, s.CustomerId })
                                     join m in _userGroupMemberRepository.GetAll()
                                               .Where(s => s.MemberId == AbpSession.UserId)
                                               .AsNoTracking()
                                               .Select(s => s.UserGroupId)
                                     on g.UserGroupId equals m
                                     into ms
                                     where ms.Count() > 0
                                     select g.CustomerId;

            return await customerGroupQuery.Distinct().ToListAsync();
        }

        protected virtual IQueryable<GetCustomerOutput> GetCustomers(
            List<Guid> userGroupCustomers,
            List<Guid?> customerIds,
            List<long?> customerTypeIds,
            List<long> customerMemberTypeIds)
        {
            var cIds = customerIds == null ? null : customerIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            var tIds = customerTypeIds == null ? null : customerTypeIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetCustomers(userGroupCustomers, cIds, tIds, customerMemberTypeIds);
        }

        protected virtual IQueryable<GetCustomerOutput> GetCustomers(
            List<Guid> userGroupCustomers,
            List<Guid> customerIds,
            List<long> customerTypeIds,
            List<long> customerMemberTypeIds)
        {
            var result = _customerRepository.GetAll()
                                            .WhereIf(userGroupCustomers != null && userGroupCustomers.Any(), s => userGroupCustomers.Contains(s.Id) || s.Member == Member.All)
                                            .WhereIf(customerIds != null && customerIds.Count() > 0, s => customerIds.Contains(s.Id))
                                            .WhereIf(customerTypeIds != null && customerTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerTypeIds.Contains(s.CustomerTypeId.Value))
                                            .WhereIf(customerMemberTypeIds != null && customerMemberTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerMemberTypeIds.Contains(s.CustomerTypeId.Value))
                                            .AsNoTracking()
                                            .Select(s => new GetCustomerOutput
                                            {
                                                Id = s.Id,
                                                CustomerName = s.CustomerName
                                            });
            return result;
        }

        protected virtual IQueryable<GetCustomerWithCodeOutput> GetCustomerWithCode(
            List<Guid> userGroupCustomers,
            List<Guid> customerIds,
            List<long> customerTypeIds,
            List<long> customerMemberTypeIds)
        {
            var result = _customerRepository.GetAll()
                                            .WhereIf(userGroupCustomers != null && userGroupCustomers.Any(), s => userGroupCustomers.Contains(s.Id) || s.Member == Member.All)
                                            .WhereIf(customerIds != null && customerIds.Count() > 0, s => customerIds.Contains(s.Id))
                                            .WhereIf(customerTypeIds != null && customerTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerTypeIds.Contains(s.CustomerTypeId.Value))
                                            .WhereIf(customerMemberTypeIds != null && customerMemberTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerMemberTypeIds.Contains(s.CustomerTypeId.Value))
                                            .AsNoTracking()
                                            .Select(s => new GetCustomerWithCodeOutput
                                            {
                                                Id = s.Id,
                                                CustomerCode = s.CustomerCode,
                                                CustomerName = s.CustomerName
                                            });
            return result;
        }

        protected virtual IQueryable<GetCustomerWithTypeOutput> GetCustomerWithType(
           List<Guid> userGroupCustomers,
           List<Guid> customerIds,
           List<long> customerTypeIds,
           List<long> customerMemberTypeIds)
        {

            var result = _customerRepository.GetAll()
                            .WhereIf(userGroupCustomers != null && userGroupCustomers.Any(), s => userGroupCustomers.Contains(s.Id) || s.Member == Member.All)
                            .WhereIf(customerIds != null && customerIds.Count() > 0, s => customerIds.Contains(s.Id))
                            .WhereIf(customerTypeIds != null && customerTypeIds.Count() > 0, s => customerTypeIds.Contains(s.CustomerTypeId.Value))
                            .WhereIf(customerMemberTypeIds != null && customerMemberTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerMemberTypeIds.Contains(s.CustomerTypeId.Value))
                            .AsNoTracking()
                            .Select(s => new GetCustomerWithTypeOutput
                            {
                                Id = s.Id,
                                CustomerName = s.CustomerName,
                                CustomerTypeName = s.CustomerType == null ? "" : s.CustomerType.CustomerTypeName
                            });

            return result;
        }

        protected virtual IQueryable<GetCustomerWithCodeTypeOutput> GetCustomerWithCodeType(
           List<Guid> userGroupCustomers,
           List<Guid> customerIds,
           List<long> customerTypeIds,
           List<long> customerMemberTypeIds)
        {

            var result = _customerRepository.GetAll()
                            .WhereIf(userGroupCustomers != null && userGroupCustomers.Any(), s => userGroupCustomers.Contains(s.Id) || s.Member == Member.All)
                            .WhereIf(customerIds != null && customerIds.Count() > 0, s => customerIds.Contains(s.Id))
                            .WhereIf(customerTypeIds != null && customerTypeIds.Count() > 0, s => customerTypeIds.Contains(s.CustomerTypeId.Value))
                            .WhereIf(customerMemberTypeIds != null && customerMemberTypeIds.Count() > 0, s => s.CustomerTypeId.HasValue && customerMemberTypeIds.Contains(s.CustomerTypeId.Value))
                            .AsNoTracking()
                            .Select(s => new GetCustomerWithCodeTypeOutput
                            {
                                Id = s.Id,
                                CustomerCode = s.CustomerCode,
                                CustomerName = s.CustomerName,
                                CustomerTypeName = s.CustomerType == null ? "" : s.CustomerType.CustomerTypeName
                            });

            return result;
        }


        protected virtual IQueryable<GetVendorOutput> GetVendors(
            List<Guid> userGroupVendors,
            List<Guid?> vendorIds,
            List<long> vendorTypeIds,
            List<long> vendorTypeMemberIds)
        {
            var vIds = vendorIds == null ? null : vendorIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetVendors(userGroupVendors, vIds, vendorTypeIds, vendorTypeMemberIds);
        }

        protected virtual IQueryable<GetVendorOutput> GetVendors(
            List<Guid> userGroupVendors,
            List<Guid> vendorIds,
            List<long> vendorTypeIds,
            List<long> vendorTypeMemberIds)
        {
            var result = _vendorRepository.GetAll()
                        .WhereIf(userGroupVendors != null && userGroupVendors.Any(), s => userGroupVendors.Contains(s.Id) || s.Member == Member.All)
                        .WhereIf(vendorIds != null && vendorIds.Count() > 0, s => vendorIds.Contains(s.Id))
                        .WhereIf(vendorTypeIds != null && vendorTypeIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeIds.Contains(s.VendorTypeId.Value))
                        .WhereIf(vendorTypeMemberIds != null && vendorTypeMemberIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeMemberIds.Contains(s.VendorTypeId.Value))
                        .AsNoTracking()
                        .Select(s => new GetVendorOutput
                        {
                            Id = s.Id,
                            VendorName = s.VendorName
                        });
            return result;
        }

        protected virtual IQueryable<GetVendorWithCodeTypeOutput> GetVendorWithCodeType(
           List<Guid> userGroupVendors,
           List<Guid> vendorIds,
           List<long> vendorTypeIds,
           List<long> vendorTypeMemberIds)
        {
            var result = _vendorRepository.GetAll()
                        .WhereIf(userGroupVendors != null && userGroupVendors.Any(), s => userGroupVendors.Contains(s.Id) || s.Member == Member.All)
                        .WhereIf(vendorIds != null && vendorIds.Count() > 0, s => vendorIds.Contains(s.Id))
                        .WhereIf(vendorTypeIds != null && vendorTypeIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeIds.Contains(s.VendorTypeId.Value))
                        .WhereIf(vendorTypeMemberIds != null && vendorTypeMemberIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeMemberIds.Contains(s.VendorTypeId.Value))
                        .AsNoTracking()
                        .Select(s => new GetVendorWithCodeTypeOutput
                        {
                            Id = s.Id,
                            VendorCode = s.VendorCode,
                            VendorName = s.VendorName,
                            VendorTypeName = s.VendorType == null ? "" : s.VendorType.VendorTypeName
                        });
            return result;
        }

        protected virtual IQueryable<GetVendorWithCodeOutput> GetVendorWithCode(
            long? userId,
            List<Guid> vendorIds,
            List<long> vendorTypeIds,
            List<long> vendorTypeMemberIds)
        {
            if (userId.HasValue)
            {

                var vendorGroupQuery = from g in _vendorGroupRepository.GetAll()
                                                .AsNoTracking()
                                                .Select(s => new { s.UserGroupId, s.VendorId })
                                       join m in _userGroupMemberRepository.GetAll()
                                                 .Where(s => s.MemberId == userId)
                                                 .AsNoTracking()
                                                 .Select(s => s.UserGroupId)
                                       on g.UserGroupId equals m
                                       into ms
                                       where ms.Count() > 0
                                       select g;

                var result = from v in _vendorRepository.GetAll()
                                        .WhereIf(vendorIds != null && vendorIds.Count() > 0, s => vendorIds.Contains(s.Id))
                                        .WhereIf(vendorTypeIds != null && vendorTypeIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeIds.Contains(s.VendorTypeId.Value))
                                        .WhereIf(vendorTypeMemberIds != null && vendorTypeMemberIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeMemberIds.Contains(s.VendorTypeId.Value))
                                        .AsNoTracking()
                                        .Select(s => new
                                        {
                                            Id = s.Id,
                                            VendorCode = s.VendorCode,
                                            VendorName = s.VendorName,
                                            Member = s.Member
                                        })
                             join m in vendorGroupQuery
                             on v.Id equals m.VendorId
                             into ms
                             where v.Member == Member.All || ms.Count() > 0
                             select new GetVendorWithCodeOutput
                             {
                                 Id = v.Id,
                                 VendorCode = v.VendorCode,
                                 VendorName = v.VendorName
                             };
                return result;
            }
            else
            {
                var result = _vendorRepository.GetAll()
                            .WhereIf(vendorIds != null && vendorIds.Count() > 0, s => vendorIds.Contains(s.Id))
                            .WhereIf(vendorTypeIds != null && vendorTypeIds.Count() > 0, s => s.VendorTypeId.HasValue && vendorTypeIds.Contains(s.VendorTypeId.Value))
                            .AsNoTracking()
                            .Select(s => new GetVendorWithCodeOutput
                            {
                                Id = s.Id,
                                VendorCode = s.VendorCode,
                                VendorName = s.VendorName
                            });
                return result;
            }
        }

        protected async Task<List<Guid>> GetUserGroupItems()
        {
            var itemGroupQuery = from g in _itemUserGroupRepository.GetAll()
                                           .AsNoTracking()
                                           .Select(s => new { s.UserGroupId, s.ItemId })
                                 join m in _userGroupMemberRepository.GetAll()
                                           .Where(s => s.MemberId == AbpSession.GetUserId())
                                           .AsNoTracking()
                                           .Select(s => s.UserGroupId)
                                 on g.UserGroupId equals m
                                 into ms
                                 where ms.Count() > 0
                                 select g.ItemId;

            return await itemGroupQuery.Distinct().ToListAsync();
        }

        protected IQueryable<ItemPropertySummary> GetItemProperties(List<Guid> itemIds = null)
        {
            var itemPropertyQuery = from ip in _itemPropertyRepository.GetAll()
                                                .WhereIf(itemIds != null && itemIds.Count() > 0, s => itemIds.Contains(s.ItemId))
                                                .AsNoTracking()
                                                .Select(p => new
                                                {
                                                    PropertyValueId = p.PropertyValueId,
                                                    ItemId = p.ItemId,
                                                    PropertyId = p.PropertyId
                                                })
                                    join pv in _propertyValueRepository.GetAll()
                                       .AsNoTracking()
                                       .Select(s => new
                                       {
                                           Id = s.Id,
                                           NetWeight = s.NetWeight,
                                           Value = s.Value
                                       })
                                    on ip.PropertyValueId equals pv.Id
                                    join p in _propertyRepository.GetAll()
                                            .AsNoTracking()
                                            .Select(s => new
                                            {
                                                Id = s.Id,
                                                Name = s.Name,
                                                IsUnit = s.IsUnit,
                                                IsItemGroup = s.IsItemGroup
                                            })
                                    on ip.PropertyId equals p.Id
                                    select new ItemPropertySummary
                                    {
                                        Id = pv.Id,
                                        PropertyId = p.Id,
                                        PropertyName = p.Name,
                                        Value = pv.Value,
                                        NetWeight = pv.NetWeight,
                                        IsUnit = p.IsUnit,
                                        ValueId = pv.Id,
                                        ItemId = ip.ItemId,
                                        IsItemGroup = p.IsItemGroup
                                    };

            return itemPropertyQuery;
        }

        protected IQueryable<ItemWithPropertySummaryOutput> GetItemWithProperties(
           List<Guid> userGroupItems,
           List<Guid?> itemIds = null,
           List<GetListPropertyFilter> propertyDics = null)
        {
            var ids = itemIds == null ? null : itemIds.Where(s => s.HasValue).Select(s => s.Value).ToList();
            return GetItemWithProperties(userGroupItems, ids, propertyDics);
        }

        protected IQueryable<ItemWithPropertySummaryOutput> GetItemWithProperties(
           List<Guid> userGroupItems,
           List<Guid> itemIds = null,
           List<GetListPropertyFilter> propertyDics = null)
        {
            var itemPropertyQuery = GetItemProperties(itemIds);

            var itemQuery = from i in _itemRepository.GetAll()
                                    .WhereIf(userGroupItems != null && userGroupItems.Any(), s => userGroupItems.Contains(s.Id) || s.Member == Member.All)
                                    .WhereIf(itemIds != null && itemIds.Any(), s => itemIds.Contains(s.Id))
                                    .AsNoTracking()
                                    .Select(s => new
                                    {
                                        s.Id,
                                        s.ItemCode,
                                        s.ItemName,
                                        s.SalePrice
                                    })
                            join ip in itemPropertyQuery
                            on i.Id equals ip.ItemId
                            into props

                            where propertyDics == null || propertyDics.Count == 0 ||
                                    props.Where(i =>
                                        propertyDics.Any(v => v.PropertyId == i.PropertyId) &&
                                        propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                        (propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                            propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                            propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.Id))
                                    ).Count() == propertyDics.Count

                            select new ItemWithPropertySummaryOutput
                            {
                                Id = i.Id,
                                ItemCode = i.ItemCode,
                                ItemName = i.ItemName,
                                SalePrice = i.SalePrice ?? 0,
                                Properties = props.ToList()
                            };

            return itemQuery;

        }

        protected IQueryable<ItemWithPropertySummaryOutput> GetItemWithPropertiesOld(
            long? userId,
            List<Guid> itemIds = null,
            List<GetListPropertyFilter> propertyDics = null)
        {

            if (userId.HasValue)
            {
                var itemQuery = from i in _itemRepository.GetAll()
                                      .WhereIf(itemIds != null && itemIds.Count() > 0, s => itemIds.Contains(s.Id))
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          s.Id,
                                          s.ItemCode,
                                          s.ItemName,
                                          s.SalePrice,
                                          s.Member
                                      })
                                join ip in _itemPropertyRepository.GetAll()
                                    .Include(s => s.Property)
                                    .Include(s => s.PropertyValue)
                                    .AsNoTracking()
                                    .Select(p => new ItemPropertySummary
                                    {
                                        Id = p.PropertyValueId ?? 0,
                                        PropertyId = p.PropertyId,
                                        PropertyName = p.Property == null ? "" : p.Property.Name,
                                        Value = p.PropertyValue == null ? "" : p.PropertyValue.Value,
                                        NetWeight = p.PropertyValue == null ? 0 : p.PropertyValue.NetWeight,
                                        IsUnit = p.Property != null && p.Property.IsUnit,
                                        ValueId = p.PropertyValueId,
                                        ItemId = p.ItemId,

                                    })
                                on i.Id equals ip.ItemId
                                into props

                                where propertyDics == null || propertyDics.Count == 0 ||
                                     (props.Where(i =>
                                         propertyDics.Any(v => v.PropertyId == i.PropertyId) &&
                                         propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                         (propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                             propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                             propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.Id))
                                      ).Count() == propertyDics.Count)

                                select new
                                {
                                    Id = i.Id,
                                    ItemCode = i.ItemCode,
                                    ItemName = i.ItemName,
                                    SalePrice = i.SalePrice ?? 0,
                                    Properties = props.ToList(),
                                    Member = i.Member
                                };

                var itemGroupQuery = from g in _itemUserGroupRepository.GetAll()
                                           .AsNoTracking()
                                           .Select(s => new { s.UserGroupId, s.ItemId })
                                     join m in _userGroupMemberRepository.GetAll()
                                               .Where(s => s.MemberId == userId)
                                               .AsNoTracking()
                                               .Select(s => s.UserGroupId)
                                     on g.UserGroupId equals m
                                     into ms
                                     where ms.Count() > 0
                                     select g;

                var result = from i in itemQuery
                             join ig in itemGroupQuery
                             on i.Id equals ig.ItemId
                             into igs
                             where igs.Count() > 0 || i.Member == Member.All
                             select new ItemWithPropertySummaryOutput
                             {
                                 Id = i.Id,
                                 ItemCode = i.ItemCode,
                                 ItemName = i.ItemName,
                                 SalePrice = i.SalePrice,
                                 Properties = i.Properties
                             };

                return result;
            }
            else
            {
                var itemQuery = from i in _itemRepository.GetAll()
                                      .WhereIf(itemIds != null && itemIds.Count() > 0, s => itemIds.Contains(s.Id))
                                      .AsNoTracking()
                                      .Select(s => new
                                      {
                                          s.Id,
                                          s.ItemCode,
                                          s.ItemName,
                                          s.SalePrice
                                      })
                                join ip in _itemPropertyRepository.GetAll()
                                    .Include(s => s.Property)
                                    .Include(s => s.PropertyValue)
                                    .AsNoTracking()
                                    .Select(p => new ItemPropertySummary
                                    {
                                        Id = p.PropertyValueId ?? 0,
                                        PropertyId = p.PropertyId,
                                        PropertyName = p.Property == null ? "" : p.Property.Name,
                                        Value = p.PropertyValue == null ? "" : p.PropertyValue.Value,
                                        NetWeight = p.PropertyValue == null ? 0 : p.PropertyValue.NetWeight,
                                        IsUnit = p.Property != null && p.Property.IsUnit,
                                        ValueId = p.PropertyValueId,
                                        ItemId = p.ItemId,

                                    })
                                on i.Id equals ip.ItemId
                                into props

                                where propertyDics == null || propertyDics.Count == 0 ||
                                     (props.Where(i =>
                                         propertyDics.Any(v => v.PropertyId == i.PropertyId) &&
                                         propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                         (propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                             propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                             propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.Id))
                                      ).Count() == propertyDics.Count)

                                select new ItemWithPropertySummaryOutput
                                {
                                    Id = i.Id,
                                    ItemCode = i.ItemCode,
                                    ItemName = i.ItemName,
                                    SalePrice = i.SalePrice ?? 0,
                                    Properties = props.ToList()
                                };

                return itemQuery;
            }
        }

        protected IQueryable<InventoryPropertySummaryOutput> GetItemWithPropertiesQuery(
           string filter,
           List<Guid> itemIds,
           List<Guid?> inventoryAccountIds,
           List<long> inventoryAccountTypeIds,
           List<GetListPropertyFilter> propertyDics)
        {
            var accountQuery = GetAccountWithCode(inventoryAccountIds, inventoryAccountTypeIds);
            var itemPropertyQuery = GetItemProperties(itemIds);

            var itemQuery = from i in _itemRepository.GetAll()
                                      .WhereIf(itemIds != null && itemIds.Count() > 0, t => itemIds.Contains(t.Id))
                                      .WhereIf(inventoryAccountIds != null && inventoryAccountIds.Count() > 0, t => inventoryAccountIds.Contains(t.InventoryAccountId))
                                      .WhereIf(!filter.IsNullOrWhiteSpace(), p =>
                                            p.ItemName.ToLower().Contains(filter.ToLower()) ||
                                            p.ItemCode.ToLower().Contains(filter.ToLower())
                                      )
                                      .AsNoTracking()
                                      .Select(i => new
                                      {
                                          Id = i.Id,
                                          ItemCode = i.ItemCode,
                                          ItemName = i.ItemName,
                                          InventoryAccountId = i.InventoryAccountId,
                                          SalePrice = i.SalePrice ?? 0,
                                          Mix = i.Min,
                                          Max = i.Max
                                      })
                            join a in accountQuery
                            on i.InventoryAccountId equals a.Id
                            join ip in itemPropertyQuery
                            on i.Id equals ip.ItemId
                            into props

                            where propertyDics == null || propertyDics.Count == 0 ||
                                  (props.Where(i =>
                                      propertyDics.Any(v => v.PropertyId == i.PropertyId) &&
                                      propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId) != null &&
                                      (propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds == null ||
                                          propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Count == 0 ||
                                          propertyDics.FirstOrDefault(v => v.PropertyId == i.PropertyId).PropertyValueIds.Contains(i.Id))
                                   ).Count() == propertyDics.Count)

                            select new InventoryPropertySummaryOutput
                            {
                                ItemId = i.Id,
                                ItemCode = i.ItemCode,
                                ItemName = i.ItemName,
                                InventoryAccountId = i.InventoryAccountId,
                                InventoryAccountCode = a.AccountCode,
                                InventoryAccountName = a.AccountName,
                                SalePrice = i.SalePrice,
                                Properties = props.ToList(),
                                Min = i.Mix,
                                Max = i.Max
                            };

            return itemQuery;
        }


        #endregion



        protected string GetInnerHtml(string startTag, string endTag, string sourceHtml)
        {
            var startIndex = sourceHtml.IndexOf(startTag) + startTag.Length;
            var endIndex = sourceHtml.IndexOf(endTag, startIndex);

            if (startIndex < endIndex)
            {
                return sourceHtml.Substring(startIndex, endIndex - startIndex);
            }

            return "";
        }

        protected string GetOuterHtml(string startTag, string endTag, string sourceHtml)
        {
            var result = GetInnerHtml(startTag, endTag, sourceHtml);
            if (result != "") return startTag + result + endTag;

            return "";
        }


        protected KeyValuePair<string, int> FindTag(string source, string tag, string attr = "")
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(tag)) return new KeyValuePair<string, int>("", 0);

            var startIndex = source.IndexOf(tag);
            if (startIndex == -1) return new KeyValuePair<string, int>("", 0);

            var endIndex = source.IndexOf(">", startIndex);
            if (startIndex >= endIndex || endIndex == -1) return new KeyValuePair<string, int>("", 0);

            var startTag = source.Substring(startIndex, endIndex - startIndex + 1);

            if (!string.IsNullOrWhiteSpace(attr))
            {
                if (startTag.IndexOf(attr) == -1) return FindTag(source.Substring(endIndex + 1), tag, attr);
            }

            return new KeyValuePair<string, int>(startTag, startIndex);
        }

        protected KeyValuePair<string, int> FindTag(int startIndex, string source, string tag, string attr = "")
        {
            return FindTag(source.Substring(startIndex), tag, attr);
        }

        protected string EmbedBase64Fonts(string html)
        {
            //online => https://www.giftofspeed.com/base64-encoder/

            if (string.IsNullOrWhiteSpace(html)) return html;

            var khmerFont = GetFontBase64();
            if (khmerFont.IsNullOrEmpty()) return html;

            var utf8 = FindTag(html, "<meta", " charset=");
            if (utf8.Key != "")
            {
                html = html.Replace(utf8.Key, utf8.Key + $"\r\n\t{khmerFont}\r\n");
            }
            else
            {
                var head = FindTag(html, "<head");
                if (head.Key != "")
                {
                    html = html.Replace(utf8.Key, utf8.Key + $"<meta charset='utf-8' />\r\n\t{khmerFont}\r\n");
                }
            }

            return html;
        }

        protected string GetFontBase64()
        {
            var result = string.Empty;

            var _appFolders = IocManager.Instance.Resolve<AppFolders>();

            //only font.ttf file will apply here
            var fontList = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>( key: "Moul", value: "Moul-Regular.ttf"),
                new KeyValuePair<string, string>( key: "Khmer OS Muol Light", value: "Khmer-OS-Muol-Light-Regular.ttf"),
                new KeyValuePair<string, string>( key: "Battambang", value: "Battambang-Regular.ttf"),
                new KeyValuePair<string, string>( key: "Khmer OS Battambang", value: "KhmerOSBattambang-Regular.ttf"),
                new KeyValuePair<string, string>( key: "Khmer", value: "Khmer-Regular.ttf"),
                new KeyValuePair<string, string>( key: "Khmer OS", value: "KhmerOS.ttf")
            };

            foreach (var font in fontList)
            {
                var fontPath = Path.Combine(_appFolders.FontFolder, font.Value);

                var fontBase64 = string.Empty;
                if (File.Exists(fontPath))
                {
                    byte[] fontBytes = System.IO.File.ReadAllBytes(fontPath);
                    fontBase64 = "@font-face {font-family: '" + font.Key + "'; src: url(data:application/font-ttf;charset=utf-8;base64," + Convert.ToBase64String(fontBytes) + ") format('truetype'); }";
                    result += $"\r\n\t\t{fontBase64}";
                }
            }

            return "\r\n\t<style type='text/css'>" + result + "\r\n\t</style>";
        }

        protected string StreamToBase64(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream); // Copy stream content to memory stream
                byte[] byteArray = memoryStream.ToArray(); // Convert memory stream to byte array
                return Convert.ToBase64String(byteArray); // Convert byte array to Base64 string
            }
        }

        #region Sync Helper

        protected async Task DeleteInventoryTransactionItems(Guid id)
        {
            var items = await _inventoryTransactionItemRepository.GetAll()
                                .Where(s => s.TransactionId == id)
                                .AsNoTracking()
                                .ToListAsync();

            if (items.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(items);

        }
        protected async Task DeleteInventoryTransactionItems(List<Guid> ids)
        {
            var items = await _inventoryTransactionItemRepository.GetAll()
                                .Where(s => ids.Contains(s.TransactionId))
                                .AsNoTracking()
                                .ToListAsync();

            if (items.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(items);

        }


        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task SyncItemReceipt(Guid id)
        {
            var tenantId = AbpSession.GetTenantId();

            var inventoryTansactionOriginalList = new List<InventoryTransactionItem>();
            var inventoryTansactionCacheList = new List<InventoryTransactionItem>();

            inventoryTansactionCacheList = await _inventoryTransactionItemRepository.GetAll()
                                              .Where(s => s.TransactionId == id)
                                              .AsNoTracking()
                                              .ToListAsync();

            var itemReceiptItems = await (from ir in _itemReceiptItemRepository.GetAll()
                                                     .Where(s => s.ItemReceiptId == id)
                                                     .AsNoTracking()
                                          join j in _journalRepository.GetAll()
                                                    .Where(s => s.ItemReceiptId.HasValue)
                                                    .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                    .Where(s => s.ItemReceiptId == id)
                                                    .AsNoTracking()
                                          on ir.ItemReceiptId equals j.ItemReceiptId
                                          join ji in _journalItemRepository.GetAll()
                                                     .Where(s => s.Key == PostingKey.Inventory)
                                                     .AsNoTracking()
                                          on ir.Id equals ji.Identifier

                                          select InventoryTransactionItem.Create(
                                                      tenantId,
                                                      ir.CreatorUserId.Value,
                                                      ir.CreationTime,
                                                      ir.LastModifierUserId,
                                                      ir.LastModificationTime,
                                                      ir.Id,
                                                      ir.ItemReceiptId,
                                                      ir.ItemReceipt.TransferOrderId.HasValue ? ir.ItemReceipt.TransferOrderId : ir.ItemReceipt.ProductionOrderId,
                                                      ir.TransferOrderItemId.HasValue ? ir.TransferOrderItemId : ir.FinishItemId,
                                                      j.Id,
                                                      j.Date,
                                                      j.CreationTimeIndex.Value,
                                                      j.JournalType,
                                                      j.JournalNo,
                                                      j.Reference,
                                                      ir.ItemId,
                                                      ji.AccountId,
                                                      ir.Lot.LocationId,
                                                      ir.Lot.Id,
                                                      ir.Qty,
                                                      ir.UnitCost,
                                                      ir.Total,
                                                      true,
                                                      ir.Description)
                                      )
                                      .ToListAsync();


            inventoryTansactionOriginalList.AddRange(itemReceiptItems);

            var inventoryTansactionOriginalDic = inventoryTansactionOriginalList.ToDictionary(s => s.Id, s => s);
            var inventoryTansactionCacheDic = inventoryTansactionCacheList.ToDictionary(s => s.Id, s => s);

            var deleteInventoryTansactionList = inventoryTansactionCacheList.Where(s => !inventoryTansactionOriginalDic.ContainsKey(s.Id)).ToList();
            var createInventoryTansactionList = inventoryTansactionOriginalList.Where(s => !inventoryTansactionCacheDic.ContainsKey(s.Id)).ToList();
            var updateInventoryTansactionList = inventoryTansactionOriginalList.Where(s => inventoryTansactionCacheDic.ContainsKey(s.Id) &&
                                                                                        !s.IsSameAs(inventoryTansactionCacheDic[s.Id])).ToList();

            var syncProductions = deleteInventoryTansactionList
                              .Concat(createInventoryTansactionList)
                              .Concat(updateInventoryTansactionList)
                              .Where(s => s.JournalType == JournalType.ItemIssueProduction || s.JournalType == JournalType.ItemReceiptProduction)
                              .Where(s => s.TransferOrProductionId.HasValue)
                              .GroupBy(s => s.TransferOrProductionId.Value)
                              .Select(s => s.Key)
                              .ToHashSet();


            if (deleteInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(deleteInventoryTansactionList);
            if (createInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(createInventoryTansactionList);
            if (updateInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(updateInventoryTansactionList);

            if (syncProductions.Any())
            {
                var productions = await _productionRepository.GetAll()
                                 .Where(s => syncProductions.Contains(s.Id))
                                 .AsNoTracking()
                                 .ToListAsync();
                foreach (var p in productions)
                {
                    p.SetCalculateionState(CalculationState.Synced);
                }

                if (productions.Any()) await _productionRepository.BulkUpdateAsync(productions);
            }


        }

        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task SyncItemReceiptCustomerCredit(Guid id)
        {
            var tenantId = AbpSession.GetTenantId();

            var inventoryTansactionOriginalList = new List<InventoryTransactionItem>();
            var inventoryTansactionCacheList = new List<InventoryTransactionItem>();

            inventoryTansactionCacheList = await _inventoryTransactionItemRepository.GetAll()
                                              .Where(s => s.TransactionId == id)
                                              .AsNoTracking()
                                              .ToListAsync();

            inventoryTansactionOriginalList = await (from ir in _itemReceiptCustomerCreditItemRepository.GetAll()
                                                               .AsNoTracking()
                                                     join j in _journalRepository.GetAll()
                                                               .Where(s => s.ItemReceiptCustomerCreditId.HasValue)
                                                               .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                               .Where(s => s.ItemReceiptCustomerCreditId == id)
                                                               .AsNoTracking()
                                                     on ir.ItemReceiptCustomerCreditId equals j.ItemReceiptCustomerCreditId
                                                     join ji in _journalItemRepository.GetAll()
                                                                .Where(s => s.Key == PostingKey.Inventory)
                                                                .AsNoTracking()
                                                     on ir.Id equals ji.Identifier

                                                     select InventoryTransactionItem.Create(
                                                             tenantId,
                                                             ir.CreatorUserId.Value,
                                                             ir.CreationTime,
                                                             ir.LastModifierUserId,
                                                             ir.LastModificationTime,
                                                             ir.Id,
                                                             ir.ItemReceiptCustomerCreditId,
                                                             null,
                                                             null,
                                                             j.Id,
                                                             j.Date,
                                                             j.CreationTimeIndex.Value,
                                                             j.JournalType,
                                                             j.JournalNo,
                                                             j.Reference,
                                                             ir.ItemId,
                                                             ji.AccountId,
                                                             ir.Lot.LocationId,
                                                             ir.Lot.Id,
                                                             ir.Qty,
                                                             ir.UnitCost,
                                                             ir.Total,
                                                             true,
                                                             ir.Description)
                                                    )
                                                    .ToListAsync();


            var inventoryTansactionOriginalDic = inventoryTansactionOriginalList.ToDictionary(s => s.Id, s => s);
            var inventoryTansactionCacheDic = inventoryTansactionCacheList.ToDictionary(s => s.Id, s => s);

            var deleteInventoryTansactionList = inventoryTansactionCacheList.Where(s => !inventoryTansactionOriginalDic.ContainsKey(s.Id)).ToList();
            var createInventoryTansactionList = inventoryTansactionOriginalList.Where(s => !inventoryTansactionCacheDic.ContainsKey(s.Id)).ToList();
            var updateInventoryTansactionList = inventoryTansactionOriginalList.Where(s => inventoryTansactionCacheDic.ContainsKey(s.Id) &&
                                                                                        !s.IsSameAs(inventoryTansactionCacheDic[s.Id])).ToList();



            if (deleteInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(deleteInventoryTansactionList);
            if (createInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(createInventoryTansactionList);
            if (updateInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(updateInventoryTansactionList);


        }

        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task SyncItemIssue(Guid id)
        {
            var tenantId = AbpSession.GetTenantId();
            //var userId = AbpSession.GetUserId();

            var inventoryTansactionOriginalList = new List<InventoryTransactionItem>();
            var inventoryTansactionCacheList = new List<InventoryTransactionItem>();

            inventoryTansactionCacheList = await _inventoryTransactionItemRepository.GetAll()
                                              .Where(s => s.TransactionId == id)
                                              .AsNoTracking()
                                              .ToListAsync();

            inventoryTansactionOriginalList = await (from ir in _itemIssueItemRepository.GetAll()
                                               .AsNoTracking()
                                                     join j in _journalRepository.GetAll()
                                                               .Where(s => s.ItemIssueId.HasValue)
                                                               .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                               .Where(s => s.ItemIssueId == id)
                                                               .AsNoTracking()
                                                     on ir.ItemIssueId equals j.ItemIssueId
                                                     join ji in _journalItemRepository.GetAll()
                                                                .Where(s => s.Key == PostingKey.Inventory)
                                                                .AsNoTracking()
                                                     on ir.Id equals ji.Identifier

                                                     select InventoryTransactionItem.Create(
                                                         tenantId,
                                                         ir.CreatorUserId.Value,
                                                         ir.CreationTime,
                                                         ir.LastModifierUserId,
                                                         ir.LastModificationTime,
                                                         ir.Id,
                                                         ir.ItemIssueId,
                                                         ir.ItemIssue.TransferOrderId.HasValue ? ir.ItemIssue.TransferOrderId : ir.ItemIssue.ProductionOrderId,
                                                         ir.TransferOrderItemId.HasValue ? ir.TransferOrderItemId : ir.RawMaterialItemId,
                                                         j.Id,
                                                         j.Date,
                                                         j.CreationTimeIndex.Value,
                                                         j.JournalType,
                                                         j.JournalNo,
                                                         j.Reference,
                                                         ir.ItemId,
                                                         ji.AccountId,
                                                         ir.Lot.LocationId,
                                                         ir.Lot.Id,
                                                         -ir.Qty,
                                                         ir.UnitCost,
                                                         -ir.Total,
                                                         false,
                                                         ir.Description)
                                            ).ToListAsync();


            var inventoryTansactionOriginalDic = inventoryTansactionOriginalList.ToDictionary(s => s.Id, s => s);
            var inventoryTansactionCacheDic = inventoryTansactionCacheList.ToDictionary(s => s.Id, s => s);

            var deleteInventoryTansactionList = inventoryTansactionCacheList.Where(s => !inventoryTansactionOriginalDic.ContainsKey(s.Id)).ToList();
            var createInventoryTansactionList = inventoryTansactionOriginalList.Where(s => !inventoryTansactionCacheDic.ContainsKey(s.Id)).ToList();
            var updateInventoryTansactionList = inventoryTansactionOriginalList.Where(s => inventoryTansactionCacheDic.ContainsKey(s.Id) &&
                                                                                        !s.IsSameAs(inventoryTansactionCacheDic[s.Id])).ToList();
            var syncProductions = deleteInventoryTansactionList
                             .Concat(createInventoryTansactionList)
                             .Concat(updateInventoryTansactionList)
                             .Where(s => s.JournalType == JournalType.ItemIssueProduction || s.JournalType == JournalType.ItemReceiptProduction)
                             .Where(s => s.TransferOrProductionId.HasValue)
                             .GroupBy(s => s.TransferOrProductionId.Value)
                             .Select(s => s.Key)
                             .ToHashSet();


            if (deleteInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(deleteInventoryTansactionList);
            if (createInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(createInventoryTansactionList);
            if (updateInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(updateInventoryTansactionList);

            if (syncProductions.Any())
            {
                var productions = await _productionRepository.GetAll()
                                 .Where(s => syncProductions.Contains(s.Id))
                                 .AsNoTracking()
                                 .ToListAsync();
                foreach (var p in productions)
                {
                    p.SetCalculateionState(CalculationState.Synced);
                }

                if (productions.Any()) await _productionRepository.BulkUpdateAsync(productions);
            }


        }

        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task SyncItemIssueVendorCredit(Guid id)
        {
            var tenantId = AbpSession.GetTenantId();
            //var userId = AbpSession.GetUserId();

            var inventoryTansactionOriginalList = new List<InventoryTransactionItem>();
            var inventoryTansactionCacheList = new List<InventoryTransactionItem>();


            inventoryTansactionCacheList = await _inventoryTransactionItemRepository.GetAll()
                                              .Where(s => s.TransactionId == id)
                                              .AsNoTracking()
                                              .ToListAsync();



            inventoryTansactionOriginalList = await (from ir in _itemIssueVendorCreditItemRepository.GetAll()
                                                           .AsNoTracking()
                                                     join j in _journalRepository.GetAll()
                                                               .Where(s => s.ItemIssueVendorCreditId.HasValue)
                                                               .Where(s => s.Status == enumStatus.EnumStatus.TransactionStatus.Publish)
                                                               .Where(s => s.ItemIssueVendorCreditId == id)
                                                               .AsNoTracking()
                                                     on ir.ItemIssueVendorCreditId equals j.ItemIssueVendorCreditId
                                                     join ji in _journalItemRepository.GetAll()
                                                                .Where(s => s.Key == PostingKey.Inventory)
                                                                .AsNoTracking()
                                                     on ir.Id equals ji.Identifier
                                                     select InventoryTransactionItem.Create(
                                                         tenantId,
                                                         ir.CreatorUserId.Value,
                                                         ir.CreationTime,
                                                         ir.LastModifierUserId,
                                                         ir.LastModificationTime,
                                                         ir.Id,
                                                         ir.ItemIssueVendorCreditId,
                                                         null,
                                                         null,
                                                         j.Id,
                                                         j.Date,
                                                         j.CreationTimeIndex.Value,
                                                         j.JournalType,
                                                         j.JournalNo,
                                                         j.Reference,
                                                         ir.ItemId,
                                                         ji.AccountId,
                                                         ir.Lot.LocationId,
                                                         ir.Lot.Id,
                                                         -ir.Qty,
                                                         ir.UnitCost,
                                                         -ir.Total,
                                                         false,
                                                         ir.Description)
                                                )
                                                .ToListAsync();


            var inventoryTansactionOriginalDic = inventoryTansactionOriginalList.ToDictionary(s => s.Id, s => s);
            var inventoryTansactionCacheDic = inventoryTansactionCacheList.ToDictionary(s => s.Id, s => s);

            var deleteInventoryTansactionList = inventoryTansactionCacheList.Where(s => !inventoryTansactionOriginalDic.ContainsKey(s.Id)).ToList();
            var createInventoryTansactionList = inventoryTansactionOriginalList.Where(s => !inventoryTansactionCacheDic.ContainsKey(s.Id)).ToList();
            var updateInventoryTansactionList = inventoryTansactionOriginalList.Where(s => inventoryTansactionCacheDic.ContainsKey(s.Id) &&
                                                                                        !s.IsSameAs(inventoryTansactionCacheDic[s.Id])).ToList();


            if (deleteInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkDeleteAsync(deleteInventoryTansactionList);
            if (createInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkInsertAsync(createInventoryTansactionList);
            if (updateInventoryTansactionList.Any()) await _inventoryTransactionItemRepository.BulkUpdateAsync(updateInventoryTansactionList);


        }

        #endregion

        #region BatchNo Helper
        /// <summary>
        /// Required SaveChanged Before call
        /// </summary>
        /// <param name="itemReceiptId"></param>
        /// <param name="batchNoIds"></param>
        /// <returns></returns>
        protected async Task<List<Guid>> GetBatchNoUseByOthers(Guid itemReceiptId, List<Guid> batchNoIds)
        {
            var batchUseByOtherReceipt = _itemReceiptItemBatchNoRepository.GetAll()
                                       .Where(s => s.ItemReceiptItem.ItemReceiptId != itemReceiptId && batchNoIds.Contains(s.BatchNoId))
                                       .AsNoTracking()
                                       .Select(s => s.BatchNoId);

            var batchUseByItemIssue = _itemIssueItemBatchNoRepository.GetAll()
                                        .Where(s => batchNoIds.Contains(s.BatchNoId))
                                        .AsNoTracking()
                                        .Select(s => s.BatchNoId);

            var batchUseByItemIssueVendorCredit = _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                                  .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                  .AsNoTracking()
                                                  .Select(s => s.BatchNoId);

            var batchUseByItemReceiptCustomerCredit = _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                    .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                    .AsNoTracking()
                                                    .Select(s => s.BatchNoId);

            var batchUseByVendorCredit = _vendorCreditItemBatchNoRepository.GetAll()
                                                  .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                  .AsNoTracking()
                                                  .Select(s => s.BatchNoId);

            var batchUseByCustomerCredit = _customerCreditItemBatchNoRepository.GetAll()
                                                    .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                    .AsNoTracking()
                                                    .Select(s => s.BatchNoId);

            var btchUseByClose = _invnetoryCostCloseItemBatchNoRepository.GetAll()
                                 .Where(s => batchNoIds.Contains(s.BatchNoId))
                                 .AsNoTracking()
                                 .Select(s => s.BatchNoId);

            var allBatchUse = batchUseByOtherReceipt
                              .Union(batchUseByItemIssue)
                              .Union(batchUseByItemIssueVendorCredit)
                              .Union(batchUseByItemReceiptCustomerCredit)
                              .Union(batchUseByCustomerCredit)
                              .Union(batchUseByVendorCredit)
                              .Union(btchUseByClose);

            var result = await allBatchUse.ToListAsync();

            return result;
        }

        protected async Task<List<Guid>> GetBatchNoUseByOtherForDelete(List<Guid> itemReceiptIds, List<Guid> batchNoIds)
        {
            var batchUseByOtherReceipt = _itemReceiptItemBatchNoRepository.GetAll()
                                       .Where(s => !itemReceiptIds.Contains( s.ItemReceiptItem.ItemReceiptId)  && batchNoIds.Contains(s.BatchNoId))
                                       .AsNoTracking()
                                       .Select(s => s.BatchNoId);

            var batchUseByItemIssue = _itemIssueItemBatchNoRepository.GetAll()
                                        .Where(s => batchNoIds.Contains(s.BatchNoId))
                                        .AsNoTracking()
                                        .Select(s => s.BatchNoId);

            var batchUseByItemIssueVendorCredit = _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                                  .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                  .AsNoTracking()
                                                  .Select(s => s.BatchNoId);

            var batchUseByItemReceiptCustomerCredit = _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                                    .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                    .AsNoTracking()
                                                    .Select(s => s.BatchNoId);

            var batchUseByVendorCredit = _vendorCreditItemBatchNoRepository.GetAll()
                                                  .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                  .AsNoTracking()
                                                  .Select(s => s.BatchNoId);

            var batchUseByCustomerCredit = _customerCreditItemBatchNoRepository.GetAll()
                                                    .Where(s => batchNoIds.Contains(s.BatchNoId))
                                                    .AsNoTracking()
                                                    .Select(s => s.BatchNoId);

            var btchUseByClose = _invnetoryCostCloseItemBatchNoRepository.GetAll()
                                 .Where(s => batchNoIds.Contains(s.BatchNoId))
                                 .AsNoTracking()
                                 .Select(s => s.BatchNoId);

            var allBatchUse = batchUseByOtherReceipt
                              .Union(batchUseByItemIssue)
                              .Union(batchUseByItemIssueVendorCredit)
                              .Union(batchUseByItemReceiptCustomerCredit)
                              .Union(batchUseByCustomerCredit)
                              .Union(batchUseByVendorCredit)
                              .Union(btchUseByClose);

            var result = await allBatchUse.ToListAsync();

            return result;
        }

        protected async Task UpdateBatchNoBalance(List<Guid> batchNoIds) 
        {
            var batchNos = await _batchNoRepository.GetAll().AsNoTracking().Where(s => batchNoIds.Contains(s.Id)).ToListAsync();
            if (!batchNoIds.Any()) return;

            var receiptDic = await _itemReceiptItemBatchNoRepository.GetAll()
                                   .Where(s => batchNoIds.Contains(s.BatchNoId))
                                   .AsNoTracking()
                                   .GroupBy(s => s.BatchNoId)
                                   .Select(s => new { s.Key, Qty = s.Sum(t => t.Qty) })
                                   .ToDictionaryAsync(k => k.Key, v => v.Qty);

            var issueDic = await _itemIssueItemBatchNoRepository.GetAll()
                                 .Where(s => batchNoIds.Contains(s.BatchNoId))
                                 .AsNoTracking()
                                 .GroupBy(s => s.BatchNoId)
                                 .Select(s => new { s.Key, Qty = s.Sum(t => t.Qty) })
                                 .ToDictionaryAsync(k => k.Key, v => v.Qty);

            var purchaseReturnDic = await _itemIssueVendorCreditItemBatchNoRepository.GetAll()
                                          .Where(s => batchNoIds.Contains(s.BatchNoId))
                                          .AsNoTracking()
                                          .GroupBy(s => s.BatchNoId)
                                          .Select(s => new { s.Key, Qty = s.Sum(t => t.Qty) })
                                          .ToDictionaryAsync(k => k.Key, v => v.Qty);

            var saleReturnDic = await _itemReceiptCustomerCreditItemBatchNoRepository.GetAll()
                                      .Where(s => batchNoIds.Contains(s.BatchNoId))
                                      .AsNoTracking()
                                      .GroupBy(s => s.BatchNoId)
                                      .Select(s => new { s.Key, Qty = s.Sum(t => t.Qty) })
                                      .ToDictionaryAsync(k => k.Key, v => v.Qty);

            var updateBatchs = batchNos.Select(s =>
            {
                var receiptQty = receiptDic.ContainsKey(s.Id) ? receiptDic[s.Id] : 0;
                var issueQty = issueDic.ContainsKey(s.Id) ? issueDic[s.Id] : 0;
                var purchaseReturnQty = purchaseReturnDic.ContainsKey(s.Id) ? purchaseReturnDic[s.Id] : 0;
                var saleReturnQty = saleReturnDic.ContainsKey(s.Id) ? saleReturnDic[s.Id] : 0;
                s.SetBalance(receiptQty + saleReturnQty, issueQty + purchaseReturnQty);

                return s;
            })
            .ToList();

            await _batchNoRepository.BulkUpdateAsync(updateBatchs);
        }
        #endregion


        #region Turn of filter if tenant is Debug

        protected async Task TurnOffTenantFilterIfDebug() {
            var tenant = await GetCurrentTenantAsync();
            if (tenant.IsDebug)
            {
                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant);
                CurrentUnitOfWork.DisableFilter(AbpDataFilters.MustHaveTenant);
            }
        }

        #endregion

    }
}