using System;
using System.Collections.Generic;
using System.Text;

namespace CorarlERP.AccountTypes
{
    public enum AccountTypeEnums
    {   
        Cash = 1,
        Bank = 2,
        AR = 3,
        Inventory = 4,
        OtherCurrentAssets = 5,
        FixedAssets = 6,
        AP = 7,
        OtherCurrentLiability = 8,
        LongTermLiability = 9,
        Equity = 10,
        Revenue = 11,
        OtherRevenue = 12,
        COGS = 13,
        Expense = 14,
        OtherExpense = 15,
    }

    public static class AccountTypesExtensions
    {
        /*@"
        1	Cash 
        2	Bank 
        3	A/R
        4	Inventory 
        5	Other Current Assets
        6	Fixed Assets
        7	A/P 
        8	Other Current Liability 
        9	Long Term Liability
        10	Equity
        11	Revenue
        12	Other Revenue
        13	COGS
        14	Expense
        15	Other Expense
        "*/

        public static string GetName (this AccountTypeEnums accountTypes){
            
            var name = string.Empty;

            switch(accountTypes) {
                case AccountTypeEnums.Cash: name = "Cash"; break; 
                case AccountTypeEnums.Bank: name = "Bank"; break;
                case AccountTypeEnums.AR : name = "A/R"; break;
                case AccountTypeEnums.Inventory: name = "Inventory"; break;
                case AccountTypeEnums.OtherCurrentAssets: name = "Other Current Assets"; break;
                case AccountTypeEnums.FixedAssets: name = "Fixed Assets"; break;
                case AccountTypeEnums.AP: name = "A/P"; break;
                case AccountTypeEnums.OtherCurrentLiability: name = "Other Current Liability"; break;
                case AccountTypeEnums.LongTermLiability: name = "Long Term Liability"; break;
                case AccountTypeEnums.Equity: name = "Equity"; break;
                case AccountTypeEnums.Revenue: name = "Revenue"; break;
                case AccountTypeEnums.OtherRevenue: name = "Other Revenue"; break;
                case AccountTypeEnums.COGS: name = "COGS"; break;
                case AccountTypeEnums.Expense: name = "Expense"; break;
                case AccountTypeEnums.OtherExpense: name = "Other Expense"; break;
            }

            return name;
        }
    }
}
