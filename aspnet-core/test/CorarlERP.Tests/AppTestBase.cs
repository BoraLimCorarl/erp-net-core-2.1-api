using System;
using System.Linq;
using System.Threading.Tasks;
using Abp;
using Abp.EntityFrameworkCore.Extensions;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Session;
using Abp.TestBase;
using Microsoft.EntityFrameworkCore;
using CorarlERP.Authorization.Roles;
using CorarlERP.Authorization.Users;
using CorarlERP.EntityFrameworkCore;
using CorarlERP.MultiTenancy;
using CorarlERP.Tests.TestDatas;
using CorarlERP.Taxes;
using CorarlERP.ChartOfAccounts;
using Shouldly;
using CorarlERP.Currencies;
using CorarlERP.PropertyValues;
using CorarlERP.ItemTypes;
using CorarlERP.Vendors;
using CorarlERP.Addresses;
using CorarlERP.PurchaseOrders;
using CorarlERP.Items;
using CorarlERP.Formats;
using CorarlERP.AccountCycles;
using CorarlERP.Classes;
using CorarlERP.Locations;
using static CorarlERP.PurchaseOrders.PurchaseOrder;
using CorarlERP.ItemReceipts;
using static CorarlERP.ItemReceipts.ItemReceipt;
using CorarlERP.Journals;
using static CorarlERP.enumStatus.EnumStatus;
using CorarlERP.Customers;
using CorarlERP.SaleOrders;
using CorarlERP.Invoices;
using CorarlERP.ItemIssues;
using CorarlERP.CustomerCredits;
using CorarlERP.VendorTypes;

namespace CorarlERP.Tests
{
    /// <summary>
    /// This is base class for all our test classes.
    /// It prepares ABP system, modules and a fake, in-memory database.
    /// Seeds database with initial data.
    /// Provides methods to easily work with <see cref="CorarlERPDbContext"/>.
    /// </summary>
    public abstract class AppTestBase : AbpIntegratedTestBase<CorarlERPTestModule>
    {
        protected AppTestBase()
        {
            SeedTestData();
            LoginAsDefaultTenantAdmin();
        }

        private void SeedTestData()
        {
            void NormalizeDbContext(CorarlERPDbContext context)
            {
                context.EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
                context.EventBus = NullEventBus.Instance;
                context.SuppressAutoSetTenantId = true;
            }

            //Seed initial data for default tenant
            AbpSession.TenantId = 1;
            UsingDbContext(context =>
            {
                NormalizeDbContext(context);
                new TestDataBuilder(context, 1).Create();
            });
        }

        protected IDisposable UsingTenantId(int? tenantId)
        {
            var previousTenantId = AbpSession.TenantId;
            AbpSession.TenantId = tenantId;
            return new DisposeAction(() => AbpSession.TenantId = previousTenantId);
        }

        protected void UsingDbContext(Action<CorarlERPDbContext> action)
        {
            UsingDbContext(AbpSession.TenantId, action);
        }

        protected Task UsingDbContextAsync(Func<CorarlERPDbContext, Task> action)
        {
            return UsingDbContextAsync(AbpSession.TenantId, action);
        }

        protected T UsingDbContext<T>(Func<CorarlERPDbContext, T> func)
        {
            return UsingDbContext(AbpSession.TenantId, func);
        }

        protected Task<T> UsingDbContextAsync<T>(Func<CorarlERPDbContext, Task<T>> func)
        {
            return UsingDbContextAsync(AbpSession.TenantId, func);
        }

        protected void UsingDbContext(int? tenantId, Action<CorarlERPDbContext> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<CorarlERPDbContext>())
                {
                    action(context);
                    context.SaveChanges();
                }
            }
        }

        protected async Task UsingDbContextAsync(int? tenantId, Func<CorarlERPDbContext, Task> action)
        {
            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<CorarlERPDbContext>())
                {
                    await action(context);
                    await context.SaveChangesAsync();
                }
            }
        }

        protected T UsingDbContext<T>(int? tenantId, Func<CorarlERPDbContext, T> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<CorarlERPDbContext>())
                {
                    result = func(context);
                    context.SaveChanges();
                }
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(int? tenantId, Func<CorarlERPDbContext, Task<T>> func)
        {
            T result;

            using (UsingTenantId(tenantId))
            {
                using (var context = LocalIocManager.Resolve<CorarlERPDbContext>())
                {
                    result = await func(context);
                    await context.SaveChangesAsync();
                }
            }

            return result;
        }

        #region Login

        protected void LoginAsHostAdmin()
        {
            LoginAsHost(User.AdminUserName);
        }

        protected void LoginAsDefaultTenantAdmin()
        {
            LoginAsTenant(Tenant.DefaultTenantName, User.AdminUserName);
        }

        protected void LoginAsHost(string userName)
        {
            AbpSession.TenantId = null;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for host.");
            }

            AbpSession.UserId = user.Id;
        }

        protected void LoginAsTenant(string tenancyName, string userName)
        {
            AbpSession.TenantId = null;

            var tenant = UsingDbContext(context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
            if (tenant == null)
            {
                throw new Exception("There is no tenant: " + tenancyName);
            }

            AbpSession.TenantId = tenant.Id;

            var user = UsingDbContext(context => context.Users.FirstOrDefault(u => u.TenantId == AbpSession.TenantId && u.UserName == userName));
            if (user == null)
            {
                throw new Exception("There is no user: " + userName + " for tenant: " + tenancyName);
            }

            AbpSession.UserId = user.Id;
        }

        #endregion

        #region GetCurrentUser

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected User GetCurrentUser()
        {
            var userId = AbpSession.GetUserId();
            return UsingDbContext(context => context.Users.Single(u => u.Id == userId));
        }

        /// <summary>
        /// Gets current user if <see cref="IAbpSession.UserId"/> is not null.
        /// Throws exception if it's null.
        /// </summary>
        protected async Task<User> GetCurrentUserAsync()
        {
            var userId = AbpSession.GetUserId();
            return await UsingDbContext(context => context.Users.SingleAsync(u => u.Id == userId));
        }

        #endregion

        #region GetCurrentTenant

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected Tenant GetCurrentTenant()
        {
            var tenantId = AbpSession.GetTenantId();
            return UsingDbContext(null, context => context.Tenants.Single(t => t.Id == tenantId));
        }

        /// <summary>
        /// Gets current tenant if <see cref="IAbpSession.TenantId"/> is not null.
        /// Throws exception if there is no current tenant.
        /// </summary>
        protected async Task<Tenant> GetCurrentTenantAsync()
        {
            var tenantId = AbpSession.GetTenantId();
            return await UsingDbContext(null, context => context.Tenants.SingleAsync(t => t.Id == tenantId));
        }

        #endregion

        #region GetTenant / GetTenantOrNull

        protected Tenant GetTenant(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.Single(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.SingleAsync(t => t.TenancyName == tenancyName));
        }

        protected Tenant GetTenantOrNull(string tenancyName)
        {
            return UsingDbContext(null, context => context.Tenants.FirstOrDefault(t => t.TenancyName == tenancyName));
        }

        protected async Task<Tenant> GetTenantOrNullAsync(string tenancyName)
        {
            return await UsingDbContext(null, async context => await context.Tenants.FirstOrDefaultAsync(t => t.TenancyName == tenancyName));
        }

        #endregion

        #region GetRole

        protected Role GetRole(string roleName)
        {
            return UsingDbContext(context => context.Roles.Single(r => r.Name == roleName && r.TenantId == AbpSession.TenantId));
        }

        protected async Task<Role> GetRoleAsync(string roleName)
        {
            return await UsingDbContext(async context => await context.Roles.SingleAsync(r => r.Name == roleName && r.TenantId == AbpSession.TenantId));
        }

        #endregion

        #region GetUserByUserName

        protected User GetUserByUserName(string userName)
        {
            var user = GetUserByUserNameOrNull(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected async Task<User> GetUserByUserNameAsync(string userName)
        {
            var user = await GetUserByUserNameOrNullAsync(userName);
            if (user == null)
            {
                throw new Exception("Can not find a user with username: " + userName);
            }

            return user;
        }

        protected User GetUserByUserNameOrNull(string userName)
        {
            return UsingDbContext(context =>
                context.Users.FirstOrDefault(u =>
                    u.UserName == userName &&
                    u.TenantId == AbpSession.TenantId
                    ));
        }

        protected async Task<User> GetUserByUserNameOrNullAsync(string userName, bool includeRoles = false)
        {
            return await UsingDbContextAsync(async context =>
                await context.Users
                    .IncludeIf(includeRoles, u => u.Roles)
                    .FirstOrDefaultAsync(u =>
                            u.UserName == userName &&
                            u.TenantId == AbpSession.TenantId
                    ));
        }

        #endregion


        #region Helper Methods
        //....chartOfAccount...........
        protected ChartOfAccount CreateChartOfAccount(string accountCode,string accountName,string description,long accountTypeId,Guid? parentAccountId,long taxId)
        {
            var chartOfAccountEntity = ChartOfAccount.Create(null, AbpSession.GetUserId(), accountCode, accountName, description, accountTypeId,parentAccountId, taxId);
            UsingDbContext(context =>
            {
                context.ChartOfAccounts.Add(chartOfAccountEntity);
                context.SaveChanges();
                chartOfAccountEntity.Id.Equals(parentAccountId);
            });
            return chartOfAccountEntity;
        }
        //.............end code........

        protected Tax CreateTax(string name, decimal rate)
        {
            var taxEntity = Tax.Create(null, null, name, rate);

            UsingDbContext(context =>
            {
                context.Taxes.Add(taxEntity);
                context.SaveChanges();
                taxEntity.Id.ShouldNotBe(0);
            });


            return taxEntity;
        }

        protected AccountType CreateAccountType(string name, string description, TypeOfAccount type)
        {
            var accountType = AccountType.Create(AbpSession.GetUserId(), name, type, description);

            UsingDbContext(context =>
            {
                context.AccountTypes.Add(accountType);
                context.SaveChanges();
                accountType.Id.ShouldNotBe(0);
            });


            return accountType;
        }
        //...............Create Currency ..............................................
        protected Currency CreateCurrency(string Code, string Symbol,string Name, string PluralName)
        {
            var currency = Currency.Create(AbpSession.GetUserId(), Code, Symbol,Name,PluralName);

            UsingDbContext(context =>
            {
                context.Currencies.Add(currency);
                context.SaveChanges();
                currency.Id.ShouldNotBe(0);
            });


            return currency;
        }
        protected Vendor CreateVendor(string VendorName, string VendorCode, string Email, string PhoneNumber,bool SameAsShippingAddress, string Websit ,string Remark, CAddress BillingAddress, CAddress ShippingAddress,Guid accountId,VendorType vendorType)
        {
            var vendor = Vendor.Create(AbpSession.TenantId, AbpSession.GetUserId(),VendorCode,VendorName,Email,Websit,Remark,BillingAddress,ShippingAddress,SameAsShippingAddress,PhoneNumber,accountId,vendorType.Id);

            UsingDbContext(context =>
            {
                context.Vendors.Add(vendor);
                context.SaveChanges();
                vendor.Id.ShouldNotBeNull();
            });


            return vendor;
        }

        protected Customer CreateCustomer(string Name, string Code, string Email, string PhoneNumber, bool SameAsShippingAddress, string Websit, string Remark, CAddress BillingAddress, CAddress ShippingAddress, Guid accountId,long customerTypeId)
        {
            var customer = Customer.Create(AbpSession.TenantId, AbpSession.GetUserId(), Code, Name, Email, Websit, Remark, BillingAddress, ShippingAddress, SameAsShippingAddress, PhoneNumber, accountId,customerTypeId,false);

            UsingDbContext(context =>
            {
                context.Customers.Add(customer);
                context.SaveChanges();
                customer.Id.ShouldNotBeNull();
            });


            return customer;
        }

        protected PurchaseOrder CreatePurchaseOrder(Guid vendorId, CAddress shippingAddress, CAddress billingAddress, bool sameAsShippingAddress, string reference, long currencyId, string orderNumber, DateTime orderDate, string memo, decimal tax, decimal total, decimal subTotal, TransactionStatus status,DateTime DTA)
        {
            var purchase = PurchaseOrder.Create(AbpSession.TenantId, AbpSession.GetUserId(),vendorId,shippingAddress,billingAddress,sameAsShippingAddress,reference,currencyId,orderNumber,orderDate,memo,tax,total,subTotal,status,DTA,null,0,0,currencyId,0, false);
            UsingDbContext(context =>
            {
                context.PurchaseOrders.Add(purchase);
                context.SaveChanges();
                purchase.Id.ShouldNotBeNull();
            });
            return purchase;
        }
        protected  PurchaseOrderItem CreatePurchaseOrderItem(Guid purchaseOrderId, Guid itemId, long taxId, decimal taxRate, string description, decimal unit, decimal unitCost, decimal discount, decimal total)
        {
            var purchaseOrderItem = PurchaseOrderItem.Create(AbpSession.TenantId, AbpSession.GetUserId(), itemId, purchaseOrderId, taxId, taxRate, description, unit, unit, discount, total,0,0);
            UsingDbContext(context =>
            {
                context.PurchaseOrderItems.Add(purchaseOrderItem);
                context.SaveChanges();
                purchaseOrderItem.Id.ShouldNotBeNull();
            });
            return purchaseOrderItem;
        }

        #region SaleOrder
        protected SaleOrder CreateSaleOrder(Guid customerId, CAddress shippingAddress, CAddress billingAddress, bool sameAsShippingAddress, string reference, long currencyId, string orderNumber, DateTime orderDate, string memo, decimal tax, decimal total, decimal subTotal, TransactionStatus status,DateTime Etd)
        {
            var saleOrder = SaleOrder.Create(AbpSession.TenantId, AbpSession.GetUserId(), customerId, shippingAddress, billingAddress, sameAsShippingAddress, reference, currencyId, orderNumber, orderDate, memo, tax, total, subTotal, status,Etd,null,currencyId,0,0,0, false);
            UsingDbContext(context =>
            {
                context.SaleOrder.Add(saleOrder);
                context.SaveChanges();
                saleOrder.Id.ShouldNotBeNull();
            });
            return saleOrder;
        }
        protected SaleOrderItem CreateSaleOrderItem(Guid saleOrderId, Guid itemId, long taxId, decimal taxRate, string description, decimal unit, decimal unitCost, decimal discount, decimal total)
        {
            var saleOrderItem = SaleOrderItem.Create(AbpSession.TenantId, AbpSession.GetUserId(), itemId, saleOrderId, taxId, taxRate, description, unit, unit, discount, total,0,0);
            UsingDbContext(context =>
            {
                context.SaleOrderItems.Add(saleOrderItem);
                context.SaveChanges();
                saleOrderItem.Id.ShouldNotBeNull();
            });
            return saleOrderItem;
        }
        #endregion


        #region CustomerCredit
        protected CustomerCredit CreateCustomerCredit(
            Guid customerId, CAddress shippingAddress, CAddress billingAddress, 
            bool sameAsShippingAddress, string reference, 
            string orderNumber, 
            DateTime orderDate, string memo, decimal tax, 
            decimal total, decimal subTotal,
            TransactionStatus status, 
            DateTime dueDate,
            bool convertToItemReceipt,
            DateTime? receiveDate)
        {
            var customerCredit = CustomerCredit.Create(
                AbpSession.TenantId, 
                AbpSession.GetUserId(), 
                customerId,               
                sameAsShippingAddress,
                shippingAddress, 
                billingAddress,
                subTotal,
                tax,
                total,
                dueDate,
                convertToItemReceipt,
                receiveDate,0,0,0,ReceiveFrom.None,null,false, false, false
                );
            UsingDbContext(context =>
            {
                context.CustomerCredit.Add(customerCredit);
                context.SaveChanges();
                customerCredit.Id.ShouldNotBeNull();
            });
            return customerCredit;
        }
        protected CustomerCreditDetail CreateCustomerCreditItem(
            Guid customerCreditId,
            Guid itemId, 
            long taxId, 
            decimal taxRate, 
            string description, 
            decimal qty, 
            decimal unitCost, 
            decimal discount, 
            decimal total)
        {
            var customerCreditItem = CustomerCreditDetail.Create(
                AbpSession.TenantId, 
                AbpSession.GetUserId(),
                customerCreditId,
                taxId,
                itemId, 
                description,
                qty,
                unitCost, 
                discount, 
                total,0,0,null,null);
            UsingDbContext(context =>
            {
                context.CustomerCreditDetail.Add(customerCreditItem);
                context.SaveChanges();
                customerCreditItem.Id.ShouldNotBeNull();
            });
            return customerCreditItem;
        }
        #endregion


        #region Vendor Credit
        protected VendorCredit.VendorCredit CreateVendorCredit(
            Guid customerId, CAddress shippingAddress, CAddress billingAddress,
            bool sameAsShippingAddress, string reference,
            string orderNumber,
            DateTime orderDate, string memo, decimal tax,
            decimal total, decimal subTotal,
            TransactionStatus status, DateTime dueDate,DateTime? issuedate, 
            bool convertToItemIssueVendor)
        {
            var vendorCredit = VendorCredit.VendorCredit.Create(
                AbpSession.TenantId,
                AbpSession.GetUserId(),
                customerId,
               
                sameAsShippingAddress,
                shippingAddress,
                billingAddress,
                subTotal,
                tax,
                total,
                dueDate,
                issuedate,
                convertToItemIssueVendor,0,0,0,null,ReceiveFrom.None, false, false
                );
            UsingDbContext(context =>
            {
                context.VendorCredit.Add(vendorCredit);
                context.SaveChanges();
                vendorCredit.Id.ShouldNotBeNull();
            });
            return vendorCredit;
        }
        protected VendorCredit.VendorCreditDetail CreateVendorCreditItem(
            Guid vendorCreditId,
            Guid itemId,
            long taxId,
            decimal taxRate,
            string description,
            decimal qty,
            decimal unitCost,
            decimal discount,
            decimal total)
        {
            var vendorCreditItem = VendorCredit.VendorCreditDetail.Create(
                AbpSession.TenantId,
                AbpSession.GetUserId(),
                vendorCreditId,
                taxId,
                itemId,
                description,
                qty,
                unitCost,
                discount,
                total,0,0,null,null);
            UsingDbContext(context =>
            {
                context.VendorCreditDetail.Add(vendorCreditItem);
                context.SaveChanges();
                vendorCreditItem.Id.ShouldNotBeNull();
            });
            return vendorCreditItem;
        }
        #endregion

        protected ItemType CreateItemType(long? creatorUserId, string name, bool displayItemCategory, bool displayInventoryAccount,
            bool displayPurchase, bool displaySale, bool displayReorderPoint, bool displayTrackSerialNumber, bool displaySubItem,bool displayUOM)
        {
            var itemType = ItemType.Create(AbpSession.GetUserId(),name, displayInventoryAccount, displayInventoryAccount, displaySale, displayReorderPoint, displayPurchase, displaySubItem,false);

            UsingDbContext(context =>
            {
                context.ItemTypes.Add(itemType);
                context.SaveChanges();
                itemType.Id.ShouldNotBe(0);
            });


            return itemType;
        }
        //..........................end code..........................................

        //...............Create Property ..............................................
        protected Property CreateProperty(string name,bool isUnit)
        {
            var Properties = Property.Create(AbpSession.TenantId,AbpSession.GetUserId(),name,isUnit,false,false,false, false);

            UsingDbContext(context =>
            {
                context.Properties.Add(Properties);
                context.SaveChanges();
                Properties.Id.ShouldNotBe(0);
            });


            return Properties;
        }
        //..........................end code..........................................
        //...............Create PropertyValue ..............................................
        protected PropertyValue CreatePropertyValue(string value,long propertyId,decimal netWeight,decimal grossWeight)
        {
            var PropertyValues = PropertyValue.Create(AbpSession.TenantId, AbpSession.GetUserId(), propertyId,value,netWeight,grossWeight,false,"00");

            UsingDbContext(context =>
            {
                context.PropertyValues.Add(PropertyValues);
                context.SaveChanges();
                PropertyValues.Id.ShouldNotBe(0);
            });


            return PropertyValues;
        }
        //..........................end code..........................................

        protected Item CreateItem(string ItemName ,string ItemCode,decimal? salePrice,decimal? purchaseCost,decimal? reorderPoint,bool trackSerial,long? saleCurrenyId,long? purchaseCurrencyId,long? purchaseTaxId,long?saleTaxId,Guid?saleAccountId,Guid? purchaseAccountId,Guid?inventoryAccountId,long itemTypeId,string description, string barcode, bool useBatchNo, bool autoBatchNo, long? batchNoFormulaId, bool trackExpiration)
        {
            var item = Item.Create(AbpSession.TenantId, AbpSession.GetUserId(), ItemName, ItemCode,salePrice,purchaseCost,reorderPoint,trackSerial,saleCurrenyId,purchaseCurrencyId,purchaseTaxId,saleTaxId,saleAccountId,purchaseAccountId,inventoryAccountId,itemTypeId,description, barcode, useBatchNo, autoBatchNo, batchNoFormulaId, trackExpiration,false);
            UsingDbContext(context =>
            {
                context.Items.Add(item);
                context.SaveChanges();
                item.Id.ShouldNotBeNull();
            });
            return item;
        }
        
        protected Format CreateFormat(long? creatorUserId, string name, string key, string web)
        {
            var Formats = Format.Create(AbpSession.GetUserId(),name, key, web);
            UsingDbContext(context =>
            {
                context.Formats.Add(Formats);
                context.SaveChanges();
                Formats.Id.ShouldNotBeNull();
            });
            return Formats;
        }
        protected AccountCycle CreateAccountCycle(
            int? tenantId, long? creatorUserId, 
            DateTime startDate, DateTime endDate,
            int roundingDigit,
            int roundingDigitUnitCost)
        {
            var AccountCycles = AccountCycle.Create(AbpSession.TenantId.Value, AbpSession.GetUserId(),startDate,endDate, roundingDigit, roundingDigitUnitCost);
            UsingDbContext(context =>
            {
                context.AccountCycles.Add(AccountCycles);
                context.SaveChanges();
                AccountCycles.Id.ShouldNotBeNull();
            });
            return AccountCycles;
        }
        
        protected Class CreateClass(int? tenantId, long? creatorUserId,  string className,bool classParent,long? parentClassId )
        {
            var classes = Class.Create(AbpSession.TenantId, AbpSession.GetUserId(), className, classParent, parentClassId);
            UsingDbContext(context =>
            {
            context.Classes.Add(classes);
                context.SaveChanges();
                classes.Id.ShouldNotBeNull();
            });
            return classes;
        }

        protected Location CreateLocation(int? tenantId, long? creatorUserId, string className, bool classParent, Member member, long? parentClassId)
        {
            var locations = Location.Create(AbpSession.TenantId, AbpSession.GetUserId(), className, classParent, member, parentClassId,"");
            UsingDbContext(context =>
            {
                context.Locations.Add(locations);
                context.SaveChanges();
                locations.Id.ShouldNotBeNull();
            });
            return locations;
        }

        #endregion
        //create itemReceipt and bill 

        protected ItemReceipt CreateItemReceipt(int? tenantId, long? creatorUserId, ReceiveFromStatus receiveFrom,
               Guid vendorId, bool sameAsShippingAddress,
               CAddress shippingAddress, CAddress billingAddress, decimal total)
        {

            var itemReceipt = Create(AbpSession.TenantId, AbpSession.GetUserId(), receiveFrom, 
                vendorId, sameAsShippingAddress,
                shippingAddress, billingAddress, total,null);
            UsingDbContext(context =>
            {
                context.ItemReceipts.Add(itemReceipt);
                context.SaveChanges();
                itemReceipt.Id.ShouldNotBeNull();
            });
            return itemReceipt;
        }

        protected ItemReceiptItem CreateItemReceiptItems(int? tenantId, long? creatorUserId, Guid itemReceiptId,
          Guid itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {

            var itemReceiptItem = ItemReceiptItem.Create(AbpSession.TenantId,
                AbpSession.GetUserId(), itemReceiptId, itemId, description, qty, unitCost, discountRate, total);
            UsingDbContext(context =>
            {
                context.ItemReceiptItems.Add(itemReceiptItem);
                context.SaveChanges();
                itemReceiptItem.Id.ShouldNotBeNull();
            });
            return itemReceiptItem;
        }

        public ItemIssue CreateItemIssue(int? tenantId, long? creatorUserId, ReceiveFrom receiveFrom,
            Guid customer, bool sameAsShippingAddress,
            CAddress shippingAddress, CAddress billingAddress, decimal total)
        {

            var itemIssue = ItemIssue.Create(AbpSession.TenantId, AbpSession.GetUserId(), receiveFrom,
                customer, sameAsShippingAddress,
                shippingAddress, billingAddress, total,null);

            UsingDbContext(context =>
            {
                context.ItemIssues.Add(itemIssue);
                context.SaveChanges();
                itemIssue.Id.ShouldNotBeNull();
            });
            return itemIssue;

        }
        
        protected ItemIssueItem CreateItemIssueItems(int? tenantId, long? creatorUserId, Guid itemIssueId,
        Guid itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {

            var itemIssueItem = ItemIssueItem.Create(AbpSession.TenantId,
                AbpSession.GetUserId(), itemIssueId, itemId, description, qty, unitCost, discountRate, total);
            UsingDbContext(context =>
            {
                context.ItemIssueItems.Add(itemIssueItem);
                context.SaveChanges();
                itemIssueItem.Id.ShouldNotBeNull();
            });
            return itemIssueItem;
        }

        protected Bills.Bill CreateBillCreate(int? tenantId, long? creatorUserId, Bills.Bill.ReceiveFromStatus status,
             DateTime dueDate,Guid vendorId, long locationId, bool sameAsShippingAddress,
             CAddress shippingAddress, CAddress billingAddress, decimal subTotal,
             decimal tax, decimal total, Guid? itemReceiptId,DateTime ETA,bool ConventToItemReceipt,DateTime? itemReceiptDate)
        {
            var bill = Bills.Bill.Create(AbpSession.TenantId, AbpSession.GetUserId(), status, dueDate,
                vendorId, sameAsShippingAddress, shippingAddress, billingAddress,
                subTotal, tax, total, itemReceiptId,ETA, ConventToItemReceipt,itemReceiptDate,0,0,0, false, false);

            UsingDbContext(context =>
            {
                context.Bills.Add(bill);
                context.SaveChanges();
                bill.Id.ShouldNotBeNull();
            });
            return bill;
        }

        protected Bills.BillItem CreateBillItem(int? tenantId, long? creatorUserId,Guid billId, long taxId, Guid? itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {
            var billItem = Bills.BillItem.Create(AbpSession.TenantId, AbpSession.GetUserId(), 
                billId, taxId,itemId.Value, description, qty, unitCost, discountRate, total,0,0);
            UsingDbContext(context =>
            {
                context.BillItems.Add(billItem);
                context.SaveChanges();
                billItem.Id.ShouldNotBeNull();
            });
            return billItem;
        }

        #region create fake databse for invoice and invoiceItem
        protected Invoice CreateInvoicCreate(int? tenantId, long? creatorUserId,  ReceiveFrom status,
          DateTime dueDate, Guid customerId, bool sameAsShippingAddress,
          CAddress shippingAddress, CAddress billingAddress, decimal subTotal,
          decimal tax, decimal total, Guid? itemReceiptId, DateTime ETA, DateTime? receiveDate, bool convertToItemIssue)
        {
            var invoice = Invoice.Create(AbpSession.TenantId, AbpSession.GetUserId(), status, dueDate,
                customerId, sameAsShippingAddress, shippingAddress, billingAddress,
                subTotal, tax, total, itemReceiptId, ETA, receiveDate, convertToItemIssue,0,0,0, false, false);

            UsingDbContext(context =>
            {
                context.Invoice.Add(invoice);
                context.SaveChanges();
                invoice.Id.ShouldNotBeNull();
            });
            return invoice;
        }

        protected InvoiceItem CreateInvoiceItem(int? tenantId, long? creatorUserId, Guid invocieId, long taxId, Guid? itemId, string description, decimal qty, decimal unitCost, decimal discountRate, decimal total)
        {
            var invoiceItem = InvoiceItem.Create(AbpSession.TenantId, AbpSession.GetUserId(),
                invocieId, taxId, itemId.Value, description, qty, unitCost, discountRate, total,0,0);
            UsingDbContext(context =>
            {
                context.InvoiceItems.Add(invoiceItem);
                context.SaveChanges();
                invoiceItem.Id.ShouldNotBeNull();
            });
            return invoiceItem;
        }
        #endregion




        protected Journal CreateGeneralJournal(int? tenantId, long creatorUserId,
            string journalNo, DateTime date, string memo,
            decimal debit, decimal credit,
            long currencyId, long? classId, string reference,long? locationId)
        {
            var generalJournal = Journal.Create(AbpSession.TenantId, AbpSession.GetUserId(), journalNo, date, memo, debit, credit, currencyId, classId, reference,locationId);
            UsingDbContext(context =>
            {
                context.Journals.Add(generalJournal);
                context.SaveChanges();
                generalJournal.Id.ShouldNotBeNull();
            });
            return generalJournal;
        }


        protected JournalItem CreateJournalItem( int? tenantId,long? creatorUserId, Guid journalId,
            Guid accountId, string description,decimal debit,decimal credit, PostingKey key, Guid? identifier)
        {
            var generalJournalitem = JournalItem.CreateJournalItem(AbpSession.TenantId, AbpSession.GetUserId(), 
                journalId, accountId, description, debit, credit, key, identifier);
            UsingDbContext(context =>
            {
                context.JournalItems.Add(generalJournalitem);
                context.SaveChanges();
                generalJournalitem.Id.ShouldNotBeNull();
            });
            return generalJournalitem;
        }


    }
}