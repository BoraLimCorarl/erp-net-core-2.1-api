using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static CorarlERP.enumStatus.EnumStatus;

namespace CorarlERP.AutoSequences
{
    public class AutoSequenceManager : CorarlERPDomainServiceBase, IAutoSequenceManager
    {
        private readonly IRepository<AutoSequence, Guid> _autoSequenceRepository;

        public AutoSequenceManager(IRepository<AutoSequence, Guid> autoSequenceRepository)
        {
            _autoSequenceRepository = autoSequenceRepository;
        }
        public async virtual Task<IdentityResult> CreateAsync(AutoSequence entity)
        {
            await _autoSequenceRepository.InsertAsync(@entity);
            return IdentityResult.Success;
        }


        public virtual async Task<AutoSequence> GetAsync(Guid id, bool tracking = false)
        {
            var @query = tracking ? _autoSequenceRepository.GetAll() : _autoSequenceRepository.GetAll().AsNoTracking();
            return await query.FirstOrDefaultAsync(u => u.Id == id);
        }


        public List<DocumentTypeData> GetDocumentTypes()
        {
            var list = new List<DocumentTypeData>();
            list.Add(DocumentTypeData.Create(DocumentType.PurchaseOrder, "Purchase Order", 1, "PO", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true,null));
            list.Add(DocumentTypeData.Create(DocumentType.ItemReceipt, "Item ", 2, "IR", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true, null));            
            list.Add(DocumentTypeData.Create(DocumentType.ItemReceipt_CustomerCredit, "Item Receipt Customer Credit", 3, "IRCC", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true, null));           
            list.Add(DocumentTypeData.Create(DocumentType.Bill, "Bill", 4, "Bill", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.VendorCredit, "Vendor Credit", 5, "VC", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.PayBill, "Pay Bill", 6, "PB", DocumentTypeGroup.Vendor, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.SaleOrder, "Sale Order", 7, "SO", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.Invoice, "Invoice", 8, "INV", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));            
            list.Add(DocumentTypeData.Create(DocumentType.CustomerCredit, "Customer Credit", 9, "CC", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.ItemIssue, "Item Issue", 10, "IIS", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.ItemIssue_VendorCredit, "Item Issue Vendor Credit", 11, "IIVC", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));           
            list.Add(DocumentTypeData.Create(DocumentType.RecievePayment, "Recieve Payment", 12, "RP", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.BankTransferOrder, "Bank Transfer Order", 13, "BTO", DocumentTypeGroup.Bank, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.Deposit, "Deposit", 14, "DPS", DocumentTypeGroup.Bank, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.Withdraw, "Withdraw", 15, "WD", DocumentTypeGroup.Bank, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.ProductionOrder, "Production Order", 16, "PDO", DocumentTypeGroup.Production, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.TransferOrder, "Transfer Order", 17, "TO", DocumentTypeGroup.Inventory, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.PhysicalCount, "Physical Count", 18, "PC", DocumentTypeGroup.Inventory, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.POS, "POS", 19, "POS", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.ProductionPlan, "Production Plan", 20, "PDP", DocumentTypeGroup.Production, "-", "0001", YearFormat.YY, true, null));
            list.Add(DocumentTypeData.Create(DocumentType.KitchenOrder, "Kitchen Order", 21, "KO", DocumentTypeGroup.Inventory, "-", "0001", YearFormat.YY, true, null));
            //list.Add(DocumentTypeData.Create(DocumentType.DeliverySchedule, "Delivery Schedule", 22, "DS", DocumentTypeGroup.Customer, "-", "0001", YearFormat.YY, true, null));
            return list;

        }
        public class DocumentTypeData
        {
            public DocumentType Type { get; set; }
            public string Title { get; set; }
            public int SortOrer { get; set; }
            public string AutoSequenceTitle { get; set; }
            public string DefaultPrefix { get; set; }
            public DocumentTypeGroup Group { get; set; }
            public string SybolFormat { get; set; }
            public string NumberFormat { get; set; }
            public bool CustomFormat { get; set; }
            public YearFormat YearFormat { get; set; }
            public string LastAutoSequence { get; set; }

            public static DocumentTypeData Create(DocumentType type, string title, int sortOrder,
                   string autoSequenceTitle, DocumentTypeGroup group, string symbolFormat, string numberFormat,
                   YearFormat yearFormat, bool customFormat, string lastAutoSequence)
            {
                var result = new DocumentTypeData();
                result.Type = type;
                result.Title = title;
                result.SortOrer = sortOrder;
                result.AutoSequenceTitle = autoSequenceTitle;
                result.Group = group;
                result.SybolFormat = symbolFormat;
                result.DefaultPrefix =
                result.NumberFormat = numberFormat;
                result.YearFormat = yearFormat;
                result.CustomFormat = customFormat;
                result.LastAutoSequence = lastAutoSequence;
                return result;
            }
        }
        public virtual async Task<IdentityResult> RemoveAsync(AutoSequence entity)
        {
            await _autoSequenceRepository.DeleteAsync(@entity);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(AutoSequence entity)
        {

            await _autoSequenceRepository.UpdateAsync(@entity);
            return IdentityResult.Success;
        }

        public async Task<AutoSequence> GetAutoSequenceAsync(DocumentType documentType)
        {
            var autoSequence = await _autoSequenceRepository.GetAll()
              .Where(t => t.DocumentType == documentType)
              .FirstOrDefaultAsync();

            return autoSequence;
        }

        //public string GetNewReferenceNumber(string defaultPrefix, 
        //                                    YearFormat yearFormat, 
        //                                    string symbolFormat, 
        //                                    string numberFormat,
        //                                    string lastAutoSequenceNumber,
        //                                    DateTime currentDate)
        //{
        //    var prefixTem = string.IsNullOrEmpty(defaultPrefix) ? "" : defaultPrefix;
        //    var yearFormatTemp = yearFormat == YearFormat.YY ? currentDate.ToString("yy") :
        //                     yearFormat == YearFormat.YYYY ? currentDate.ToString("yyyy") : "";
        //    var symbolFormatTemp = string.IsNullOrEmpty(symbolFormat) ? "" : symbolFormat;

        //    var combinePrefixeTemp = $"{prefixTem}{yearFormatTemp}{symbolFormatTemp}";

        //    var storePrefixTemp = string.IsNullOrEmpty(lastAutoSequenceNumber) ? "" :
        //                      lastAutoSequenceNumber.Substring(0, combinePrefixeTemp.Length);

        //    var referenceNumber = "";

        //    if (!string.IsNullOrEmpty(storePrefixTemp) && combinePrefixeTemp == storePrefixTemp)
        //    {
        //        var numberFormatLength = lastAutoSequenceNumber.Length - storePrefixTemp.Length;

        //        var previousNumberFormat = lastAutoSequenceNumber
        //                                  .Substring(combinePrefixeTemp.Length, numberFormatLength);

        //        var increPreviousNumberFormat = (Convert.ToUInt16(previousNumberFormat) + 1).ToString();
        //        if (increPreviousNumberFormat.Length >= numberFormat.Length)
        //        {
        //            referenceNumber = $"{combinePrefixeTemp}{increPreviousNumberFormat}";
        //        }
        //        else
        //        {
        //            var leadingZeroToAddCount = numberFormat.Length - increPreviousNumberFormat.Length;

        //            var leadingZeroText = "";
        //            for (int i =0; i < leadingZeroToAddCount; i++)
        //            {
        //                leadingZeroText += "0";
        //            }
        //            referenceNumber = $"{combinePrefixeTemp}{leadingZeroText}{increPreviousNumberFormat}";

        //        }

        //        return referenceNumber;


        //    }
        //    else
        //    {
        //        return $"{combinePrefixeTemp}{numberFormat}";
        //    }
        //}


        public string GetNewReferenceNumber(string defaultPrefix,
                                    YearFormat yearFormat,
                                    string symbolFormat,
                                    string numberFormat,
                                    string lastAutoSequenceNumber,
                                    DateTime currentDate)
        {
            // Construct the prefix based on the inputs
            string prefix = (defaultPrefix ?? "") +
                            (yearFormat == YearFormat.YY ? currentDate.ToString("yy") :
                            yearFormat == YearFormat.YYYY ? currentDate.ToString("yyyy") : "") +
                            (symbolFormat ?? "");

            // If the last auto sequence number is provided, extract the prefix part
            string storedPrefix = !string.IsNullOrEmpty(lastAutoSequenceNumber)
                ? lastAutoSequenceNumber.Substring(0, prefix.Length)
                : string.Empty;

            // Initialize the reference number
            string referenceNumber;

            if (!string.IsNullOrEmpty(storedPrefix) && prefix == storedPrefix)
            {
                // Extract and increment the numeric part of the last auto sequence number
                string previousNumber = lastAutoSequenceNumber.Substring(prefix.Length);
                int incrementedNumber = Convert.ToInt32(previousNumber) + 1;

                // Format the incremented number with leading zeros if needed
                referenceNumber = prefix + incrementedNumber.ToString().PadLeft(numberFormat.Length, '0');
            }
            else
            {
                // If no valid previous sequence is found, start with the initial number format
                referenceNumber = prefix + numberFormat;
            }

            return referenceNumber;
        }


    }
}
