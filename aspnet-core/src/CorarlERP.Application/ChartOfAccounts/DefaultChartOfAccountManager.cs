using System;
using System.Collections.Generic;
using System.Text;
using CorarlERP.EntityFrameworkCore;
using CorarlERP.ChartOfAccounts;
using CorarlERP.ChartOfAccounts.Dto;
using System.Linq;
using CorarlERP.AccountTypes;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using CorarlERP.MultiTenancy;
using CorarlERP.Taxes;
using System.Transactions;
using CorarlERP.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using CorarlERP.Locations;
using CorarlERP.Lots;
using CorarlERP.Classes;
using CorarlERP.PaymentMethods;
using CorarlERP.AccountCycles;
using CorarlERP.TransactionTypes;
using CorarlERP.CustomerTypes;
using CorarlERP.Customers;
using CorarlERP.Addresses;
using Abp.Domain.Entities;
using CorarlERP.Vendors;
using CorarlERP.VendorTypes;
using CorarlERP.PropertyValues;
using CorarlERP.Features;
using Abp.Application.Features;
using CorarlERP.InventoryTransactionTypes;

namespace CorarlERP.Migrations.Seed.Tenants
{
    public class DefaultChartOfAccountManager : CorarlERPDomainServiceBase, IDefaultChartOfAccountManager
    {

        private readonly ICorarlRepository<Tenant, int> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICorarlRepository<Tax, long> _taxRepository;
        private readonly ICorarlRepository<Location, long> _locationRepository;
        private readonly ICorarlRepository<Lot, long> _lotRepository;
        private readonly ICorarlRepository<Class, long> _classRepository;
        private readonly ICorarlRepository<ChartOfAccount, Guid> _chartOfAccountRepository;
        private readonly ICorarlRepository<AccountType, long> _accountTypeRepository;
        private readonly ICorarlRepository<User, long> _userRepository;
        private readonly ICorarlRepository<TransactionType, long> _saleTypeRepository;
        private readonly ICorarlRepository<CustomerType, long> _customerTypeRepository;
        private readonly ICorarlRepository<Customer, Guid> _customerRepository;
        private readonly ICorarlRepository<VendorType, long> _vendorTypeRepository;
        private readonly ICorarlRepository<Vendor, Guid> _vendorRepository;
        private readonly ICorarlRepository<PaymentMethodBase, Guid> _paymentMethodBaseRepository;
        private readonly ICorarlRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IAccountCycleManager _accountCycleManager;
        private readonly ICorarlRepository<Property, long> _propertyRepository;
        private readonly ICorarlRepository<PropertyValue, long> _propertyValueRepository;
        private readonly IFeatureChecker _featureChecker;
      
        public DefaultChartOfAccountManager(
            IUnitOfWorkManager unitOfWorkManager,
            ICorarlRepository<Tenant, int> tenantRepository,
            ICorarlRepository<ChartOfAccount, Guid> chartOfAccountRepository,
            ICorarlRepository<User, long> userRepository,
            ICorarlRepository<AccountType, long> accountTypeRepository,
            ICorarlRepository<Tax, long> taxRepository,
            ICorarlRepository<Location, long> locationRepository,
            ICorarlRepository<Lot, long> lotRepository,
            ICorarlRepository<Class, long> classRepository,
            ICorarlRepository<TransactionType, long> saleTypeRepository,
            ICorarlRepository<CustomerType, long> customerTypeRepository,
            ICorarlRepository<Customer, Guid> customerRepository,
            ICorarlRepository<VendorType, long> vendorTypeRepository,
            ICorarlRepository<Vendor, Guid> vendorRepository,
            IAccountCycleManager accountCycleManager,
            ICorarlRepository<PaymentMethodBase, Guid> paymentMethodBaseRepository,
            ICorarlRepository<Property, long> propertyRepository,
            ICorarlRepository<PaymentMethod, Guid> paymentMethodRepository,
            ICorarlRepository<PropertyValue, long> propertyValueRepository,
            IFeatureChecker featureChecker)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _tenantRepository = tenantRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
            _userRepository = userRepository;
            _accountTypeRepository = accountTypeRepository;
            _locationRepository = locationRepository;
            _classRepository = classRepository;
            _lotRepository = lotRepository;
            _taxRepository = taxRepository;
            _paymentMethodBaseRepository = paymentMethodBaseRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountCycleManager = accountCycleManager;
            _saleTypeRepository = saleTypeRepository;
            _customerTypeRepository = customerTypeRepository;
            _customerRepository = customerRepository;
            _vendorTypeRepository = vendorTypeRepository;
            _vendorRepository = vendorRepository;
            _propertyRepository = propertyRepository;
            _propertyValueRepository = propertyValueRepository;
            _featureChecker = featureChecker;
           
        }


        public async Task CreateDefaultChartAccounts(int tenantId)
        {
            Dictionary<string, long> accountTypeDic = new Dictionary<string, long>();
            long userId = 0;

            Tax defaultTax = null;
            Location defaultLocation = null;
            Lot defaultLot = null;
            Class defaultClass = null;
            var defaultAccounts = new List<ChartOfAccount>();
            Tenant tenant = null;

            var paymentMethodBases = new List<PaymentMethodBase>();
            var paymentMethods = new List<PaymentMethod>();

            Customer defaultCustomer = null;
            Vendor defaultVendor = null;


            tenant = await _tenantRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.Id == tenantId);
            var hasClassFeature = _featureChecker.IsEnabled(tenantId, AppFeatures.SetupFeatureClasss);
            if (!tenant.UseDefaultAccount) return;

            //Default Class
            defaultClass = await _classRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultClass == null)
            {
                defaultClass = Class.Create(tenantId, userId, "Default", false, null);
                defaultClass.Id = await _classRepository.InsertAndGetIdAsync(defaultClass);
            }

            defaultAccounts = await _chartOfAccountRepository.GetAll().AsNoTracking().ToListAsync();
            if (defaultAccounts.Any()) return;

            var adminUser = await _userRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (adminUser == null) throw new UserFriendlyException("UserNotExist");

            userId = adminUser.Id;

            //Default Tax
            defaultTax = await _taxRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultTax == null)
            {
                defaultTax = Tax.Create(tenantId, userId, "No Tax", 0);
                defaultTax.Id = await _taxRepository.InsertAndGetIdAsync(defaultTax);
            }
            //Default Property
            var isProperty = await _propertyRepository.GetAll().AsNoTracking().AnyAsync();
            if (!isProperty)
            {
                var pentity = Property.Create(tenantId, userId, "Unit", true, false, true, false, false);
                await _propertyRepository.InsertAsync(pentity);
                var @vEntity = PropertyValue.Create(tenantId, userId, pentity, "Unit", 0, true, "000", true, null, 0);
                await _propertyValueRepository.InsertAsync(@vEntity);

                var pentityItemGroup = Property.Create(tenantId, userId, "Item Group", false, false, true, true, false);
                await _propertyRepository.InsertAsync(pentityItemGroup);
                var @ItemGroupEntity = PropertyValue.Create(tenantId, userId, pentityItemGroup, "Item Group", 0, true, "000", false, null, 0);
                await _propertyValueRepository.InsertAsync(@ItemGroupEntity);

            }
            //Default Location
            defaultLocation = await _locationRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultLocation == null)
            {
                defaultLocation = Location.Create(tenantId, userId, "Default", false, enumStatus.EnumStatus.Member.All, null, "");
                defaultLocation.Id = await _locationRepository.InsertAndGetIdAsync(defaultLocation);
            }

            //Default Lot
            defaultLot = await _lotRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultLot == null)
            {
                defaultLot = Lot.Create(tenantId, userId, "Default", defaultLocation.Id);
                defaultLot.Id = await _lotRepository.InsertAndGetIdAsync(defaultLot);
            }



            paymentMethodBases = await _paymentMethodBaseRepository.GetAll().Where(s => s.IsActive).AsNoTracking().ToListAsync();
            paymentMethods = await _paymentMethodRepository.GetAll().AsNoTracking().ToListAsync();

            accountTypeDic = await _accountTypeRepository.GetAll().AsNoTracking().ToDictionaryAsync(k => k.AccountTypeName.Trim(), v => v.Id);

            //Default Sale Type
            var saleType = await _saleTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (saleType == null)
            {
                var defaultSaleType = TransactionType.Create(tenantId, userId, "POS");
                defaultSaleType.SetIsPOS(true);
                await _saleTypeRepository.InsertAsync(defaultSaleType);
            }

            //Default Customer Types
            var defaultCustomerType = await _customerTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultCustomerType == null)
            {
                var wholeseller = CustomerType.Create(tenantId, userId, "Wholesaler");
                defaultCustomerType = CustomerType.Create(tenantId, userId, "Retailer");
                await _customerTypeRepository.BulkInsertAsync(new List<CustomerType> { wholeseller, defaultCustomerType });
            }

            //Default Customer
            defaultCustomer = await _customerRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultCustomer == null)
            {
                defaultCustomer = Customer.Create(
                    tenantId,
                    userId,
                    "C12-000001",
                    "General Customer",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    new CAddress(String.Empty, "KH", String.Empty, String.Empty, String.Empty),
                    new CAddress(String.Empty, "KH", String.Empty, String.Empty, String.Empty),
                    true,
                    String.Empty,
                    null,
                    defaultCustomerType.Id,
                true);

                defaultCustomer.UpdateMember(enumStatus.EnumStatus.Member.All);
            }

            //Default Vendor Type
            var defaultVendorType = await _vendorTypeRepository.GetAll().AsNoTracking().FirstOrDefaultAsync(s => s.VendorTypeName == "Supplier" || s.VendorTypeName == "Vendor");
            if (defaultVendorType == null)
            {
                defaultVendorType = VendorType.Create(tenantId, userId, "Supplier");
                defaultVendorType.Id = await _vendorTypeRepository.InsertAndGetIdAsync(defaultVendorType);
            }

            //Default Vendor
            defaultVendor = await _vendorRepository.GetAll().AsNoTracking().FirstOrDefaultAsync();
            if (defaultVendor == null)
            {
                defaultVendor = Vendor.Create(
                    tenantId,
                    userId,
                    "V20-000001",
                    "General Supplier",
                    String.Empty,
                    String.Empty,
                    String.Empty,
                    new CAddress(String.Empty, "KH", String.Empty, String.Empty, String.Empty),
                    new CAddress(String.Empty, "KH", String.Empty, String.Empty, String.Empty),
                    true,
                    String.Empty,
                    null,
                    defaultVendorType.Id);

                defaultVendor.UpdateMember(enumStatus.EnumStatus.Member.All);
            }


            ChartOfAccount cashAccount = null;
            ChartOfAccount bankAccount = null;
            ChartOfAccount arAccount = null;
            ChartOfAccount inventoryAccount = null;
            ChartOfAccount bankTransferAccount = null;
            ChartOfAccount goodsInTransitAccount = null;
            ChartOfAccount workInProgressAccount = null;
            ChartOfAccount apAccount = null;
            ChartOfAccount equityAccount = null;
            ChartOfAccount saleRevenueAccount = null;
            ChartOfAccount saleReturnAccount = null;
            ChartOfAccount cogsAccount = null;
            ChartOfAccount stockAdjustmentAccount = null;
            ChartOfAccount expenseAccount = null;
            ChartOfAccount exchangeLossGainAccount = null;

            if (accountTypeDic.ContainsKey(AccountTypeEnums.Cash.GetName()))
            {
                defaultAccounts.Add(cashAccount = ChartOfAccount.Create(tenantId, userId, "10-0000", "Cash", "", accountTypeDic[AccountTypeEnums.Cash.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.Bank.GetName()))
            {
                defaultAccounts.Add(bankAccount = ChartOfAccount.Create(tenantId, userId, "11-0000", "Bank", "", accountTypeDic[AccountTypeEnums.Bank.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.AR.GetName()))
            {
                defaultAccounts.Add(arAccount = ChartOfAccount.Create(tenantId, userId, "12-0000", "Account Receivable", "", accountTypeDic[AccountTypeEnums.AR.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.Inventory.GetName()))
            {
                defaultAccounts.Add(inventoryAccount = ChartOfAccount.Create(tenantId, userId, "13-0000", "Inventory", "", accountTypeDic[AccountTypeEnums.Inventory.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.OtherCurrentAssets.GetName()))
            {
                var typeId = accountTypeDic[AccountTypeEnums.OtherCurrentAssets.GetName()];
                defaultAccounts.Add(bankTransferAccount = ChartOfAccount.Create(tenantId, userId, "14-0000", "Bank Trasfer", "", typeId, null, defaultTax.Id));
                defaultAccounts.Add(goodsInTransitAccount = ChartOfAccount.Create(tenantId, userId, "14-0001", "Good in Transit", "", typeId, null, defaultTax.Id));
                defaultAccounts.Add(workInProgressAccount = ChartOfAccount.Create(tenantId, userId, "14-0002", "Work in Progress", "", typeId, null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.AP.GetName()))
            {
                defaultAccounts.Add(apAccount = ChartOfAccount.Create(tenantId, userId, "20-0000", "Account Payable", "", accountTypeDic[AccountTypeEnums.AP.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.Equity.GetName()))
            {
                defaultAccounts.Add(equityAccount = ChartOfAccount.Create(tenantId, userId, "30-0000", "Equity", "", accountTypeDic[AccountTypeEnums.Equity.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.Revenue.GetName()))
            {
                var typeId = accountTypeDic[AccountTypeEnums.Revenue.GetName()];
                defaultAccounts.Add(saleRevenueAccount = ChartOfAccount.Create(tenantId, userId, "40-0000", "Sale Revenue", "", typeId, null, defaultTax.Id));
                defaultAccounts.Add(saleReturnAccount = ChartOfAccount.Create(tenantId, userId, "40-0001", "Sale Return", "", typeId, null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.COGS.GetName()))
            {
                var typeId = accountTypeDic[AccountTypeEnums.COGS.GetName()];
                defaultAccounts.Add(cogsAccount = ChartOfAccount.Create(tenantId, userId, "50-0000", "Cost of Goods Sold", "", typeId, null, defaultTax.Id));
                defaultAccounts.Add(stockAdjustmentAccount = ChartOfAccount.Create(tenantId, userId, "50-0001", "Stock Adjustment", "", typeId, null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.Expense.GetName()))
            {
                defaultAccounts.Add(expenseAccount = ChartOfAccount.Create(tenantId, userId, "60-0000", "Expense", "", accountTypeDic[AccountTypeEnums.Expense.GetName()], null, defaultTax.Id));
            }
            if (accountTypeDic.ContainsKey(AccountTypeEnums.OtherExpense.GetName()))
            {
                defaultAccounts.Add(exchangeLossGainAccount = ChartOfAccount.Create(tenantId, userId, "61-0000", "Exchange Loss/Gain", "", accountTypeDic[AccountTypeEnums.OtherExpense.GetName()], null, defaultTax.Id));
            }

            if (!defaultAccounts.Any()) return;

            var addPaymentMethods = new List<PaymentMethod>();
            if (paymentMethodBases.Any() && !paymentMethods.Any())
            {
                foreach (var p in paymentMethodBases)
                {
                    var accountId = p.Name == "Bank" ? bankAccount.Id : cashAccount.Id;
                    addPaymentMethods.Add(PaymentMethod.Create(tenantId, userId, p.Id, accountId));
                }
            }

            await _chartOfAccountRepository.BulkInsertAsync(defaultAccounts);

            if (!tenant.AccountCycleId.HasValue)
            {
                var date = new DateTime(Abp.Timing.Clock.Now.Year, 1, 1, 0, 0, 0);
                tenant.AccountCycle = AccountCycle.Create(tenantId, userId, date, null, 2, 2);
                await _accountCycleManager.CreateAsync(tenant.AccountCycle);
            }
            var classId = defaultClass != null ? defaultClass.Id : (long?)null;
            tenant.UpdateDetault(
                defaultLocation.Id,
                classId,
                goodsInTransitAccount.Id,
                saleReturnAccount.Id,
                cashAccount.Id,
                goodsInTransitAccount.Id,
                goodsInTransitAccount.Id,
                stockAdjustmentAccount.Id,
                goodsInTransitAccount.Id,
                stockAdjustmentAccount.Id,
                saleReturnAccount.Id,
                goodsInTransitAccount.Id,
                stockAdjustmentAccount.Id,
                goodsInTransitAccount.Id,
                stockAdjustmentAccount.Id,
                bankTransferAccount.Id,
                workInProgressAccount.Id,
                workInProgressAccount.Id,
                stockAdjustmentAccount.Id,
                apAccount.Id,
                arAccount.Id,
                exchangeLossGainAccount.Id,
                inventoryAccount.Id,
                cogsAccount.Id,
                saleRevenueAccount.Id,
                expenseAccount.Id,
                defaultTax.Id);

            //Default POS Currency
            tenant.SetPOSCurrency(tenant.CurrencyId);

            await _tenantRepository.BulkUpdateAsync(new List<Tenant> { tenant });

            if (addPaymentMethods.Any()) await _paymentMethodRepository.BulkInsertAsync(addPaymentMethods);

            if (defaultCustomer != null)
            {
                defaultCustomer.SetAccount(arAccount.Id);
                await _customerRepository.InsertAsync(defaultCustomer);
            }

            if (defaultVendor != null)
            {
                defaultVendor.SetAccount(apAccount.Id);
                await _vendorRepository.InsertAsync(defaultVendor);

            }

        }

    }
}
