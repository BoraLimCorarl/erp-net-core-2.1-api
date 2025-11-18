using Abp.Timing;
using CorarlERP.Addresses;
using CorarlERP.Classes;
using CorarlERP.Currencies;
using CorarlERP.Customers;
using CorarlERP.Locations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CorarlERP.enumStatus.EnumStatus;
namespace CorarlERP.KitchenOrders
{
    [Table("CarlErpKitchenOrders")]
    public class KitchenOrder : BaseAuditedEntity<Guid>
    {
        public const int MaxOrderNumberLength = 128;      
        public Guid? CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        [Required]
        [MaxLength(MaxOrderNumberLength)]
        public string OrderNumber { get; private set; }
        public DateTime OrderDate { get; private set; }
        [MaxLength(MaxOrderNumberLength)]
        public string Reference { get; private set; }
        [Required]
        public long CurrencyId { get; private set; }
        public Currency Currency { get; private set; }
        public long? MultiCurrencyId { get; private set; }
        public Currency MultiCurrency { get; private set; }
        public long? LocationId { get; private set; }
        public Location Location { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal Tax { get; private set; }
        public decimal Total { get; private set; }
        public decimal MultiCurrencySubTotal { get; private set; }
        public decimal MultiCurrencyTotal { get; private set; }
        public decimal MultiCurrencyTax { get; private set; }
        public bool SameAsShippingAddress { get; private set; }
        public string Memo { get; private set; }     
        public CAddress BillingAddress { get; private set; }
        public CAddress ShippingAddress { get; private set; }

        public TransactionStatus Status { get; private set; }
        public Class Class { get; private set; }
        public long ClassId { get; private set; }

        public void SetClass(long classId)
        {
            this.ClassId = classId;
        }
        public void SetLocation(long? locationId)
        {
            this.LocationId = locationId;
        }

        public void ChangeDate(DateTime date)
        {
            this.OrderDate = date;
        }

        public void SetCurrency(long? companyCurrencyId, long? multiCurrencyId)
        {

            this.MultiCurrencyId = multiCurrencyId;
            this.CurrencyId = companyCurrencyId.Value;
        }


        public void UpdateStatusToDraft()
        {
            Status = TransactionStatus.Draft;
        }
        public void UpdateStatusToClose()
        {
            Status = TransactionStatus.Close;
        }
        public void UpdateStatusToVoid()
        {
            Status = TransactionStatus.Void;
        }
        public void UpdateStatusToPublish()
        {
            Status = TransactionStatus.Publish;
        }

        public void SetSubTotal(decimal subTotal) { SubTotal = subTotal; }
        public void SetTax(decimal tax) { Tax = tax; }
        public void SetTotal(decimal total) { Total = total; }    
        public void SetMultiCurrencySubTotal(decimal subTotal) => MultiCurrencySubTotal = subTotal;
        public void SetMultiCurrencyTotal(decimal total) => MultiCurrencyTotal = total;
        public static KitchenOrder Create(int? tenantId, long creatorUserId, Guid? customerId,
                                          CAddress shippingAddress, CAddress billingAddress, bool sameAsShippingAddress,
                                          string reference, long currencyId, string orderNumber, DateTime orderDate,
                                          string memo, decimal tax, decimal total, decimal subTotal, TransactionStatus status,
                                          long? locationId, long? multiCurrencyId, decimal multiCurrencySubTotal,
                                          decimal multiCurrencyTotal, decimal multiCurrencyTax,
                                          long classId)
        {
            return new KitchenOrder()
            {
                MultiCurrencyTax = multiCurrencyTax,
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CreatorUserId = creatorUserId,
                CreationTime = Clock.Now,
                CustomerId = customerId,
                BillingAddress = new CAddress(billingAddress.CityTown,
                    billingAddress.Country, billingAddress.PostalCode,
                    billingAddress.Province, billingAddress.Street),
                SameAsShippingAddress = sameAsShippingAddress,
                ShippingAddress = sameAsShippingAddress ? new CAddress(billingAddress.CityTown,
                    billingAddress.Country, billingAddress.PostalCode,
                    billingAddress.Province, billingAddress.Street) :
                    new CAddress(
                        shippingAddress.CityTown,
                        shippingAddress.Country, shippingAddress.PostalCode,
                        shippingAddress.Province, shippingAddress.Street),
                Reference = reference,
                CurrencyId = currencyId,
                OrderNumber = orderNumber,
                OrderDate = orderDate,
                Tax = tax,
                SubTotal = subTotal,
                Memo = memo,
                Total = total,           
                Status = status,             
                LocationId = locationId,
                MultiCurrencyId = multiCurrencyId,
                MultiCurrencySubTotal = multiCurrencySubTotal,
                MultiCurrencyTotal = multiCurrencyTotal,
                ClassId = classId
            };
        }
        public void Update( long lastModifiedUserId, Guid? customerId,
                            string reference, long currencyId, string orderNumber,
                            DateTime orderDate, string memo, CAddress shippingAddress,
                            CAddress billingAddress, bool sameAsShippingAddress,
                            decimal subTotal, decimal tax, decimal total,
                            TransactionStatus status,  long? locationId,
                            long? multiCurrencyId, decimal multiCurrencySubTotal, 
                            decimal multiCurrencyTotal, decimal multiCurrencyTax,
                            long classId)
        {
            MultiCurrencyTax = multiCurrencyTax;
            LastModifierUserId = lastModifiedUserId;
            LastModificationTime = Clock.Now;
            CustomerId = customerId;
            BillingAddress.Update(billingAddress);
            ShippingAddress.Update(sameAsShippingAddress ? billingAddress : shippingAddress);
            Reference = reference;
            CurrencyId = currencyId;
            OrderNumber = orderNumber;
            OrderDate = orderDate;
            Tax = tax;
            Total = total;
            SubTotal = subTotal;
            Memo = memo;
            SameAsShippingAddress = sameAsShippingAddress;
            Status = status;          
            LocationId = locationId;
            MultiCurrencyId = multiCurrencyId;
            MultiCurrencySubTotal = multiCurrencySubTotal;
            MultiCurrencyTotal = multiCurrencyTotal;
            ClassId = classId;

        }

    }
}
