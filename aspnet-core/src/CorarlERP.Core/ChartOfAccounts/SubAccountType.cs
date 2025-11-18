using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Amazon.S3.Model;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorarlERP.ChartOfAccounts
{
    //public enum AccountType
    //{
    //    Cash = 10,
    //    Bank = 11,
    //    AccountReceivable = 12,
    //    Inventory = 13,
    //    CurrentAsset = 14,
    //    FixedAsset = 15,
    //    NoneCurrentAsset = 16,
    //    AccountPayable = 20,
    //    CreditCard = 21,
    //    CurrentLiability = 22,
    //    NoneCurrentLiability = 23,
    //    Equity = 30,
    //    Revenue = 40,
    //    OtherRevenue = 41,
    //    CostOfSale = 50,
    //    Expense = 60,
    //    OtherExpense = 61
    //}

    public enum SubAccountType
    {
        //CashOnHand = 1000,
        //CashEquivalent = 1001,
        //ClientTrustAccount = 1002,
        //MoneyMarket = 1003,
        //RentsHeldInTrust = 1004,

        //Bank = 1100,
        //Saving = 1101,

        //AccountReceivable = 1200,

        //Inventory = 1300,

        //AssetAvailableForSale = 1400,
        //DevelopmentCost = 1401,
        //EmployeeCashAdvance = 1402,
        //InvestmentOther = 1403,
        //LoansToOfficer = 1404,
        //LoansToOther = 1405,
        //LoansToShareholder = 1406,
        //PrepaidExpense = 1407,
        //Retainage = 1408,
        //UndepositedFund = 1409,
        //AllowanceForBadDebt = 1410,
        //OtherCurrentAsset = 1411,

        //Building = 1500,
        //Land = 1501,
        //Vehicle = 1502,
        //MachinaryAndEquipment = 1503,
        //FurnitureAndFixture = 1504,
        //DepletableAsset = 1505,
        //LeaseholdImprovement = 1506,
        //AccumulatedDepletion = 1507,
        AccumulatedDepreciation = 1508,
        //OtherFixedAsset = 1509,

        //AssetHeldForSale = 1600,
        //DefferedTax = 1601,
        //Goodwill = 1602,
        //IntangibleAsset = 1603,
        //LeaseBuyout = 1604,
        //Licence = 1605,
        //LongTermInvestment = 1606,
        //OrganazationalCost = 1607,
        //SecurityDeposti = 1608,
        AccumulatedAmortization = 1609,
        //OtherNoneCurrentAsset = 1610,

        //AccountPayable = 2000,

        //CreditCard = 2100,

        //DividentPayable = 2200,
        //InsurancePayable = 2201,
        //LoanPayable = 2202,
        //PrepaidExpensePayable = 2203,
        //IncomeTaxPayable = 2204,
        //SaleAndServiceTaxPayable = 2205,
        //LineOfCredit = 2206,
        //PayrollClearing = 2207,
        //PayrollLiability = 2208,
        //AccruedLiability = 2209,
        //CurrentPotionOfObligationUnderFinanceLease = 2210,
        //CurrentTaxLiability = 2211,
        //ClientTrustAccountLiability = 2212,
        //RentInTrustLiability = 2213,
        //OtherCurrentLiability = 2214,

        //NotePayable = 2300,
        //ShareholderNotePayable = 2301,
        //AccruedHolidayPayable = 2302,
        //LongTermDebt = 2303,
        //LiabilityRelatedToAssetHeldForSale = 2304,
        //AccruedNoneCurrentLiability = 2305,
        //OtherNoneCurrentLiability = 2306,

        OpeningBalanceEquity = 3000,
        //OwnersEquity = 3001,
        //PartnersEquity = 3002,
        //PartnerContribution = 3003,
        //PartnerDistribution = 3004,
        //DividendDisbursed = 3005,
        //ShareCapital = 3006,
        //OrdinaryShare = 3007,
        //PreferredShare = 3008,
        //TreasuryShare = 3009,
        //AccumulatedAdjustment = 3010,
        //OtherComprehensiveIncome = 3011,
        //PaidInCapitalOrSurplus = 3012,
        RetainedEarning = 3013,
        //EquityInEarningOfSubsidiaries = 3014,

        //SaleOfProductIncome = 4000,
        //ServiceFeeIncome = 4001,
        //SaleRetail = 4002,
        //SaleWholesale = 4003,
        //GeneralRevenue = 4004,
        //OtherPrimaryIncome = 4005,
        //NonProfitIncome = 4006,
        //DiscountOrRefundGiven = 4007,
        //UnappliedCashPaymentIncome = 4008,

        //DividendIncome = 4100,
        //InterestEarned = 4101,
        //OtherInvestmentIncome = 4102,
        //OtherOperatingIncome = 4103,
        //OtherMiscellaneousIncome = 4104,
        //TaxExemptInterest = 4105,
        //LossOnDisposalOfAsset = 4106,
        //UnrealisedLossOnSecurityNetOfTax = 4107,

        //LaborCost = 5000,
        //SuppliesAndMaterialsCost = 5001,
        //EquipmentRentalCost = 5002,
        //FreightAndDeliveryCost = 5003,
        //OtherCostOfSale = 5004,

        //AdvertisingPromotional = 6000,
        //AmortizationExpense = 6001,
        //Auto = 6002,
        //BadDebt = 6003,
        //BankCharge = 6004,
        //CharitableContribution = 6005,
        //CommisionAndFee = 6006,
        //LaborExpense = 6007,
        //DueAndSubscription = 6008,
        //EquipmentRental = 6009,
        //FinanceCost = 6010,
        //IncomeTaxExpense = 6011,
        //Insurance = 6012,
        //InterestPaid = 6013,
        //LegalAndProfessionalFee = 6014,
        //LossOnDiscontinuedOperationNetOfTax = 6015,
        //ManagementCompensation = 6016,
        //MealAndEntertai = 6017,
        //AdministrativeExpense = 6018,
        //OtherMiscelleneousServiceCost = 6019,
        //OtherSellingExpense = 6020,
        //PayrollExpense = 6021,
        //RentOrLeaseOfBuilding = 6022,
        //RepairAndMaintenance = 6023,
        //ShipingAndDeliveryExpense = 6024,
        //SuppliesAndMaterials = 6025,
        //TaxPaid = 6026,
        //TravelGeneralAndAdminExpense = 6027,
        //TravelSellExpense = 6028,
        //UnappliedCashBillPaymentExpense = 6029,
        //Utilities = 6030,

        //Amortization = 6100,
        //Depreciation = 6101,
        //ExchangeLossGain = 6102,
        //PenaltiesAndSettlements = 6103,
        //OtherExpense = 6104
    }

    public static class AccountTypeExtensions
    {
        //public static bool IsCurrentAsset(this AccountType type) => type == AccountType.Cash || type == AccountType.Bank || type == AccountType.AccountReceivable || type == AccountType.Inventory || type == AccountType.CurrentAsset;
        //public static bool IsNoneCurrentAsset(this AccountType type) => type == AccountType.FixedAsset || type == AccountType.NoneCurrentAsset;
        //public static bool IsCurrentLiability(this AccountType type) => type == AccountType.AccountPayable || type == AccountType.CreditCard || type == AccountType.CurrentLiability;
        //public static bool IsCashEquivalent(this AccountType type) => type == AccountType.Cash || type == AccountType.Bank;
        //public static bool IsRevenue(this AccountType type) => type == AccountType.Revenue || type == AccountType.OtherRevenue;
        //public static bool IsExpense(this AccountType type) => type == AccountType.Expense || type == AccountType.OtherExpense;
        //public static bool IsAccumulatedDepreciation(this SubAccountType type) => type == SubAccountType.AccumulatedDepletion || type == SubAccountType.AccumulatedDepreciation || type == SubAccountType.AccumulatedAmotization;
        //public static NameValueDto<AccountType> NameValue(this AccountType type) => new NameValueDto<AccountType> { Name = type.ToString(), Value = type };
        public static NameValueDto<SubAccountType> NameValue(this SubAccountType type) => new NameValueDto<SubAccountType> { Name = type.ToString(), Value = type };
        //public static List<AccountType> ToList(this AccountType type) => Enum.GetValues(typeof(AccountType)).Cast<AccountType>().ToList();
        public static List<SubAccountType> ToList(this SubAccountType type) => Enum.GetValues(typeof(SubAccountType)).Cast<SubAccountType>().ToList();
        //public static HashSet<AccountType> ToHash(this AccountType type) => Enum.GetValues(typeof(AccountType)).Cast<AccountType>().ToHashSet();
        public static HashSet<SubAccountType> ToHash(this SubAccountType type) => Enum.GetValues(typeof(SubAccountType)).Cast<SubAccountType>().ToHashSet();
        //public static AccountType? Parent(this SubAccountType sub) => AccountType.Cash.ToHash().FirstOrDefault(s => ((int)sub).ToString().StartsWith(((int)s).ToString()));
        //public static HashSet<SubAccountType> SubTypeHashs(this AccountType type) => SubAccountType.CashOnHand.ToList().Where(s => ((int)s).ToString().StartsWith(((int)type).ToString())).ToHashSet();
        //public static List<SubAccountType> SubTypes(this AccountType type) => SubAccountType.CashOnHand.ToList().Where(s => ((int)s).ToString().StartsWith(((int)type).ToString())).ToList();

        public static List<SubAccountType> SubTypes(this AccountType type)
        {
            List<SubAccountType> result;
            switch(type.Id) {
                case 6:
                    result = new List<SubAccountType> { 
                        SubAccountType.AccumulatedDepreciation, 
                        SubAccountType.AccumulatedAmortization 
                    };
                    break;
                case 10:
                    result = new List<SubAccountType> { 
                        SubAccountType.OpeningBalanceEquity,
                        SubAccountType.RetainedEarning 
                    };
                    break;
                default: 
                    result = new List<SubAccountType>();
                    break;            
            }

            return result;
        }

        //public static SubAccountType DefaultSubType(this AccountType type)
        //{
        //    SubAccountType result;
        //    switch (type)
        //    {
        //        case AccountType.Cash:
        //            result = SubAccountType.CashOnHand;
        //            break;
        //        case AccountType.Bank:
        //            result = SubAccountType.Bank;
        //            break;
        //        case AccountType.AccountReceivable:
        //            result = SubAccountType.AccountReceivable;
        //            break;
        //        case AccountType.Inventory:
        //            result = SubAccountType.Inventory;
        //            break;
        //        case AccountType.CurrentAsset:
        //            result = SubAccountType.OtherCurrentAsset;
        //            break;
        //        case AccountType.FixedAsset:
        //            result = SubAccountType.OtherFixedAsset;
        //            break;
        //        case AccountType.NoneCurrentAsset:
        //            result = SubAccountType.OtherNoneCurrentAsset;
        //            break;
        //        case AccountType.AccountPayable:
        //            result = SubAccountType.AccountPayable;
        //            break;
        //        case AccountType.CreditCard:
        //            result = SubAccountType.CreditCard;
        //            break;
        //        case AccountType.CurrentLiability:
        //            result = SubAccountType.OtherCurrentLiability;
        //            break;
        //        case AccountType.NoneCurrentLiability:
        //            result = SubAccountType.OtherNoneCurrentLiability;
        //            break;
        //        case AccountType.Equity:
        //            result = SubAccountType.OwnersEquity;
        //            break;
        //        case AccountType.Revenue:
        //            result = SubAccountType.GeneralRevenue;
        //            break;
        //        case AccountType.OtherRevenue:
        //            result = SubAccountType.OtherOperatingIncome;
        //            break;
        //        case AccountType.CostOfSale:
        //            result = SubAccountType.OtherCostOfSale;
        //            break;
        //        case AccountType.Expense:
        //            result = SubAccountType.AdministrativeExpense;
        //            break;
        //        case AccountType.OtherExpense:
        //            result = SubAccountType.OtherExpense;
        //            break;
        //        default:
        //            result = SubAccountType.CashEquivalent;
        //            break;
        //    }
        //    return result;
        //}

        public static string Description(this SubAccountType type)
        {
            var descriptions = new Dictionary<SubAccountType, string> {
                //{ SubAccountType.CashOnHand, "Use a Cash on hand account to track cash your company keeps for occasional expenses, also called petty cash.\r\nTo track cash from sales that have not been deposited yet, use a pre-created account called Undeposited funds, instead." },
                //{ SubAccountType.CashEquivalent, "Use Cash and Cash Equivalents to track cash or assets that can be converted into cash immediately. For example, marketable securities and Treasury bills." },
                //{ SubAccountType.ClientTrustAccount, "Use Client trust accounts for money held by you for the benefit of someone else.\r\nFor example, trust accounts are often used by attorneys to keep track of expense money their customers have given them.\r\n\r\nOften, to keep the amount in a trust account from looking like it’s yours, the amount is offset in a \"contra\" liability account (a Current Liability)." },
                //{ SubAccountType.MoneyMarket, "Use Money market to track amounts in money market accounts.\r\nFor investments, see Current Assets, instead." },
                //{ SubAccountType.RentsHeldInTrust, "Use Rents held in trust to track deposits and rent held on behalf of the property owners.\r\nTypically only property managers use this type of account." },

                //{ SubAccountType.Bank, "Use Bank accounts to track all your current activity, including debit card transactions.\r\nEach current account your company has at a bank or other financial institution should have its own Bank type account in QuickBooks Online Plus." },
                //{ SubAccountType.Saving, "Use Savings accounts to track your savings and CD activity.\r\nEach savings account your company has at a bank or other financial institution should have its own Savings type account.\r\n\r\nFor investments, see Current Assets, instead." },

                //{ SubAccountType.AccountReceivable, "Accounts receivable (also called A/R, Debtors, or Trade and other receivables) tracks money that customers owe you for products or services, and payments customers make.\r\nQuickBooks Online Plus automatically creates one Accounts receivable account for you. Most businesses need only one.\r\n\r\nEach customer has a register, which functions like an Accounts receivable account for each customer." },

                //{ SubAccountType.Inventory, "Use Inventory to track the cost of goods your business purchases for resale.\r\nWhen the goods are sold, assign the sale to a Cost of sales account." },

                //{ SubAccountType.AssetAvailableForSale, "Use Assets available for sale to track assets that are available for sale that are not expected to be held for a long period of time." },
                //{ SubAccountType.DevelopmentCost, "Use Development costs to track amounts you deposit or set aside to arrange for financing, such as an SBA loan, or for deposits in anticipation of the purchase of property or other assets.\r\nWhen the deposit is refunded, or the purchase takes place, remove the amount from this account." },
                //{ SubAccountType.EmployeeCashAdvance, "Use Employee cash advances to track employee wages and salary you issue to an employee early, or other non-salary money given to employees.\r\nIf you make a loan to an employee, use the Current asset account type called Loans to others, instead." },
                //{ SubAccountType.InvestmentOther, "Use Investments - Other to track the value of investments not covered by other investment account types. Examples include publicly-traded shares, coins, or gold." },
                //{ SubAccountType.LoansToOfficer, "If you operate your business as a Corporation, use Loans to officers to track money loaned to officers of your business." },
                //{ SubAccountType.LoansToOther, "Use Loans to others to track money your business loans to other people or businesses.\r\nThis type of account is also referred to as Notes Receivable.\r\n\r\nFor early salary payments to employees, use Employee cash advances, instead." },
                //{ SubAccountType.LoansToShareholder, "If you operate your business as a Corporation, use Loans to Shareholders to track money your business loans to its shareholders." },
                //{ SubAccountType.PrepaidExpense, "Use Prepaid expenses to track payments for expenses that you won’t recognise until your next accounting period.\r\nWhen you recognise the expense, make a journal entry to transfer money from this account to the expense account." },
                //{ SubAccountType.Retainage, "Use Retainage if your customers regularly hold back a portion of a contract amount until you have completed a project.\r\nThis type of account is often used in the construction industry, and only if you record income on an accrual basis." },
                //{ SubAccountType.UndepositedFund, "Use Undeposited funds for cash or cheques from sales that haven’t been deposited yet.\r\nFor petty cash, use Cash on hand, instead." },
                //{ SubAccountType.AllowanceForBadDebt, "Use Allowance for bad debts to estimate the part of Accounts Receivable that you think you might not collect.\r\nUse this only if you are keeping your books on the accrual basis." },
                //{ SubAccountType.OtherCurrentAsset, "Use Other current assets for current assets not covered by the other types. Current assets are likely to be converted to cash or used up in a year." },

                //{ SubAccountType.Building, "Use Buildings to track the cost of structures you own and use for your business. If you have a business in your home, consult your accountant.\r\nUse a Land account for the land portion of any real property you own, splitting the cost of the property between land and building in a logical method. A common method is to mimic the land-to-building ratio on the property tax statement." },
                //{ SubAccountType.Land, "Use Land to track assets that are not easily convertible to cash or not expected to become cash within the next year. For example, leasehold improvements." },
                //{ SubAccountType.Vehicle, "Use Vehicles to track the value of vehicles your business owns and uses for business. This includes off-road vehicles, air planes, helicopters, and boats.\r\nIf you use a vehicle for both business and personal use, consult your accountant to see how you should track its value." }   ,
                //{ SubAccountType.MachinaryAndEquipment, "Use Machinery and equipment to track computer hardware, as well as any other non-furniture fixtures or devices owned and used for your business.\r\nThis includes equipment that you ride, like tractors and lawn mowers. Cars and lorries, however, should be tracked with Vehicle accounts, instead." },
                //{ SubAccountType.FurnitureAndFixture, "Use Furniture and fixtures to track any furniture and fixtures your business owns and uses, like a dental chair or sales booth." },
                //{ SubAccountType.DepletableAsset, "Use Depletable assets to track natural resources, such as timberlands, oil wells, and mineral deposits." },
                //{ SubAccountType.LeaseholdImprovement, "Use Leasehold improvements to track improvements to a leased asset that increases the asset’s value. For example, if you carpet a leased office space and are not reimbursed, that’s a leasehold improvement." },
                //{ SubAccountType.AccumulatedDepletion, "Use Accumulated depletion to track how much you deplete a natural resource." },
                { SubAccountType.AccumulatedDepreciation, "Use Accumulated depreciation on property, plant and equipment to track how much you depreciate a fixed asset (a physical asset you do not expect to convert to cash during one year of normal operations)." },
                //{ SubAccountType.OtherFixedAsset, "Use Other fixed asset for fixed assets that are not covered by other asset types.\r\nFixed assets are physical property that you use in your business and that you do not expect to convert to cash or be used up during one year of normal operations." },

                //{ SubAccountType.AssetHeldForSale, "Use Assets held for sale to track assets of a company that are available for sale that are not expected to be held for a long period of time." },
                //{ SubAccountType.DefferedTax, "Use Deferred tax for tax liabilities or assets that are to be used in future accounting periods." }   ,
                //{ SubAccountType.Goodwill, "Use Goodwill only if you have acquired another company. It represents the intangible assets of the acquired company which gave it an advantage, such as favourable government relations, business name, outstanding credit ratings, location, superior management, customer lists, product quality, or good labour relations." },
                //{ SubAccountType.IntangibleAsset, "Use Intangible assets to track intangible assets that you plan to amortise. Examples include franchises, customer lists, copyrights, and patents." },
                //{ SubAccountType.LeaseBuyout, "Use Lease buyout to track lease payments to be applied toward the purchase of a leased asset.\r\nYou don’t track the leased asset itself until you purchase it." },
                //{ SubAccountType.Licence, "Use Licences to track non-professional licences for permission to engage in an activity, like selling alcohol or radio broadcasting.\r\nFor fees associated with professional licences granted to individuals, use a Legal and professional fees expense account, instead." },
                //{ SubAccountType.LongTermInvestment, "Use Long-term investments to track investments that have a maturity date of longer than one year." },
                //{ SubAccountType.OrganazationalCost, "Use Organisational costs to track costs incurred when forming a partnership or corporation.\r\nThe costs include the legal and accounting costs necessary to organise the company, facilitate the filings of the legal documents, and other paperwork." },
                //{ SubAccountType.SecurityDeposti, "Use Security deposits to track funds you’ve paid to cover any potential costs incurred by damage, loss, or theft.\r\nThe funds should be returned to you at the end of the contract.\r\n\r\nIf you accept down payments, advance payments, security deposits, or other kinds of deposits, use an Other Current liabilities account with the detail type Other Current liabilities." },
                { SubAccountType.AccumulatedAmortization, "Use Accumulated amortisation of non-current assets to track how much you’ve amortised an asset whose type is Non-Current Asset." },
                //{ SubAccountType.OtherNoneCurrentAsset, "Use Other non-current assets to track assets not covered by other types.\r\nNon-current assets are long-term assets that are expected to provide value for more than one year." },

                //{ SubAccountType.AccountPayable, "Accounts payable (also called A/P, Trade and other payables, or Creditors) tracks amounts you owe to your suppliers.\r\nQuickBooks Online Plus automatically creates one Accounts Payable account for you. Most businesses need only one." },

                //{ SubAccountType.CreditCard, "Credit card accounts track the balance due on your business credit cards.\r\nCreate one Credit card account for each credit card account your business uses." },

                //{ SubAccountType.DividentPayable, "Account Type\r\n*Detail Type\r\nUse Dividends payable to track dividends that are owed to shareholders but have not yet been paid." },
                //{ SubAccountType.InsurancePayable, "Use Insurance payable to keep track of insurance amounts due.\r\nThis account is most useful for businesses with monthly recurring insurance expenses." },
                //{ SubAccountType.LoanPayable, "Use Loan payable to track loans your business owes which are payable within the next twelve months.\r\nFor longer-term loans, use the Long-term liability called Notes payable, instead." },
                //{ SubAccountType.PrepaidExpensePayable, "Use Prepaid expenses payable to track items such as property taxes that are due, but not yet deductible as an expense because the period they cover has not yet passed.\r\n" },
                //{ SubAccountType.IncomeTaxPayable, "Use Income tax payable to track monies that are due to pay the company’s income tax liabilties." },
                //{ SubAccountType.SaleAndServiceTaxPayable, "Use Sales and service tax payable to track tax you have collected, but not yet remitted to your government tax agency. This includes value-added tax, goods and services tax, sales tax, and other consumption tax.\r\n" },
                //{ SubAccountType.LineOfCredit, "Use Line of credit to track the balance due on any lines of credit your business has. Each line of credit your business has should have its own Line of credit account." },
                //{ SubAccountType.PayrollClearing, "Use Payroll clearing to keep track of any non-tax amounts that you have deducted from employee paycheques or that you owe as a result of doing payroll. When you forward money to the appropriate suppliers, deduct the amount from the balance of this account.\r\nDo not use this account for tax amounts you have withheld or owe from paying employee wages. For those amounts, use the Payroll tax payable account instead." },
                //{ SubAccountType.PayrollLiability, "Use Payroll liabilities to keep track of tax amounts that you owe to government agencies as a result of paying wages. This includes taxes withheld, health care premiums, employment insurance, government pensions, etc. When you forward the money to the government agency, deduct the amount from the balance of this account.\r\n" },
                //{ SubAccountType.AccruedLiability, "Use Accrued Liabilities to track expenses that a business has incurred but has not yet paid. For example, pensions for companies that contribute to a pension fund for their employees for their retirement." },
                //{ SubAccountType.CurrentPotionOfObligationUnderFinanceLease, "Use Current portion of obligations under finance leases to track the value of lease payments due within the next 12 months." },
                //{ SubAccountType.CurrentTaxLiability, "Use Current tax liability to track the total amount of taxes collected but not yet paid to the government." },
                //{ SubAccountType.ClientTrustAccountLiability, "Use Client Trust accounts - liabilities to offset Client Trust accounts in assets.\r\nAmounts in these accounts are held by your business on behalf of others. They do not belong to your business, so should not appear to be yours on your balance sheet. This \"contra\" account takes care of that, as long as the two balances match." },
                //{ SubAccountType.RentInTrustLiability, "Use Rents in trust - liability to offset the Rents in trust amount in assets.\r\nAmounts in these accounts are held by your business on behalf of others. They do not belong to your business, so should not appear to be yours on your balance sheet. This \"contra\" account takes care of that, as long as the two balances match.\r\n\r\n" },
                //{ SubAccountType.OtherCurrentLiability, "Use Other current liabilities to track monies owed by the company and due within one year.\r\n" },

                //{ SubAccountType.NotePayable, "Use Notes payable to track the amounts your business owes in long-term (over twelve months) loans.\r\nFor shorter loans, use the Current liability account type called Loan payable, instead.\r\n\r\n" },
                //{ SubAccountType.ShareholderNotePayable, "Use Shareholder notes payable to track long-term loan balances your business owes its shareholders.\r\n" },
                //{ SubAccountType.AccruedHolidayPayable, "Use Accrued holiday payable to track holiday earned but that has not been paid out to employees.\r\n" },
                //{ SubAccountType.LongTermDebt, "Use Long-term debt to track loans and obligations with a maturity of longer than one year. For example, mortgages.\r\n" },
                //{ SubAccountType.LiabilityRelatedToAssetHeldForSale, "Use Liabilities related to assets held for sale to track any liabilities that are directly related to assets being sold or written off.\r\n" },
                //{ SubAccountType.AccruedNoneCurrentLiability, "Use Accrued Non-current liabilities to track expenses that a business has incurred but has not yet paid. For example, pensions for companies that contribute to a pension fund for their employees for their retirement.\r\n" },
                //{ SubAccountType.OtherNoneCurrentLiability, "Use Other non-current liabilities to track liabilities due in more than twelve months that don’t fit the other Non-Current liability account types.\r\n" },

                { SubAccountType.OpeningBalanceEquity, "QuickBooks Online Plus creates this account the first time you enter an opening balance for a balance sheet account.\r\nAs you enter opening balances, QuickBooks Online Plus records the amounts in Opening balance equity. This ensures that you have a correct balance sheet for your company, even before you’ve finished entering all your company’s assets and liabilities.\r\n\r\n" },
                //{ SubAccountType.OwnersEquity, "Corporations use Owner’s equity to show the cumulative net income or loss of their business as of the beginning of the financial year.\r\n" },
                //{ SubAccountType.PartnersEquity, "Partnerships use Partner’s equity to show the income remaining in the partnership for each partner as of the end of the prior year.\r\n" },
                //{ SubAccountType.PartnerContribution,  "Partnerships use Partner contributions to track amounts partners contribute to the partnership during the year.\r\n" },
                //{ SubAccountType.PartnerDistribution, "Partnerships use Partner distributions to track amounts distributed by the partnership to its partners during the year.\r\nDon’t use this for regular payments to partners for interest or service. For regular payments, use a Guaranteed payments account (a Expense account in Payroll expenses), instead.\r\n\r\n" },
                //{ SubAccountType.DividendDisbursed, "Use Dividend disbursed to track a payment given to its shareholders out of the company’s retained earnings.\r\n" },
                //{ SubAccountType.ShareCapital, "Use Share capital to track the funds raised by issuing shares.\r\n" },
                //{ SubAccountType.OrdinaryShare, "Corporations use Ordinary shares to track its ordinary shares in the hands of shareholders. The amount in this account should be the stated (or par) value of the stock.\r\n" },
                //{ SubAccountType.PreferredShare, "Corporations use this account to track its preferred shares in the hands of shareholders. The amount in this account should be the stated (or par) value of the shares.\r\n" },
                //{ SubAccountType.TreasuryShare, "Corporations use Treasury shares to track amounts paid by the corporation to buy its own shares back from shareholders.\r\n" },
                //{ SubAccountType.AccumulatedAdjustment, "Some corporations use this account to track adjustments to owner’s equity that are not attributable to net income.\r\n" },
                //{ SubAccountType.OtherComprehensiveIncome, "Use Other comprehensive income to track the increases or decreases in income from various businesses that is not yet absorbed by the company.\r\n" },
                //{ SubAccountType.PaidInCapitalOrSurplus, "Corporations use Paid-in capital to track amounts received from shareholders in exchange for shares that are over and above the shares’ stated (or par) value.\r\n" },
                { SubAccountType.RetainedEarning, "QuickBooks Online Plus adds this account when you create your company.\r\nRetained earnings tracks net income from previous financial years.\r\n\r\nQuickBooks Online Plus automatically transfers your profit (or loss) to Retained earnings at the end of each financial year.\r\n\r\n" },
                //{ SubAccountType.EquityInEarningOfSubsidiaries, "Use Equity in earnings of subsidiaries to track the original investment in shares of subsidiaries plus the share of earnings or losses from the operations of the subsidiary.\r\n" },

                //{ SubAccountType.SaleOfProductIncome, "Use Sales of product income to track income from selling products.\r\nThis can include all kinds of products, like crops and livestock, rental fees, performances, and food served." },
                //{ SubAccountType.ServiceFeeIncome, "Use Service/fee income to track income from services you perform or ordinary usage fees you charge.\r\nFor fees customers pay you for late payments or other uncommon situations, use an Other Income account type called Other miscellaneous income, instead." },
                //{ SubAccountType.SaleRetail, "Use Sales - retail to track sales of goods/services that have a mark-up cost to consumers." },
                //{ SubAccountType.SaleWholesale, "Use Sales - wholesale to track the sale of goods in quantity for resale purposes." },
                //{ SubAccountType.GeneralRevenue, "Use Revenue - General to track income from normal business operations that do not fit under any other category." },
                //{ SubAccountType.OtherPrimaryIncome, "Use Other primary income to track income from normal business operations that doesn’t fall into another Income type." },
                //{ SubAccountType.NonProfitIncome, "Use Non-profit income to track money coming in if you are a non-profit organisation." },
                //{ SubAccountType.DiscountOrRefundGiven, "Use Discounts/refunds given to track discounts you give to customers.\r\nThis account typically has a negative balance so it offsets other income.\r\n\r\nFor discounts from suppliers, use an expense account, instead." },
                //{ SubAccountType.UnappliedCashPaymentIncome, "Unapplied Cash Payment Income reports the Cash Basis income from customers payments you’ve received but not applied to invoices or charges. In general, you would never use this directly on a purchase or sale transaction." },

                //{ SubAccountType.DividendIncome, "Use Dividend income to track taxable dividends from investments." },
                //{ SubAccountType.InterestEarned, "Use Interest earned to track interest from bank or savings accounts, investments, or interest payments to you on loans your business made.\r\n" },
                //{ SubAccountType.OtherInvestmentIncome, "Use Other investment income to track other types of investment income that isn’t from dividends or interest.\r\n" },
                //{ SubAccountType.OtherOperatingIncome, "Use Other operating income to track income from activities other than normal business operations. For example, Investment interest, foreign exchange gains, and rent income.\r\n" },
                //{ SubAccountType.OtherMiscellaneousIncome, "Use Other miscellaneous income to track income that isn’t from normal business operations, and doesn’t fall into another Other Income type.\r\n" },
                //{ SubAccountType.TaxExemptInterest, "Use Tax-exempt interest to record interest that isn’t taxable, such as interest on money in tax-exempt retirement accounts, or interest from tax-exempt bonds.\r\n" },
                //{ SubAccountType.LossOnDisposalOfAsset, "Use Loss on disposal of assets to track losses realised on the disposal of assets.\r\n" },
                //{ SubAccountType.UnrealisedLossOnSecurityNetOfTax, "Use Unrealised loss on securities, net of tax to track losses on securities that have occurred but are yet been realised through a transaction. For example, shares whose value has fallen but that are still being held.\r\n" },

                //{ SubAccountType.LaborCost, "Use Cost of labour - COS to track the cost of paying employees to produce products or supply services.\r\nIt includes all employment costs, including food and transportation, if applicable.\r\n\r\n" },
                //{ SubAccountType.SuppliesAndMaterialsCost, "Use Supplies and materials - COS to track the cost of raw goods and parts used or consumed when producing a product or providing a service.\r\n" },
                //{ SubAccountType.EquipmentRentalCost, "Use Equipment rental - COS to track the cost of renting equipment to produce products or services.\r\nIf you purchase equipment, use a Fixed Asset account type called Machinery and equipment.\r\n\r\n" },
                //{ SubAccountType.FreightAndDeliveryCost, "Use Freight and delivery - COS to track the cost of shipping/delivery of obtaining raw materials and producing finished goods for resale.\r\n" },
                //{ SubAccountType.OtherCostOfSale, "Use Other costs of sales - COS to track costs related to services or sales that you provide that don’t fall into another Cost of Sales type.\r\n" },

                //{ SubAccountType.AdvertisingPromotional, "Use Advertising/promotional to track money spent promoting your company.\r\nYou may want different accounts of this type to track different promotional efforts (Yellow Pages, newspaper, radio, flyers, events, and so on).\r\n\r\nIf the promotion effort is a meal, use Promotional meals instead.\r\n\r\n" },
                //{ SubAccountType.AmortizationExpense, "Use Amortisation expense to track writing off of assets (such as intangible assets or investments) over the projected life of the assets.\r\n" },
                //{ SubAccountType.Auto, "Use Auto to track costs associated with vehicles.\r\nYou may want different accounts of this type to track petrol, repairs, and maintenance.\r\n\r\nIf your business owns a car or lorry, you may want to track its value as a Fixed Asset, in addition to tracking its expenses.\r\n\r\n" },
                //{ SubAccountType.BadDebt, "Use Bad debt to track debt you have written off.\r\n" },
                //{ SubAccountType.BankCharge, "Use Bank charges for any fees you pay to financial institutions.\r\n" },
                //{ SubAccountType.CharitableContribution, "Use Charitable contributions to track gifts to charity.\r\n" },
                //{ SubAccountType.CommisionAndFee, "Use Commissions and fees to track amounts paid to agents (such as brokers) in order for them to execute a trade.\r\n" },
                //{ SubAccountType.LaborExpense, "Use Cost of labour to track the cost of paying employees to produce products or supply services.\r\nIt includes all employment costs, including food and transportation, if applicable.\r\n\r\nThis account is also available as a Cost of Sales (COS) account.\r\n\r\n" },
                //{ SubAccountType.DueAndSubscription, "Use Dues and subscriptions to track dues and subscriptions related to running your business.\r\nYou may want different accounts of this type for professional dues, fees for licences that can’t be transferred, magazines, newspapers, industry publications, or service subscriptions.\r\n\r\n" },
                //{ SubAccountType.EquipmentRental, "Use Equipment rental to track the cost of renting equipment to produce products or services.\r\nThis account is also available as a Cost of Sales account.\r\n\r\nIf you purchase equipment, use a Fixed asset account type called Machinery and equipment.\r\n\r\n" },
                //{ SubAccountType.FinanceCost, "Use Finance costs to track the costs of obtaining loans or credit.\r\nExamples of finance costs would be credit card fees, interest and mortgage costs.\r\n\r\n" },
                //{ SubAccountType.IncomeTaxExpense, "Use Income tax expense to track income taxes that the company has paid to meet their tax obligations.\r\n" },
                //{ SubAccountType.Insurance, "Use Insurance to track insurance payments.\r\nYou may want different accounts of this type for different types of insurance (auto, general liability, and so on).\r\n\r\n" },
                //{ SubAccountType.InterestPaid, "Use Interest paid for all types of interest you pay, including mortgage interest, finance charges on credit cards, or interest on loans.\r\n" },
                //{ SubAccountType.LegalAndProfessionalFee, "Use Legal and professional fees to track money to pay to professionals to help you run your business.\r\nYou may want different accounts of this type for payments to your accountant, attorney, or other consultants.\r\n\r\n" },
                //{ SubAccountType.LossOnDiscontinuedOperationNetOfTax, "Use Loss on discontinued operations, net of tax to track the loss realised when a part of the business ceases to operate or when a product line is discontinued.\r\n" },
                //{ SubAccountType.ManagementCompensation, "Use Management compensation to track remuneration paid to Management, Executives and non-Executives. For example, salary, fees, and benefits.\r\n" },
                //{ SubAccountType.MealAndEntertai, "Use Meals and entertainment to track how much you spend on dining with your employees to promote morale.\r\nIf you dine with a customer to promote your business, use a Promotional meals account, instead.\r\n\r\nBe sure to include who you ate with and the purpose of the meal when you enter the transaction.\r\n\r\n" },
                //{ SubAccountType.AdministrativeExpense, "Use Office/general administrative expenses to track all types of general or office-related expenses.\r\n" },
                //{ SubAccountType.OtherMiscelleneousServiceCost, "Use Other miscellaneous service cost to track costs related to providing services that don’t fall into another Expense type.\r\nThis account is also available as a Cost of Sales (COS) account.\r\n\r\n" },
                //{ SubAccountType.OtherSellingExpense, "Use Other selling expenses to track selling expenses incurred that do not fall under any other category.\r\n" },
                //{ SubAccountType.PayrollExpense, "Use Payroll expenses to track payroll expenses. You may want different accounts of this type for things like:\r\nCompensation of officers\r\nGuaranteed payments\r\nWorkers compensation\r\nSalaries and wages\r\nPayroll taxes\r\n" },
                //{ SubAccountType.RentOrLeaseOfBuilding, "Use Rent or lease of buildings to track rent payments you make.\r\n" },
                //{ SubAccountType.RepairAndMaintenance, "Use Repair and maintenance to track any repairs and periodic maintenance fees.\r\nYou may want different accounts of this type to track different types repair & maintenance expenses (auto, equipment, landscape, and so on).\r\n\r\n" },
                //{ SubAccountType.ShipingAndDeliveryExpense, "Use Shipping and delivery expense to track the cost of shipping and delivery of goods to customers.\r\n" },
                //{ SubAccountType.SuppliesAndMaterials, "Use Supplies & materials to track the cost of raw goods and parts used or consumed when producing a product or providing a service.\r\nThis account is also available as a Cost of Sales account.\r\n\r\n" },
                //{ SubAccountType.TaxPaid, "Use Taxes paid to track taxes you pay.\r\nYou may want different accounts of this type for payments to different tax agencies.\r\n\r\n" },
                //{ SubAccountType.TravelGeneralAndAdminExpense, "Use Travel expenses - general and admin expenses to track travelling costs incurred that are not directly related to the revenue-generating operation of the company. For example, flight tickets and hotel costs when performing job interviews.\r\n" },
                //{ SubAccountType.TravelSellExpense, "Use Travel expenses - selling expense to track travelling costs incurred that are directly related to the revenue-generating operation of the company. For example, flight tickets and hotel costs when selling products and services.\r\n" },
                //{ SubAccountType.UnappliedCashBillPaymentExpense, "Unapplied Cash Bill Payment Expense reports the Cash Basis expense from supplier payment cheques you’ve sent but not yet applied to supplier bills. In general, you would never use this directly on a purchase or sale transaction.\r\n" },
                //{ SubAccountType.Utilities, "Use Utilities to track utility payments.\r\nYou may want different accounts of this type to track different types of utility payments (gas and electric, telephone, water, and so on).\r\n\r\n" },

                //{ SubAccountType.Amortization, "Use Amortisation to track amortisation of intangible assets.\r\nAmortisation is spreading the cost of an intangible asset over its useful life, like depreciation of fixed assets.\r\n\r\nYou may want an amortisation account for each intangible asset you have.\r\n\r\n" },
                //{ SubAccountType.Depreciation, "Use Depreciation to track how much you depreciate fixed assets.\r\nYou may want a depreciation account for each fixed asset you have.\r\n\r\n" },
                //{ SubAccountType.ExchangeLossGain, "Use Exchange Gain or Loss to track gains or losses that occur as a result of exchange rate fluctuations.\r\n" },
                //{ SubAccountType.PenaltiesAndSettlements, "Use Penalties and settlements to track money you pay for violating laws or regulations, settling lawsuits, or other penalties.\r\n" },
                //{ SubAccountType.OtherExpense, "Use Other expense to track unusual or infrequent expenses that don’t fall into another Other Expense type.\r\n" }
            };

            return descriptions.ContainsKey(type) ? descriptions[type] : string.Empty;
        }
    }
}
